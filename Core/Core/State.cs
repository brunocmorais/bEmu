using System;
using System.IO;

namespace bEmu.Core
{
    public abstract class State : IState
    {
        public State(ISystem system)
        {
            System = system;
        }

        public ISystem System { get; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }
        public int Cycles { get; set; }
        public bool Halted { get; set; }
        public int Instructions { get; set; }
    }
}