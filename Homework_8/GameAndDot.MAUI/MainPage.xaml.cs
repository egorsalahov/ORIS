using GameAndDot.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text.Json;
using GameAndDot.MAUI.Converters;
using GameAndDot.Shared.Enums;

namespace GameAndDot.MAUI
{
    // Модель точки для рисования
    public record Dot(int X, int Y, Color Color);

    public partial class MainPage : ContentPage
    {
        private TcpClient _client = new();
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private string _username = string.Empty;

        // Список точек, который будет использоваться для отрисовки
        private readonly ObservableCollection<Dot> _dots = new();

        public MainPage()
        {
            InitializeComponent();
            DotDrawer.Dots = _dots; // Связываем коллекцию с отрисовщиком
            ColorLabel.Text = "Ожидание"; // Начальное состояние
        }

        // 1. ЛОГИКА ВХОДА
        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            _username = UsernameEntry.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(_username))
            {
                await DisplayAlert("Ошибка", "Введите имя пользователя.", "OK");
                return;
            }

            ConnectButton.IsEnabled = false;
            ActivityIndicator.IsRunning = ActivityIndicator.IsVisible = true;

            try
            {
                string host = "127.0.0.1";
                int port = 8888;

                await _client.ConnectAsync(host, port);

                var stream = _client.GetStream();
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream, leaveOpen: true);

                // Отправляем сообщение о подключении
                var message = new EventMessage()
                {
                    type = EventType.PlayerConnected,
                    Username = _username
                };

                string json = JsonSerializer.Serialize(message);
                await SendMessageAsync(json);

                // Запускаем цикл приема сообщений
                Task.Run(ReceiveMessageLoop);

                // Скрываем логин, показываем игру
                LoginLayout.IsVisible = false;
                InfoBarLayout.IsVisible = true;
                GameField.IsVisible = true;
                PlayersListLayout.IsVisible = true;
                UsernameLabel.Text = _username;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка подключения", $"Не удалось подключиться: {ex.Message}", "OK");
                _client.Close();
                _client = new TcpClient(); // Сброс клиента
            }
            finally
            {
                ConnectButton.IsEnabled = true;
                ActivityIndicator.IsRunning = ActivityIndicator.IsVisible = false;
            }
        }

        // 2. ОТПРАВКА ТОЧКИ
        private async void GameField_Tapped(object sender, TappedEventArgs e)
        {
            var tapPoint = e.GetPosition((View)sender);

            if (tapPoint.HasValue)
            {
                string playerColor = ColorLabel.Text;

                if (string.IsNullOrEmpty(playerColor) || playerColor == "Ожидание")
                {
                    // Сообщение будет видно в консоли и в логах, а не как Alert (чтобы не мешать)
                    Console.WriteLine("Ожидание цвета от сервера.");
                    return;
                }

                var msg = new EventMessage()
                {
                    type = EventType.PointPlaced,
                    Username = _username,
                    Color = playerColor,
                    X = (int)tapPoint.Value.X,
                    Y = (int)tapPoint.Value.Y
                };

                string json = JsonSerializer.Serialize(msg);
                await SendMessageAsync(json);
            }
        }

        private async Task SendMessageAsync(string message)
        {
            if (_writer == null) return;
            try
            {
                await _writer.WriteLineAsync(message);
                await _writer.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки: {ex.Message}");
            }
        }

        // 3. ПРИЕМ СООБЩЕНИЙ
        private async Task ReceiveMessageLoop()
        {
            while (_client.Connected && _reader != null)
            {
                try
                {
                    string? jsonRequest = await _reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(jsonRequest)) break;

                    var messageRequest = JsonSerializer.Deserialize<EventMessage>(jsonRequest);

                    // Обновление UI всегда в главном потоке!
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        switch (messageRequest?.type)
                        {
                            case EventType.PlayerConnected:
                                HandlePlayerConnected(messageRequest);
                                break;
                            case EventType.PointPlaced:
                                HandlePointPlaced(messageRequest);
                                break;
                        }
                    });
                }
                catch (IOException) { break; }
                catch (Exception ex) { Console.WriteLine($"Ошибка при получении: {ex.Message}"); }
            }

            // Обработка потери соединения
            if (_client.Connected) _client.Close();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert("Внимание", "Соединение с сервером потеряно.", "OK");
                // Возврат UI в состояние входа
                LoginLayout.IsVisible = true;
                InfoBarLayout.IsVisible = false;
                GameField.IsVisible = false;
                PlayersListLayout.IsVisible = false;
                _client = new TcpClient();
            });
        }

        // 4. ОБРАБОТЧИКИ СОБЫТИЙ СЕРВЕРА

        private void HandlePlayerConnected(EventMessage message)
        {
            // 1. Устанавливаем цвет текущего игрока
            var currentPlayer = message.Players
                .FirstOrDefault(p => p.Username == _username);

            if (currentPlayer != null)
            {
                ColorLabel.Text = currentPlayer.Color;

                if (ColorConverter.TryParseColor(currentPlayer.Color, out Color mauiColor))
                {
                    ColorLabel.TextColor = mauiColor;
                }
                else
                {
                    ColorLabel.TextColor = Colors.Black;
                }
            }

            // 2. Обновляем список игроков
            PlayersList.ItemsSource = new ObservableCollection<PlayerInfo>(message.Players);
        }

        private void HandlePointPlaced(EventMessage message)
        {
            if (ColorConverter.TryParseColor(message.Color, out Color dotColor))
            {
                _dots.Add(new Dot(message.X, message.Y, dotColor));

                // Заставляем GraphicsView перерисоваться
                GameField.Invalidate();
            }
        }
    }
}
