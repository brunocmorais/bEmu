using System;
using System.IO;

namespace bEmu.Core
{
    public abstract class State : IState
    {
        public ISystem System { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }
        public int Cycles { get; set; }
        public bool Halted { get; set; }
        public int Instructions { get; set; }
    }
}