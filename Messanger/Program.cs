using UDPService;

Console.Write("ваш порт: ");
ReadLine(out var localPort);
Console.Write("порт для подключения: ");
ReadLine(out var remotePort);
Console.Write("Имя пользователя: ");
var username = Console.ReadLine();
Console.WriteLine("Для выхода напишите \"exit\"");
if (string.IsNullOrWhiteSpace(username)) { username = "unnamed user"; Console.WriteLine(username); }

var _udpService = new UdpService(localPort, remotePort);

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
    if (newMessage == "exit") { cts.Cancel(); }
    var createdMessage = await _udpService.SendMessageAsync(username, newMessage);
}

static void ReadLine(out int port)
{
    while (!int.TryParse(Console.ReadLine(), out port)) { Console.Write("Неверный формат записи, введите номер порта: "); };
}
