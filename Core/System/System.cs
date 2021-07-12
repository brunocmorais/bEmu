using System.IO;
using System.Linq;
using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;

namespace bEmu.Core.System
{
    public abstract class System : ISystem
    {
        protected string FileNameWithoutExtension => 
            Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName));
        public IDebugger Debugger { get; private set; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int RefreshRate { get; }
        public abstract int CycleCount { get; }
        public abstract int StartAddress { get; }
        public abstract IState GetInitialState();
        public abstract void Stop();
        public abstract void UpdateGamePad(IGamePad gamePad);
        public abstract IRunner Runner { get; }
        public abstract IState State { get; }
        public abstract IMMU MMU { get; }
        public abstract IPPU PPU { get; }
        public abstract IAPU APU { get; }
        public abstract SystemType Type { get; }
        public string FileName { get; }
        public int Cycles { get; protected set; }
        public string SaveFileName => FileNameWithoutExtension + ".sav";
        public string SaveStateName => FileNameWithoutExtension + ".state";
        
        public int Frame
        {
            get => PPU?.Frame ?? 0;
            private set
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

        public void LoadProgram()
        {
            MMU.LoadProgram();
            Frame = 0;
        }

        public void AttachDebugger()
        {
            if (Debugger == null)
                Debugger = new Debugger(this);
        }
    }
}