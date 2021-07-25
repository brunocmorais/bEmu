using System.Collections.Generic;
using System.Linq;

namespace bEmu.Core.GUI
{
    public abstract class Manager<T> : IManager<T>
    {
        public Stack<T> Items { get; }
        public T Current => Items.Any() ? Items.Peek() : default;
        public bool IsOpen => Items.Any();
        public IMain Game { get; }

        public Manager(IMain game)
        {
            Game = game;
            Items = new Stack<T>();
        }

        public virtual void CloseAll()
        {
            Items.Clear();
        }

        public virtual T CloseCurrent()
        {
            Items.TryPop(out T result);
            return result;
        }

        public virtual void Open(T popup)
        {
            Items.Push(popup);
        }

        public abstract void Update(double totalMilliseconds);
    }
}