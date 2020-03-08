namespace bEmu.Core
{
    public interface IExecutor
    {
        ISystem System { get; }
        IOpcode StepCycle();
    }
}