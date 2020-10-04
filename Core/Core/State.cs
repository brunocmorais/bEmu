using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using bEmu.Core.Util;

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
        public virtual void Reset()
        {
            PC = 0;
            SP = 0;
            Cycles = 0;
            Halted = false;
            Instructions = 0;
        }

        public abstract byte[] SaveState();
        public abstract void LoadState(byte[] value);
    }
}