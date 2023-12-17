using Logger;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace UDPService
{
    public class UdpService
    {
        private Logger.Logger _logger;
        private readonly int _localPort;
        private readonly int _remotePort;
        private readonly IPAddress _localAddress = IPAddress.Parse("127.0.0.1");

        public UdpService(int localPort, int remotePort, string logPath = null)
        {
            StartLogging(logPath);
            _localPort = localPort;
            _remotePort = remotePort;
        }

        public async Task<MessageDto?> SendMessageAsync(string userName, string messageText)
        {
            try
            {
                var sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                var endPoint = new IPEndPoint(_localAddress, _remotePort);
                await sender.ConnectAsync(endPoint);
                var messageDto = new MessageDto
                {
                    Text = messageText,
                    UserName = userName,
                    SentAt = DateTime.Now
                };

                var messageJsonStr = JsonConvert.SerializeObject(messageDto);
                var messageBytes = messageJsonStr.GetBytes();
                await sender.SendAsync(messageBytes);
                return messageDto;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
                return null;
            }
        }

        public async Task<MessageDto?> ReceiveMessageAsync()
        {
            try
            {
                byte[] buffer = new byte[8192];
                using var receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(_localAddress, _localPort);
                receiver.Bind(endPoint);
                var messageSizeByte = await receiver.ReceiveAsync(buffer);
                var messageJsonStr = buffer.GetString(messageSizeByte);
                return JsonConvert.DeserializeObject<MessageDto>(messageJsonStr);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
                return null;
            }
        }

        private string GetLogNum(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return fileName.Replace("log", "").Replace(".txt", "");
        }

        private void StartLogging(string logPath)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var logFilePath = Path.GetFullPath(appDataPath);

            var logFolder = Path.Combine(logFilePath, "Logs");
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            if (string.IsNullOrWhiteSpace(logPath))
            {
                var logFilesNums = Directory.GetFiles(logFolder)
                .Where(fileName => Path.GetFileName(fileName).StartsWith("log"))
                .Select(filePath =>
                    int.TryParse(GetLogNum(filePath), out var fileNumber)
                    ? fileNumber
                    : 0);

                var logFile = logFilesNums.Any() ? logFilesNums.Max() + 1 : 1;

                logPath = Path.Combine(logFolder, $"log{logFile}.txt");
            }

            File.Create(logPath).Dispose();
            _logger = new Logger.Logger(logPath);
        }
    }
}