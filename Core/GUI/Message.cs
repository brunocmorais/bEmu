using System;
using bEmu.Core.Enums;

namespace bEmu.Core.GUI
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime ExpirationDate { get; set; }
        public MessageType Type { get; set; }
    }
}