using UDPService;

namespace Messanger
{
    class Program
    {
        private static int _remotePort = 22;
        private static int _localPort = 22;

        static void Main(string[] args)
        {
            MainAsync();
        }
        static async void MainAsync()
        {
            Console.Write("Имя пользователя: ");
            var username = Console.ReadLine();
            Console.WriteLine("Для выхода напишите \"exit\"");
            Console.WriteLine("Для смены порта напишите \"port\"");
            if (string.IsNullOrWhiteSpace(username)) { username = "unnamed user"; Console.WriteLine(username); }

            var _udpService = new UdpService(_localPort, _remotePort);

            CancellationTokenSource cts = new CancellationTokenSource();
            var token = cts.Token;

            _ = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var message = await _udpService.ReceiveMessageAsync();

                    if (message == null)
                        continue;
                    Console.WriteLine(message);
                }
            }, token);

            await _udpService.SendMessageAsync(username, $"{username} connected");

            while (!token.IsCancellationRequested)
            {
                var newMessage = Console.ReadLine();
                if (newMessage == "port") { GetNewPorts(); _udpService = new UdpService(_localPort, _remotePort); Console.WriteLine("Порты изменены"); continue; }
                if (newMessage == "exit") { cts.Cancel(); continue; }
                await _udpService.SendMessageAsync(username, newMessage);
            }
        }
        static void ReadLine(out int port)
        {
            while (!int.TryParse(Console.ReadLine(), out port)) { Console.Write("Неверный формат записи, введите номер порта: "); };
        }
        static void GetNewPorts()
        {
            Console.Write("ваш порт: ");
            ReadLine(out _localPort);
            Console.Write("порт для подключения: ");
            ReadLine(out _remotePort);
        }
    }
}



