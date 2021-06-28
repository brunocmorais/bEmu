using System;

namespace bEmu.Components
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime ExpirationDate { get; set; }
        public MessageType Type { get; set; }
    }
}