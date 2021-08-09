using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.CPU
{
    public abstract class CPU<TState, TMMU> : ICPU<TState, TMMU> 
        where TState : class, IState, IProgramCounter<ushort>
        where TMMU : class, IMMU
    {
        public ISystem System { get; }
        public TState State { get; }
        public TMMU MMU { get; }
        public int Clock { get; }

        public CPU(ISystem system, int clock)
        {
            System = system;
            State = system.State as TState;
            MMU = system.MMU as TMMU;
            Clock = clock;
        }

        public virtual IOpcode StepCycle()
        {
            State.Instructions++;
            return default(IOpcode);
        }

        protected virtual void IncreaseCycles(sbyte cycles)
        {
            State.Cycles += cycles;
        }

        protected virtual ushort GetNextWord()
        {
            byte b1 = MMU[State.PC++];
            byte b2 = MMU[State.PC++];
            return ByteOperations.GetWordFrom2Bytes(b1, b2);
        }

        protected virtual byte GetNextByte()
        {
            return MMU[State.PC++];
        }

        protected virtual byte ReadByteFromMemory(ushort addr)
        {
            return MMU[addr];
        }

        protected virtual void WriteByteToMemory(ushort addr, byte value)
        {
            MMU[addr] = value;
        }

        protected virtual ushort ReadWordFromMemory(ushort addr)
        {
            byte a = MMU[addr];
            byte b = MMU[addr + 1];
            return ByteOperations.GetWordFrom2Bytes(a, b);
        }

        protected virtual void WriteWordToMemory(ushort addr, ushort word)
        {
            ByteOperations.Get2BytesFromWord(word, out byte a, out byte b);
            MMU[addr] = b;
            MMU[addr + 1] = a;
        }
    }
}