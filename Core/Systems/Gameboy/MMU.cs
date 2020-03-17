using System;
using System.IO;

namespace bEmu.Core.Systems.Gameboy
{
    public class MMU : IMMU
    {
        byte[] rom0 = InitializeRAMPart(16384);
        byte[] romX = InitializeRAMPart(16384);
        byte[] vram = InitializeRAMPart(8192);
        byte[] cartRam = InitializeRAMPart(8192);
        byte[] wram = InitializeRAMPart(8192);
        byte[] oam = InitializeRAMPart(160);
        byte[] io = InitializeRAMPart(128);
        byte[] zp = InitializeRAMPart(128);

        public int Length => 0x10000;

        private static byte[] InitializeRAMPart(int size)
        {
            Random random = new Random();
            byte[] ramPart = new byte[size];

            // for (int i = 0; i < ramPart.Length; i++)
            //     ramPart[i] = (byte) random.Next(0x100);

            return ramPart;
        }

        public byte this[int addr] 
        { 
            get
            {
                if (addr >= 0x0000 && addr <= 0x3FFF)
                    return rom0[addr - 0x0000];
                if (addr >= 0x4000 && addr <= 0x7FFF)
                    return romX[addr - 0x4000];
                if (addr >= 0x8000 && addr <= 0x9FFF)
                    return vram[addr - 0x8000];
                if (addr >= 0xA000 && addr <= 0xBFFF)
                    return cartRam[addr - 0xA000];
                if (addr >= 0xC000 && addr <= 0xDFFF)
                    return wram[addr - 0xC000];
                if (addr >= 0xE000 && addr <= 0xFDFF)
                    return wram[addr - 0xE000];
                if (addr >= 0xFE00 && addr <= 0xFE9F)
                    return oam[addr - 0xFE00];
                if (addr >= 0xFF00 && addr <= 0xFF7F)
                    return io[addr - 0xFF00];
                if (addr >= 0xFF80 && addr <= 0xFFFF)
                    return zp[addr - 0xFF80];
                
                return 0;
            }
            set
            {
                if (addr >= 0x8000 && addr <= 0x9FFF)
                    vram[addr - 0x8000] = value;
                if (addr >= 0xA000 && addr <= 0xBFFF)
                    cartRam[addr - 0xA000] = value;
                if (addr >= 0xC000 && addr <= 0xDFFF)
                    wram[addr - 0xC000] = value;
                if (addr >= 0xE000 && addr <= 0xFDFF)
                    wram[addr - 0xE000] = value;
                if (addr >= 0xFE00 && addr <= 0xFE9F)
                    oam[addr - 0xFE00] = value;
                if (addr >= 0xFF00 && addr <= 0xFF7F)
                    io[addr - 0xFF00] = value;
                if (addr >= 0xFF80 && addr <= 0xFFFF)
                    zp[addr - 0xFF80] = value;
            }
        }

        public void LoadProgram(string fileName, int startAddress = 0)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            LoadProgram(bytes, startAddress);
        }

        public void LoadProgram(byte[] bytes, int startAddress = 0)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i < 16384)
                    rom0[i] = bytes[i];
                else
                    romX[i - 16384] = bytes[i];
            }
        }
    }
}