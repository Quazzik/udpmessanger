namespace UDPService
{
    public class MessageDto
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }

        public override string ToString()
        {
            return $"{UserName} [{SentAt:G}]: {Text}";
        }
    }
}
