using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.UI
{
    public interface IOSD
    {
        IEnumerable<Message> Messages { get; }
        void InsertMessage(MessageType type, string messageText);
        void RemoveMessage(MessageType type);
        void Update();
        void UpdateMessage(MessageType type, string messageText);
    }
}