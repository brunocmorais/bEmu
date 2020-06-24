using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.Gameboy.MBCs
{
    public abstract class DefaultMBC : IMBC
    {
        private const int RomBankSize = 16384;
        protected byte[][] RomBanks;
        protected byte[][] RamBanks;
        protected byte[] Rom0 => RomBanks[0];
        protected abstract byte[] CartRam { get; }
        protected abstract int RamBankCount { get; }
        protected abstract int ExternalRamSize { get; }
        protected readonly string FileName;
        protected readonly bool Battery;
        protected string SaveName
        {
            get
            {
                string directory = Path.GetDirectoryName(FileName);
                string name = Path.GetFileNameWithoutExtension(FileName) + ".sav";
                return Path.Combine(directory, name);
            }
        }

        public DefaultMBC(string fileName, bool battery, bool ram)
        {
            FileName = fileName;
            Battery = battery;
            InitializeMBC(ram);
        }

        public abstract byte ReadCartRAM(int addr);
        public abstract byte ReadROM(int addr);
        public abstract void SetMode(int addr, byte value);
        public abstract void WriteCartRAM(int addr, byte value);

        private void InitializeMBC(bool ram)
        {
            if (ram)
            {
                InitializeRAMBanks();

                if (Battery)
                {
                    byte[] ramBytes = GetOrCreateSaveFile();

                    if (ramBytes.Length != RamBankCount * ExternalRamSize)
                        return;

                    for (int i = 0; i < RamBankCount; i++)
                        for (int j = 0; j < ExternalRamSize; j++)
                            RamBanks[i][j] = ramBytes[j + (i * ExternalRamSize)];
                }
            }
        }

        private byte[] GetOrCreateSaveFile()
        {
            if (File.Exists(SaveName))
                return File.ReadAllBytes(SaveName);
            
            var bytes = new byte[ExternalRamSize * RamBankCount];
            File.WriteAllBytes(SaveName, bytes);
            return bytes;
        }

        public virtual void LoadProgram(byte[] bytes)
        {
            RomBanks = new byte[bytes.Length / RomBankSize][];

            for (int i = 0; i < RomBanks.Length; i++)
                RomBanks[i] = new byte[RomBankSize];

            for (int i = 0; i < bytes.Length; i++)
                RomBanks[i / RomBankSize][i % RomBankSize] = bytes[i];
        }

        private void InitializeRAMBanks()
        {
            RamBanks = new byte[RamBankCount][];

            for (int i = 0; i < RamBankCount; i++)
                RamBanks[i] = new byte[ExternalRamSize];
        }

        public virtual void Shutdown()
        {
            if (Battery)
            {
                byte[] externalRAM = new byte[ExternalRamSize * RamBankCount];

                for (int i = 0; i < RamBankCount; i++)
                    for (int j = 0; j < ExternalRamSize; j++)
                        externalRAM[j + (i * ExternalRamSize)] = RamBanks[i][j];

                File.WriteAllBytes(SaveName, externalRAM);
            }
        }
    }
}