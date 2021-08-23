using System.IO;
using System.Linq;
using bEmu.Core.GamePad;
using bEmu.Core.Video;

namespace bEmu.Core.System
{
    public abstract class VideoGameSystem : AudioSystem, IVideoGameSystem
    {
        public IPPU PPU { get; protected set; }
        public virtual int Width { get; }
        public virtual int Height { get; }
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
        public bool SkipFrame => (Frameskip >= 1 && Frame % (Frameskip + 1) != 0);
        public string SaveStateName => FileNameWithoutExtension + ".state";

        public abstract void UpdateGamePad(IGamePad gamePad);

        protected VideoGameSystem(string fileName) : base(fileName)
        {
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
    }
}