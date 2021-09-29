using bEmu.Core.CPU;
using bEmu.Core.Util;

namespace bEmu.Core.System
{
    public abstract class State<TPC, TSP> : IState, IProgramCounter<TPC>, IStackPointer<TSP> 
        where TPC : struct
        where TSP : struct
    {
        public State(IRunnableSystem system)
        {
            System = system;
        }

        public IRunnableSystem System { get; }
        public Number<TPC> PC { get; set; }
        public Number<TSP> SP { get; set; }
        public int Cycles { get; set; }
        public bool Halted { get; set; }
        public ulong Instructions { get; set; }
        public abstract IEndianness Endianness { get; }

        public virtual void Reset()
        {
            Cycles = 0;
            Halted = false;
            Instructions = 0;
        }

        public abstract byte[] SaveState();
        public abstract void LoadState(byte[] value);
    }
}