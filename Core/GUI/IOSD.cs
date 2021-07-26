using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.GUI
{
    public interface IOSD : IDrawable
    {
        IEnumerable<Message> Messages { get; }
        void InsertMessage(MessageType type, string messageText);
        void RemoveMessage(MessageType type);
        void UpdateMessage(MessageType type, string messageText);
    }
}