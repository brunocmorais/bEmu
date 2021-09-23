using bEmu.Core.System;

namespace bEmu.Core.CPU
{
    public interface IRunner
    {
        IRunnableSystem System { get; }
        IOpcode StepCycle();
        int Clock { get; }
        IEndianness Endianness { get; }
    }
}