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

        public DefaultMBC(string fileName, bool battery)
        {
            this.fileName = fileName;
            this.battery = battery;
            if (battery)
            {
                byte[] ram = GetOrCreateSaveFile();

                for (int i = 0; i < ram.Length; i++)
                    WriteCartRAM(i, ram[i]);
            }
        }

        private byte[] GetOrCreateSaveFile()
        {
            if (File.Exists(SaveName))
                return File.ReadAllBytes(SaveName);
            
            File.Create(SaveName);
            return new byte[0];
        }

        public void LoadProgram(byte[] bytes)
        {
            romBanks = new byte[bytes.Length / ROMBankSize][];

            for (int i = 0; i < romBanks.Length; i++)
                romBanks[i] = new byte[ROMBankSize];

            for (int i = 0; i < bytes.Length; i++)
                romBanks[i / ROMBankSize][i % ROMBankSize] = bytes[i];
        }

        protected void InitializeRAMBanks(int banks, int bankSize)
        {
            ramBanks = new byte[banks][];

            for (int i = 0; i < banks; i++)
                ramBanks[i] = new byte[bankSize];
        }

        public void Shutdown()
        {
            if (battery)
                File.WriteAllBytes(SaveName, cartRAM);
        }

        public abstract byte ReadCartRAM(int addr);
        public abstract byte ReadROM(int addr);
        public abstract void SetMode(int addr, byte value);
        public abstract void WriteCartRAM(int addr, byte value);
    }
}