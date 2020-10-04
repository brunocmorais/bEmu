using System.IO;
using System.Linq;

namespace bEmu.Core
{
    public abstract class System : ISystem
    {
        public IRunner Runner { get; protected set; }
        public IState State { get; protected set; }
        public IMMU MMU { get; protected set; }
        public IPPU PPU { get; protected set; }
        public IAPU APU { get; protected set; }
        public string FileName { get; }
        public string SaveFileName => FileNameWithoutExtension + ".sav";
        public string SaveStateName => FileNameWithoutExtension + ".state";
        private string FileNameWithoutExtension => Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName));

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

        public abstract IState GetInitialState();
    }
}