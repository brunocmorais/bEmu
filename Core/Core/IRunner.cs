namespace bEmu.Core
{
    public interface IRunner
    {
        ISystem System { get; }
        IOpcode StepCycle();
    }
}