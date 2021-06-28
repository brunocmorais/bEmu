using System.IO;
using System.Linq;

namespace bEmu.Core
{
    public abstract class System : ISystem
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int RefreshRate { get; }
        public abstract int CycleCount { get; }
        public abstract int StartAddress { get; }
        public abstract IState GetInitialState();
        public abstract void Stop();
        public abstract void UpdateGamePad(IGamePad gamePad);

        public IRunner Runner { get; protected set; }
        public IState State { get; protected set; }
        public IMMU MMU { get; protected set; }
        public IPPU PPU { get; protected set; }
        public IAPU APU { get; protected set; }
        public IDebugger Debugger { get; set; }
        public string FileName { get; set; }
        public string SaveFileName => FileNameWithoutExtension + ".sav";
        public string SaveStateName => FileNameWithoutExtension + ".state";
        private string FileNameWithoutExtension => Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName));
        public int Cycles { get; protected set; }
        public int Frame
        {
            get => PPU?.Frame ?? 0;
            set
            {
                if (PPU != null)
                    PPU.Frame = value;
            }  
        }
        public int Frameskip
        {
            get => PPU?.Frameskip ?? 0;
            set
            {
                if (PPU != null)
                    PPU.Frameskip = value;
            }
        }

        public Framebuffer Framebuffer => PPU?.Framebuffer ?? new Framebuffer(Width, Height);

        public byte[] SoundBuffer
        {
            get
            {
                if (APU != null)
                {
                    APU.UpdateBuffer();
                    return APU.Buffer;
                }

                return new byte[0];
            }
        }

        public System(string fileName)
        {
            FileName = fileName;
            Initialize();
        }

        public virtual void Initialize()
        {
            State = GetInitialState();
        }

        public virtual void Reset()
        {
            State.Reset();
            PPU.Reset();
        }

        public virtual bool LoadState()
        {
            if (!File.Exists(SaveStateName))
                return false;

            byte[] state = File.ReadAllBytes(SaveStateName);
            State.LoadState(state);
            MMU.LoadState(state);

            return true;
        }

        public virtual void SaveState()
        {
            byte[] state = State.SaveState();
            byte[] mmu = MMU.SaveState();
            File.WriteAllBytes(SaveStateName, Enumerable.Concat(state, mmu).ToArray());
        }

        public virtual bool Update()
        {
            return !(Debugger != null && (Debugger.IsStopped || (Debugger.BreakpointAddress > 0 && Debugger.BreakpointAddress == State.PC)));
        }

        public void ResetCycles()
        {
            Cycles = CycleCount;
        }

        public virtual void LoadProgram()
        {
            MMU.LoadProgram();
        }
    }
}