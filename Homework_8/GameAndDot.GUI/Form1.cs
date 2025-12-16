using GameAndDot.Shared;
using GameAndDot.Shared.Models;
using System.Drawing;
using System.Net.Sockets;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GameAndDot.GUI
{
    public partial class Form1 : Form
    {
        private readonly StreamReader? _reader;
        private readonly StreamWriter? _writer;

        private readonly TcpClient _client;


        private readonly string host;
        private readonly int port;

        private List<(int x, int y, string color)> points = new(); //точки


        public Form1()
        {
            InitializeComponent();

            var config = ConfigLoader.Load();
            host = config.host;
            port = config.port;


            _client = new TcpClient();



            try
            {
                _client.Connect(host, port); //подключение клиента
                

                _reader = new StreamReader(_client.GetStream());
                _writer = new StreamWriter(_client.GetStream());

                listboxPlayers.DrawMode = DrawMode.OwnerDrawFixed; //базарю что сам решаю как в списке пацанов выводить
                listboxPlayers.DrawItem += listboxPlayers_DrawItem; //метод добавления в список определенным цветом

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //метод отрисовки цвета
        private void listboxPlayers_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // Текст элемента
            string itemText = listboxPlayers.Items[e.Index].ToString();

            // Парсим "Username (Red)"
            var parts = itemText.Split('(', ')');
            string username = parts[0].Trim();
            string colorName = parts.Length > 1 ? parts[1].Trim() : "Black";

            Color playerColor = Color.FromName(colorName);

            // фон чтоб был фиксированный
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            if (selected)
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            else
                e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);

            // для текста
            Brush brush = selected
                ? Brushes.White
                : new SolidBrush(playerColor);  //красим всю строку цветом игррока

            e.Graphics.DrawString(itemText, e.Font, brush, e.Bounds);

            e.DrawFocusRectangle();
        }

        private async void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            // не отправляем, если нет writer (иначе ошибка)
            if (_writer == null)
                return;

            string playerColor = colorLbl.Text;

            var msg = new EventMessage()
            {
                type = Shared.Enums.EventType.PointPlaced,
                Username = usernameLbl.Text,
                Color = playerColor,
                X = e.X,
                Y = e.Y
            };

            string json = JsonSerializer.Serialize(msg);
            await SendMessageAsync(json);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;


            label2.Visible = true;
            label4.Visible = true;
            usernameLbl.Visible = true;
            colorLbl.Visible = true;
            listboxPlayers.Visible = true;

            string userName = textBox1.Text;
            usernameLbl.Text = userName;

            // запускаем новый поток для получения данных, ожидание сообщения
            Task.Run(() => ReceiveMessageAsync());

            var message = new EventMessage()
            {
                type = Shared.Enums.EventType.PlayerConnected,
                Username = userName
            };

            string json = JsonSerializer.Serialize(message);

            // запускаем ввод сообщений
            await SendMessageAsync(json);
        }

        // отправка сообщений
        async Task SendMessageAsync(string message)
        {
            // сначала отправляем имя
            await _writer.WriteLineAsync(message);
            await _writer.FlushAsync(); //очистка

        }

        // получение сообщений от сервера
        async Task ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                    string? jsonRequest = await _reader.ReadLineAsync();
                    var messageRequest = JsonSerializer.Deserialize<EventMessage>(jsonRequest);

                    switch (messageRequest.type)
                    {
                        case Shared.Enums.EventType.PlayerConnected:
                            Invoke(() =>
                            {
                                // 1. меняем цвет игрока
                                var currentPlayer = messageRequest.Players
                                    .FirstOrDefault(p => p.Username == usernameLbl.Text);

                                if (currentPlayer != null)
                                {
                                    // Устанавливаем текст (имя цвета)
                                    colorLbl.Text = currentPlayer.Color;

                                    // ставим цвет
                                    try
                                    {
                                        // Преобразуем строку ("Red") в объект Color.
                                        colorLbl.ForeColor = Color.FromName(currentPlayer.Color);
                                    }
                                    catch
                                    {
                                        // Обработка на случай, если строка цвета не распознана 
                                        colorLbl.ForeColor = Color.Black;
                                    }
                                }

                                // 2 обноление списка братишек
                                listboxPlayers.Items.Clear();
                                foreach (var player in messageRequest.Players)
                                {
                                    // Отображаем имя И цвет в списке
                                    listboxPlayers.Items.Add($"{player.Username} ({player.Color})");
                                }
                            });
                            break;


                        case Shared.Enums.EventType.PointPlaced:
                            Invoke(() =>
                            {
                                points.Add((messageRequest.X, messageRequest.Y, messageRequest.Color));
                                this.Invalidate(); // перерисовать форму
                            });
                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        // чтобы полученное сообщение не накладывалось на ввод нового сообщения

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var p in points)
            {
                Color color = Color.FromName(p.color);

                using (Brush b = new SolidBrush(color))
                {
                    // рисуем маленький круг (диаметр 10px)
                    e.Graphics.FillEllipse(b, p.x - 5, p.y - 5, 10, 10);
                }
            }
        }


        private void usernameLbl_Click(object sender, EventArgs e)
        {

        }

       
    }
}
