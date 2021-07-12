using System;
using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Enums;

namespace bEmu.Core.UI
{
    public class OSD : IOSD
    {
        private readonly List<Message> messages = new List<Message>();
        public IEnumerable<Message> Messages => messages.Select(x => x);

        public void Update()
        {
            messages.RemoveAll(x => messages.Where(x => x.ExpirationDate < DateTime.Now).Contains(x));
        }

        public void UpdateMessage(MessageType type, string messageText)
        {
            if (type == MessageType.Default || !messages.Any(x => x.Type == type))
                return;

            var message = messages.First(x => x.Type == type);

            message.Text = messageText;
        }

        public void InsertMessage(MessageType type, string messageText)
        {
            DateTime expirationDate = type == MessageType.Default ? DateTime.Now.AddSeconds(3) : DateTime.MaxValue;
            messages.Add(new Message() { Text = messageText, Type = type, ExpirationDate = expirationDate });
        }

        public void RemoveMessage(MessageType type)
        {
            if (type == MessageType.Default)
                return;

            messages.RemoveAll(x => x.Type == type);
        }
    }
}