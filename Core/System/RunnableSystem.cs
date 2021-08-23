using bEmu.Core.CPU;
using bEmu.Core.Memory;

namespace bEmu.Core.System
{
    public abstract class RunnableSystem : BaseSystem, IRunnableSystem
    {
        public IRunner Runner { get; protected set; }
        public virtual int CycleCount => Runner?.Clock / 60 ?? 0;
        public int Cycles { get; protected set; }
        public IMMU MMU { get; protected set; }
        public IState State { get; protected set; }
        public abstract int StartAddress { get; }
        
        protected RunnableSystem(IROM rom) : base(rom)
        {
        }

        public abstract IState GetInitialState();
        
        public virtual void LoadProgram()
        {
            MMU.LoadProgram();
        }

        public virtual void ResetCycles()
        {
            Cycles = CycleCount;
        }

        public override void Reset()
        {
            State.Reset();
        }

        public override bool Update()
        {
            ResetCycles();
            return true;
        }

        public override void AttachDebugger()
        {
            if (Debugger == null)
                Debugger = new Debugger(this);
        }

    }
}