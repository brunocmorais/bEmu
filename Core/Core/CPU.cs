namespace bEmu.Core
{
    public abstract class CPU<TState> : ICPU<TState> where TState : class, IState
    {
        public ISystem System { get; set; }
        public TState State => (System.State as TState);
        public MMU MMU => (System.MMU as MMU);

        public CPU(ISystem system)
        {
            System = system;
        }

        public virtual IOpcode StepCycle()
        {
            State.Instructions++;
            return default(IOpcode);
        }

        public void IncreaseCycles(sbyte cycles)
        {
            State.Cycles += cycles;
        }
    }
}