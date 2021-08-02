using System.IO;
using System.Linq;
using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;

namespace bEmu.Core.System
{
    public abstract class System : ISystem
    {
        private readonly byte[] dummySoundBuffer = new byte[Audio.APU.BufferSize];
        public string FileNameWithoutExtension => 
            Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName));
        public string FilePath => Path.GetDirectoryName(FileName);
        public IDebugger Debugger { get; private set; }
        public abstract int Width { get; }
        public abstract int Height { get; }
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
        public bool SkipFrame => (Frameskip >= 1 && Frame % (Frameskip + 1) != 0);
        public virtual int CycleCount => Runner?.Clock / 60 ?? 0;
        
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

        public IFrameBuffer Framebuffer => PPU?.Framebuffer ?? new FrameBuffer(Width, Height);

        public byte[] SoundBuffer
        {
            get
            {
                if (APU != null)
                {
                    APU.UpdateBuffer();
                    return APU.Buffer;
                }

                return dummySoundBuffer;
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
            ResetCycles();

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