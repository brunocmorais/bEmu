namespace bEmu.Core.Model
{
    public interface ICPU
    {
        IState State { get; }
        IOpcode StepCycle();
    }
}