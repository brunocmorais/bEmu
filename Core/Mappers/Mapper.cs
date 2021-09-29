using System.Collections.Generic;
using System.IO;
using bEmu.Core;
using bEmu.Core.Memory;

namespace bEmu.Core.Mappers
{
    public abstract class Mapper : IMapper
    {
        private const int RomBankSize = 16384;
        protected byte[][] RomBanks;
        protected byte[][] RamBanks;
        protected byte[] Rom0 => RomBanks[0];
        protected abstract byte[] CartRam { get; }
        protected abstract int RamBankCount { get; }
        protected abstract int ExternalRamSize { get; }
        protected IMMU MMU { get; }
        protected readonly bool Battery;

        public Mapper(IMMU mmu, bool battery, bool ram)
        {
            MMU = mmu;
            Battery = battery;
            InitializeMBC(ram);
        }

        public abstract byte ReadROM(int addr);

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
            if (File.Exists(MMU.System.ROM.SaveFileName))
                return File.ReadAllBytes(MMU.System.ROM.SaveFileName);
            
            var bytes = new byte[ExternalRamSize * RamBankCount];
            File.WriteAllBytes(MMU.System.ROM.SaveFileName, bytes);
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

                File.WriteAllBytes(MMU.System.ROM.SaveFileName, externalRAM);
            }
        }
    }
}