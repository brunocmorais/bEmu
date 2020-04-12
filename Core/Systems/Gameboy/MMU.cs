using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace bEmu.Core.Systems.Gameboy
{
    public class MMU : IMMU
    {
        byte[] vram = InitializeRAMPart(8192);
        byte[] wram = InitializeRAMPart(8192);
        byte[] oam = InitializeRAMPart(160);
        byte[] io = InitializeRAMPart(128);
        byte[] zp = InitializeRAMPart(128);
        State state => (System.State as bEmu.Core.Systems.Gameboy.State);
        IMBC mbc;
        public BIOS Bios { get; }

        public int Length => 0x10000;
        public ISystem System { get; }
        public CartridgeHeader CartridgeHeader { get; private set; }

        public MMU(ISystem system)
        {
            System = system;
            Bios = new BIOS();
        }

        private static byte[] InitializeRAMPart(int size)
        {
            Random random = new Random();
            byte[] ramPart = new byte[size];

            return ramPart;
        }

        public byte this[int addr] 
        { 
            get
            {
                if (Bios.Running && addr < 0x100)
                    return Bios[(byte) addr];
                if (addr >= 0x0000 && addr <= 0x7FFF)
                    return mbc.ReadROM((ushort) addr);
                if (addr >= 0x8000 && addr <= 0x9FFF)
                    return vram[addr - 0x8000];
                if (addr >= 0xA000 && addr <= 0xBFFF)
                    return mbc.ReadCartRAM((ushort) (addr - 0xA000));
                if (addr >= 0xC000 && addr <= 0xDFFF)
                    return wram[addr - 0xC000];
                if (addr >= 0xE000 && addr <= 0xFDFF)
                    return wram[addr - 0xE000];
                if (addr >= 0xFE00 && addr <= 0xFE9F)
                    return oam[addr - 0xFE00];
                if (addr >= 0xFF00 && addr <= 0xFF7F)
                {
                    if (addr == 0xFF00) // joypad
                        return state.Joypad.GetJoypadInfo();

                    return io[addr - 0xFF00];
                }
                if (addr >= 0xFF80 && addr <= 0xFFFF)
                    return zp[addr - 0xFF80];
                
                return 0;
            }
            set
            {
                if (addr >= 0x0000 && addr <= 0x7FFF)
                    mbc.SetMode((ushort) addr, value);
                if (addr >= 0x8000 && addr <= 0x9FFF)
                    vram[addr - 0x8000] = value;
                if (addr >= 0xA000 && addr <= 0xBFFF)
                    mbc.WriteCartRAM((ushort) (addr - 0xA000), value);
                if (addr >= 0xC000 && addr <= 0xDFFF)
                    wram[addr - 0xC000] = value;
                if (addr >= 0xE000 && addr <= 0xFDFF)
                    wram[addr - 0xE000] = value;
                if (addr >= 0xFE00 && addr <= 0xFE9F)
                    oam[addr - 0xFE00] = value;
                if (addr >= 0xFF00 && addr <= 0xFF7F)
                {
                    if (addr == 0xFF00) // joypad
                        state.Joypad.SetJoypadColumn(value);
                    else if (addr == 0xFF04) // DIV timer
                        io[addr - 0xFF00] = 0;
                    else if (addr == 0xFF46) // OAM DMA
                    {
                        ushort oamStartAddress = (ushort) (value << 8);

                        for (int i = 0; i < 0x9F; i++)
                            oam[i] = this[oamStartAddress + i];
                    }
                    else
                        io[addr - 0xFF00] = value;
                }
                if (addr >= 0xFF80 && addr <= 0xFFFF)
                    zp[addr - 0xFF80] = value;
            }
        }

        public void InitTimer()
        {
            io[4] = 171;
        }

        // public void UpdateLY(int value)
        // {
        //     io[0x44] = (byte) value;
        // }

        public void LoadProgram(string fileName, int startAddress = 0)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            LoadProgram(bytes, startAddress);
        }

        public void LoadProgram(byte[] bytes, int startAddress = 0)
        {
            CartridgeHeader = new CartridgeHeader(bytes);
            mbc = MBCFactory.GetMBC(CartridgeHeader.CartridgeType);
            mbc.LoadProgram(bytes);
        }
    }
}