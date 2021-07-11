using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using bEmu.Core;
using bEmu.Systems.Gameboy.GPU;
using bEmu.Systems.Gameboy.MBCs;
using Debugger = System.Diagnostics.Debugger;

namespace bEmu.Systems.Gameboy
{
    public class MMU : bEmu.Core.MMU
    {
        public VRAM VRAM { get; private set; }
        public byte[] IO { get; private set; }
        public WRAM WRAM { get; private set; }
        public OAM OAM { get; private set; }
        public byte[] ZeroPage { get; private set; }
        public IMBC MBC { get; private set; }
        public BIOS Bios { get; private set; }
        public CartridgeHeader CartridgeHeader { get; private set; }
        public ColorPaletteData ColorPaletteData { get; private set; }
        public MonochromePaletteData MonochromePaletteData { get; private set; }
        public Joypad Joypad { get; set; }
        private Sound.APU APU => (System.APU as Gameboy.Sound.APU);

        public MMU(ISystem system) : base(system, 0x10000)
        {
            IO = new byte[128];
            VRAM = new VRAM(this);
            WRAM = new WRAM(this);
            OAM = new OAM(this);
            ZeroPage = new byte[128];
            MonochromePaletteData = new MonochromePaletteData(this);
            ColorPaletteData = new ColorPaletteData();
            Joypad = new Joypad();
        }

        public override byte this[int addr] 
        { 
            get
            {
                if (Bios.Running && (addr < 0x100 || (addr >= 0x200 && addr < Bios.InitAddress)))
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
                        return Joypad.GetJoypadInfo();

                    if (addr == 0xFF55)
                        return VRAM.HDMA5;

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
            IO[addr - 0xFF00] = value;

            if (addr == 0xFF00) // joypad
                Joypad.SetJoypadColumn(value);
            else if (addr == 0xFF10) // sweep envelope Channel1
                APU.StartSweepEnvelope();
            else if (addr == 0xFF11) // sound length Channel1
                APU.StartSound(Sound.GbSoundChannels.Channel1);
            else if (addr == 0xFF12) // volume envelope Channel1
                APU.StartVolumeEnvelope(Sound.GbSoundChannels.Channel1);
            else if (addr == 0xFF13 || addr == 0xFF14)
                APU.StartSound(Sound.GbSoundChannels.Channel1);
            else if (addr == 0xFF16) // sound length Channel2
                APU.StartSound(Sound.GbSoundChannels.Channel2);
            else if (addr == 0xFF17) // volume envelope Channel2
                APU.StartVolumeEnvelope(Sound.GbSoundChannels.Channel2);
            else if (addr == 0xFF18 || addr == 0xFF19)
                APU.StartSound(Sound.GbSoundChannels.Channel2);
            else if (addr == 0xFF1B) // sound length Channel3
                APU.StartSound(Sound.GbSoundChannels.Channel3);
            else if (addr == 0xFF20) // sound length Channel4
                APU.StartSound(Sound.GbSoundChannels.Channel4);
            else if (addr == 0xFF21) // volume envelope Channel4
                APU.StartVolumeEnvelope(Sound.GbSoundChannels.Channel4);
            else if (addr == 0xFF04) // DIV timer
                IO[addr - 0xFF00] = 0;
            else if (addr == 0xFF46) // OAM DMA
                OAM.StartDMATransfer(value);
            else if (addr == 0xFF55) // VRAM DMA
                VRAM.StartDMATransfer(value);
            else if (addr >= 0xFF68 && addr <= 0xFF6B) // paletas de cor
                ColorPaletteData[addr] = value;
        }

        public override void LoadProgram()
        {
            byte[] bytes = File.ReadAllBytes(System.FileName);
            CartridgeHeader = new CartridgeHeader(bytes);
            bool gbc = (CartridgeHeader.GBCFlag & 0x80) == 0x80;
            Bios = new BIOS(System, gbc);
            MBC = MBCFactory.GetMBC(this, CartridgeHeader.CartridgeType);
            MBC.LoadProgram(bytes);
        }
    }
}