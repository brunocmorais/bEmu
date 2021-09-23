using bEmu.Core.CPU;

namespace bEmu.Core.System
{
    public interface IState : ISaveable
    {
        IRunnableSystem System { get; }
        int Cycles { get; set; }
        bool Halted { get; set; }
        ulong Instructions { get; set; }
        IEndianness Endianness { get; }

        void Reset();
    }
}