using System;
using bEmu.Core.Enums;

namespace bEmu.Core.UI
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime ExpirationDate { get; set; }
        public MessageType Type { get; set; }
    }
}