using System;

namespace bEmu.Core.GUI.Popups
{
    public interface IButton
    {
        public string Text { get; }
        public Action Action { get; }
        bool Close { get; }
    }
}