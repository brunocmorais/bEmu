using System.Collections.Generic;

namespace bEmu.Core.GUI
{
    public interface IManager<T> where T : IDrawable
    {
        Stack<T> Items { get; }
        T Current { get; }
        bool IsOpen { get; }
        IMain Game { get; }

        void CloseAll();
        T CloseCurrent();
        void Open(T item);
        void UpdateControls(double totalMilliseconds);
        void Update();
    }
}