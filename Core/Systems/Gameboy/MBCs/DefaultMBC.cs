using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.Gameboy.MBCs
{
    public abstract class DefaultMBC : IMBC
    {
        const int ROMBankSize = 16384;
        protected byte[][] romBanks;
        protected byte[][] ramBanks;
        protected byte[] rom0 => romBanks[0];
        protected abstract byte[] cartRAM { get; }
        protected abstract int ramBankCount { get; }
        protected abstract int externalRAMSize { get; }
        private readonly string fileName;
        private readonly bool battery;

        private string SaveName
        {
            get
            {
                string directory = Path.GetDirectoryName(fileName);
                string name = Path.GetFileNameWithoutExtension(fileName) + ".sav";
                return Path.Combine(directory, name);
            }
        }

        public DefaultMBC(string fileName, bool battery, bool ram)
        {
            this.fileName = fileName;
            this.battery = battery;
            InitializeMBC(ram);
        }

        private void InitializeMBC(bool ram)
        {
            if (ram)
            {
                InitializeRAMBanks();

                if (battery)
                {
                    byte[] ramBytes = GetOrCreateSaveFile();

                    for (int i = 0; i < ramBankCount; i++)
                        for (int j = 0; j < externalRAMSize; j++)
                            ramBanks[i][j] = ramBytes[j + (i * externalRAMSize)];
                }
            }
        }

        private byte[] GetOrCreateSaveFile()
        {
            if (File.Exists(SaveName))
                return File.ReadAllBytes(SaveName);
            
            var bytes = new byte[externalRAMSize * ramBankCount];
            File.WriteAllBytes(SaveName, bytes);
            return bytes;
        }

        public void LoadProgram(byte[] bytes)
        {
            romBanks = new byte[bytes.Length / ROMBankSize][];

            for (int i = 0; i < romBanks.Length; i++)
                romBanks[i] = new byte[ROMBankSize];

            for (int i = 0; i < bytes.Length; i++)
                romBanks[i / ROMBankSize][i % ROMBankSize] = bytes[i];
        }

        protected void InitializeRAMBanks()
        {
            ramBanks = new byte[ramBankCount][];

            for (int i = 0; i < ramBankCount; i++)
                ramBanks[i] = new byte[externalRAMSize];
        }

        public void Shutdown()
        {
            if (battery)
            {
                byte[] externalRAM = new byte[externalRAMSize * ramBankCount];

                for (int i = 0; i < ramBankCount; i++)
                    for (int j = 0; j < externalRAMSize; j++)
                        externalRAM[j + (i * externalRAMSize)] = ramBanks[i][j];

                File.WriteAllBytes(SaveName, externalRAM);
            }
        }

        public abstract byte ReadCartRAM(int addr);
        public abstract byte ReadROM(int addr);
        public abstract void SetMode(int addr, byte value);
        public abstract void WriteCartRAM(int addr, byte value);
    }
}