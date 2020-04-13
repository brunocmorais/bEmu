using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace bEmu.Core.Systems.Gameboy
{
    public class MMU : IMMU
    {
        public byte[] VRAM { get; } = InitializeRAMPart(8192);
        public byte[] IO { get; } = InitializeRAMPart(128);
        byte[] wram = InitializeRAMPart(8192);
        public byte[] OAM { get; } = InitializeRAMPart(160);
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
            return new byte[size];
        }

        public byte this[int addr] 
        { 
            get
            {
                if (Bios.Running && addr < 0x100)
                    return Bios[addr];
                else if (addr >= 0x0000 && addr <= 0x7FFF)
                    return mbc.ReadROM(addr);
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                    return VRAM[addr - 0x8000];
                else if (addr >= 0xA000 && addr <= 0xBFFF)
                    return mbc.ReadCartRAM((addr - 0xA000));
                else if (addr >= 0xC000 && addr <= 0xDFFF)
                    return wram[addr - 0xC000];
                else if (addr >= 0xE000 && addr <= 0xFDFF)
                    return wram[addr - 0xE000];
                else if (addr >= 0xFE00 && addr <= 0xFE9F)
                    return OAM[addr - 0xFE00];
                else if (addr >= 0xFF00 && addr <= 0xFF7F)
                {
                    if (addr == 0xFF00) // joypad
                        return state.Joypad.GetJoypadInfo();

                    return IO[addr - 0xFF00];
                }
                else if (addr >= 0xFF80 && addr <= 0xFFFF)
                    return zp[addr - 0xFF80];
                
                return 0;
            }
            set
            {
                if (addr >= 0x0000 && addr <= 0x7FFF)
                    mbc.SetMode(addr, value);
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                    VRAM[addr - 0x8000] = value;
                else if (addr >= 0xA000 && addr <= 0xBFFF)
                    mbc.WriteCartRAM((addr - 0xA000), value);
                else if (addr >= 0xC000 && addr <= 0xDFFF)
                    wram[addr - 0xC000] = value;
                else if (addr >= 0xE000 && addr <= 0xFDFF)
                    wram[addr - 0xE000] = value;
                else if (addr >= 0xFE00 && addr <= 0xFE9F)
                    OAM[addr - 0xFE00] = value;
                else if (addr >= 0xFF00 && addr <= 0xFF7F)
                {
                    if (addr == 0xFF00) // joypad
                        state.Joypad.SetJoypadColumn(value);
                    else if (addr == 0xFF04) // DIV timer
                        IO[addr - 0xFF00] = 0;
                    else if (addr == 0xFF44) // registrador LY 
                        return;
                    else if (addr == 0xFF46) // OAM DMA
                    {
                        var oamStartAddress = (value << 8);

                        for (int i = 0; i < 0x9F; i++)
                            OAM[i] = this[oamStartAddress + i];
                    }
                    else
                        IO[addr - 0xFF00] = value;
                }
                else if (addr >= 0xFF80 && addr <= 0xFFFF)
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
            CartridgeHeader = new CartridgeHeader(bytes);
            mbc = MBCFactory.GetMBC(CartridgeHeader.CartridgeType);
            mbc.LoadProgram(bytes);
        }
    }
}