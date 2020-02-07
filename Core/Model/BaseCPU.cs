namespace bEmu.Core.Model
{
    public abstract class BaseCPU : ICPU
    {
        public abstract IState State { get; }

        public virtual IOpcode StepCycle()
        {
            State.Instructions++;
            return default(IOpcode);
        }
    }
}