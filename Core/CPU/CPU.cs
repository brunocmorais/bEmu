using bEmu.Core.Enums;
using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.CPU
{
    public abstract class CPU<TState, TMMU> : ICPU<TState, TMMU> 
        where TState : class, IState, IProgramCounter<ushort>
        where TMMU : class, IMMU
    {
        public IRunnableSystem System { get; }
        public TState State => System.State as TState;
        public TMMU MMU => System.MMU as TMMU;
        public int Clock { get; }
        public IEndianness Endianness { get; }

        public CPU(Endianness endianness, IRunnableSystem system, int clock)
        {
            Endianness = EndiannessFactory.Instance.Get(endianness);
            System = system;
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
            return Endianness.GetWordFrom2Bytes(b1, b2);
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
            return Endianness.GetWordFrom2Bytes(a, b);
        }

        protected virtual void WriteWordToMemory(ushort addr, ushort word)
        {
            Endianness.Get2BytesFromWord(word, out byte a, out byte b);
            MMU[addr] = b;
            MMU[addr + 1] = a;
        }

        protected virtual bool CheckZero(byte value)
        {
            return value == 0;
        }

        protected virtual bool CheckHalfCarry(ushort a, ushort b, ushort result)
        {
            return (((a ^ b ^ result) & 0x10) == 0x10);
        }

        protected virtual bool CheckHalfCarry(byte a, byte b, byte result)
        {
            return (((a ^ b ^ result) & 0x10) == 0x10);
        }

        protected virtual bool CheckNegative(byte value)
        {
            return (value & 0x80) == 0x80;
        }
    }
}