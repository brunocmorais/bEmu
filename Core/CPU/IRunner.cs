using bEmu.Core.System;

namespace bEmu.Core.CPU
{
    public interface IRunner
    {
        ISystem System { get; }
        IOpcode StepCycle();
    }
}