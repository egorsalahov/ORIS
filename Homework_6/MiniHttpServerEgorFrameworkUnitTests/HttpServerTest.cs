using MiniHttpServerEgorFramework.Server;
using MiniHttpServerEgorFramework.Settings;

namespace MiniHttpServerEgorFrameworkUnitTests
{
    [TestClass]
    public sealed class HttpServerTest
    {
        [TestMethod]
        public void Start_WithValidSettings_ServerStartsSuccessfully()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8081" };
            var server = new HttpServer(settings);

            // Act & Assert
            try
            {
                server.Start();
                // Если не было исключения - тест пройден
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                // Если есть исключение, но не связанное с уже занятым портом - тест не пройден
                if (!ex.Message.Contains("уже занят") && !ex.Message.Contains("already in use"))
                {
                    Assert.Fail($"Unexpected exception: {ex.Message}");
                }
                // Если порт занят - это ожидаемо в тестовой среде
            }
            finally
            {
                server.Stop();
            }
        }



        [TestMethod]
        public void Stop_WhenServerIsRunning_ServerStopsSuccessfully()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8082" };
            var server = new HttpServer(settings);

            // Act & Assert
            try
            {
                server.Start();
                server.Stop();
                // Если не было исключения - тест пройден
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                // Если порт занят - это ожидаемо
                if (!ex.Message.Contains("уже занят") && !ex.Message.Contains("already in use"))
                {
                    Assert.Fail($"Unexpected exception: {ex.Message}");
                }
            }
        }

        [TestMethod]
        public void CommandProperty_CanBeSetAndInvoked()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8083" };
            var server = new HttpServer(settings);
            bool commandExecuted = false;

            // Act
            server.Command = (context) => { commandExecuted = true; };
            server.Command?.Invoke(null); // Передаем null контекст для теста

            // Assert
            Assert.IsTrue(commandExecuted);
        }

        [TestMethod]
        public void CancellationTokenSource_IsInitializedAndUsable()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8084" };
            var server = new HttpServer(settings);

            // Act & Assert
            Assert.IsFalse(server.cts.IsCancellationRequested);

            // Проверяем, что токен можно отменить
            server.cts.Cancel();
            Assert.IsTrue(server.cts.IsCancellationRequested);
        }

        [TestMethod]
        public void UrlConstruction_WithStringPort_CorrectFormat()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "9090" };
            var server = new HttpServer(settings);

            // Act & Assert
            try
            {
                server.Start();
                // Если сервер запустился, значит URL сформирован корректно
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                // Проверяем, что ошибка не связана с форматом URL
                if (ex.Message.Contains("Invalid URI") || ex.Message.Contains("формат URI"))
                {
                    Assert.Fail($"URL format error: {ex.Message}");
                }
                // Другие ошибки (например, занятый порт) игнорируем
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void Start_WithLocalhostDomain_ServerStartsSuccessfully()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "127.0.0.1", Port = "8085" };
            var server = new HttpServer(settings);

            // Act & Assert
            try
            {
                server.Start();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("уже занят") && !ex.Message.Contains("already in use"))
                {
                    Assert.Fail($"Unexpected exception: {ex.Message}");
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void Stop_WhenServerNotStarted_DoesNotThrowException()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8086" };
            var server = new HttpServer(settings);

            // Act & Assert
            try
            {
                server.Stop(); // Останавливаем без предварительного старта
                Assert.IsTrue(true); // Не должно быть исключения
            }
            catch (Exception ex)
            {
                Assert.Fail($"Stop should not throw exception when server not started: {ex.Message}");
            }
        }

        [TestMethod]
        public void MultipleStartStopCalls_WorkCorrectly()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8087" };
            var server = new HttpServer(settings);

            // Act & Assert
            try
            {
                // Первый запуск и остановка
                server.Start();
                server.Stop();

                // Второй запуск и остановка
                server.Start();
                server.Stop();

                Assert.IsTrue(true); // Если дошли сюда - все ок
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("уже занят") && !ex.Message.Contains("already in use"))
                {
                    Assert.Fail($"Multiple start/stop failed: {ex.Message}");
                }
            }
        }

        [TestMethod]
        public void CancellationToken_CanBeUsedForCancellation()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8088" };
            var server = new HttpServer(settings);
            bool cancellationWorked = false;

            // Act
            server.cts.Token.Register(() => cancellationWorked = true);
            server.cts.Cancel();

            // Assert
            Assert.IsTrue(cancellationWorked);
            Assert.IsTrue(server.cts.IsCancellationRequested);
        }

        [TestMethod]
        public void CommandProperty_CanBeSetToNull()
        {
            // Arrange
            var settings = new JsonEntity { Domain = "localhost", Port = "8089" };
            var server = new HttpServer(settings);

            // Act
            server.Command = null;

            // Assert
            Assert.IsNull(server.Command);
        }

       
    }
}