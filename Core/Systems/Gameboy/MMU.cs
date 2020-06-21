using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using bEmu.Core.Systems.Gameboy.GPU;
using bEmu.Core.Systems.Gameboy.MBCs;

namespace bEmu.Core.Systems.Gameboy
{
    public class MMU : IMMU
    {
        public State State { get; }
        public VRAM VRAM { get; }
        public byte[] IO { get; }
        public byte[] WRAM { get; }
        public OAM OAM { get; }
        public byte[] ZeroPage { get; }
        public IMBC MBC { get; private set; }
        public BIOS Bios { get; }
        public int Length => 0x10000;
        public CartridgeHeader CartridgeHeader { get; private set; }
        public ColorPaletteData ColorPaletteData { get; }

        public MMU(State state)
        {
            State = state;
            Bios = new BIOS();

            VRAM = new VRAM(this);
            IO = new byte[128];
            WRAM = new byte[8192];
            OAM = new OAM(this);
            ZeroPage = new byte[128];
            ColorPaletteData = new ColorPaletteData();
        }

        public byte this[int addr] 
        { 
            get
            {
                if (Bios.Running && addr < 0x100)
                    return Bios[addr];
                else if (addr >= 0x0000 && addr <= 0x7FFF)
                    return MBC.ReadROM(addr);
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                    return VRAM[addr - 0x8000];
                else if (addr >= 0xA000 && addr <= 0xBFFF)
                    return MBC.ReadCartRAM((addr - 0xA000));
                else if (addr >= 0xC000 && addr <= 0xDFFF)
                    return WRAM[addr - 0xC000];
                else if (addr >= 0xE000 && addr <= 0xFDFF)
                    return WRAM[addr - 0xE000];
                else if (addr >= 0xFE00 && addr <= 0xFE9F)
                    return OAM[addr - 0xFE00];
                else if (addr >= 0xFF00 && addr <= 0xFF7F)
                {
                    if (addr == 0xFF00) // joypad
                        return State.Joypad.GetJoypadInfo();

                    if (addr >= 0xFF68 && addr <= 0xFF6B) // paletas de cor
                        return ColorPaletteData[addr];

                    return IO[addr - 0xFF00];
                }
                else if (addr >= 0xFF80 && addr <= 0xFFFF)
                    return ZeroPage[addr - 0xFF80];
                
                return 0xFF;
            }
            set
            {
                if (addr >= 0x0000 && addr <= 0x7FFF)
                    MBC.SetMode(addr, value);
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                    VRAM[addr - 0x8000] = value;
                else if (addr >= 0xA000 && addr <= 0xBFFF)
                    MBC.WriteCartRAM((addr - 0xA000), value);
                else if (addr >= 0xC000 && addr <= 0xDFFF)
                    WRAM[addr - 0xC000] = value;
                else if (addr >= 0xE000 && addr <= 0xFDFF)
                    WRAM[addr - 0xE000] = value;
                else if (addr >= 0xFE00 && addr <= 0xFE9F)
                    OAM[addr - 0xFE00] = value;
                else if (addr >= 0xFF00 && addr <= 0xFF7F)
                    SetRegister(addr, value);
                else if (addr >= 0xFF80 && addr <= 0xFFFF)
                    ZeroPage[addr - 0xFF80] = value;
            }
        }

        public void SetRegister(int addr, byte value)
        {
            if (addr == 0xFF00) // joypad
                State.Joypad.SetJoypadColumn(value);
            else if (addr == 0xFF04) // DIV timer
                IO[addr - 0xFF00] = 0;
            else if (addr == 0xFF44) // registrador LY 
                return;
            else if (addr == 0xFF46) // OAM DMA
                OAM.StartDMATransfer(value);
            else if (addr == 0xFF55) // VRAM DMA
                VRAM.StartDMATransfer(value);
            else if (addr >= 0xFF68 && addr <= 0xFF6B) // paletas de cor
                ColorPaletteData[addr] = value;
            else
                IO[addr - 0xFF00] = value;
        }

        public void LoadProgram(string fileName, int startAddress = 0)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            CartridgeHeader = new CartridgeHeader(bytes);
            MBC = MBCFactory.GetMBC(fileName, CartridgeHeader.CartridgeType);
            MBC.LoadProgram(bytes);
        }

        public void LoadProgram(byte[] bytes, int startAddress = 0)
        {
            throw new NotImplementedException();
        }
    }
}