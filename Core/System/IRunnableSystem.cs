using bEmu.Core.Memory;
using bEmu.Core.CPU;

namespace bEmu.Core.System
{
    public interface IRunnableSystem : ISystem
    {
        IRunner Runner { get; }
        int CycleCount { get; }
        int Cycles { get; }
        IState State { get; }
        IMMU MMU { get; }
        int StartAddress { get; }
        void LoadProgram();
        IState GetInitialState();
        void ResetCycles();
    }
}