namespace bEmu.Core
{
    public abstract class System : ISystem
    {
        public IRunner Runner { get; protected set; }
        public IState State { get; protected set; }
        public IMMU MMU { get; protected set; }
        public IPPU PPU { get; protected set; }

        public System()
        {
            Initialize();
        }

        public abstract IState GetInitialState();
        public virtual void Initialize()
        {
            State = GetInitialState();
        }
    }
}