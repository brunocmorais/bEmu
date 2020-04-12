using System;
using System.Collections.Generic;
using System.Diagnostics;
using bEmu.Core;
using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy
{
    public class GPU : Core.PPU
    {
        public int Cycles { get; set; } = 0;
        GPUMode Mode = GPUMode.HBlank;
        //Color[,] frameBuffer = new Color[160, 144];
        State State => System.State as bEmu.Core.Systems.Gameboy.State;
        MMU MMU => System.MMU as bEmu.Core.Systems.Gameboy.MMU;
        IEnumerable<Sprite> spritesCurrentLine = new List<Sprite>();
        OAM oam;
        bool skipFrame => Frameskip > 0 && Frame % Frameskip != 0;
        public int Frame { get; private set; } = 0;
        public int Frameskip { get; set; } = 0;

        public GPU(ISystem system, int width, int height) : base(system, width, height) 
        { 
            oam = new OAM(system);
        }

        private uint GetColorFrom(PaletteType type, int colorNumber)
        {
            byte val = 0;

            switch (type)
            {
                case PaletteType.BGP: val  = State.LCD.BGP;  break;
                case PaletteType.OBP0: val = State.LCD.OBP0; break;
                case PaletteType.OPB1: val = State.LCD.OBP1; break;
            }

            int bitOffset = (colorNumber * 2);
            int shadeNumber = ((val & (3 << bitOffset)) >> bitOffset);
            byte color = 0;

            switch (shadeNumber)
            {
                case 0: color = 0xFF; break;
                case 1: color = 0xC0; break;
                case 2: color = 0x60; break;
                case 3: color = 0x00; break;
            }

            return (uint) ((color << 24) | (color << 16) | (color << 8) | 0xFF);
        }

        private uint[] GetPaletteFromBytes(PaletteType paletteType, byte byte1, byte byte2)
        {
            var palette = new uint[8];

            for (int i = 0; i < 8; i++)
            {
                int colorNumber = 0;
                int v = (7 - i);

                if (((byte2 & (1 << v)) >> v) == 1)
                    colorNumber += 2;

                if (((byte1 & (1 << v)) >> v) == 1)
                    colorNumber += 1;

                if (paletteType == PaletteType.BGP || colorNumber > 0)
                    palette[i] = GetColorFrom(paletteType, colorNumber);
                else
                    palette[i] = 0;
            }

            return palette;
        }

        public void StepCycle()
        {
            if (!State.LCD.GetLCDCFlag(LCDC.LCDDisplayEnable))
                return;

            State.LCD.SetSTATMode((int) Mode);
            bool lycCoincidence = State.LCD.LY == State.LCD.LYC;
            State.LCD.SetSTATFlag(STAT.CoincidenceFlag, lycCoincidence);

            if (lycCoincidence && State.LCD.GetSTATFlag(STAT.LYCoincidenceInterrupt) == 1)
                State.RequestInterrupt(InterruptType.LcdStat);

            switch (Mode)
            {
                case GPUMode.HBlank:
                    if (Cycles >= 204)
                    {
                        if (!skipFrame)
                            Renderscan();

                        State.LCD.LY++;
                        Cycles -= 204;

                        if (State.LCD.LY == 144)
                            Mode = GPUMode.VBlank;
                        else
                            Mode = GPUMode.ScanlineOAM;
                    }

                    break;
                case GPUMode.VBlank:

                    if (Cycles >= 456)
                    {
                        if (State.LCD.LY == 144)
                            State.RequestInterrupt(InterruptType.VBlank);

                        Cycles -= 456;
                        State.LCD.LY++;

                        if (State.LCD.LY > 153)
                        {
                            State.LCD.LY = 0;
                            Mode = GPUMode.ScanlineOAM;

                            if (!skipFrame)
                                oam.UpdateSprites();

                            Frame++;
                        }
                    }
                    break;
                case GPUMode.ScanlineOAM:
                    if (Cycles >= 80)
                    {
                        Mode = GPUMode.ScanlineVRAM;

                        if (!skipFrame)
                            spritesCurrentLine = oam.GetSpritesOnLine(State.LCD.LY, State.LCD.GetLCDCFlag(LCDC.SpriteSize) ? 16 : 8);

                        Cycles -= 80;
                    }
                    break;
                case GPUMode.ScanlineVRAM:
                    if (Cycles >= 172)
                    {
                        Mode = GPUMode.HBlank;
                        Cycles -= 172;
                    }

                    break;
            }
        }

        private void Renderscan()
        {
            if (State.LCD.LY < 0 && State.LCD.LY > 143)
                return;

            bool bgDisplay = State.LCD.GetLCDCFlag(LCDC.BGDisplay);
            bool spriteDisplay = State.LCD.GetLCDCFlag(LCDC.SpriteDisplayEnable);
            int tileStartAddress = State.LCD.GetLCDCFlag(LCDC.BGWindowTileDataSelect) ? 0x8000 : 0x8800;
            int mapSelect = State.LCD.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) ? 0x9C00 : 0x9800;

            byte line = (byte) ((State.LCD.LY + State.LCD.SCY));

            if (bgDisplay)
                for (int i = 0; i < 20; i++) // 20 tiles 8x8
                    RenderBackgroundScanline(tileStartAddress, mapSelect, i, line);
            
            if (spriteDisplay)
                RenderOAMScanline();
        }

        private void RenderOAMScanline()
        {
            foreach (var sprite in spritesCurrentLine)
            {
                int paletteAddr = 0x8000 + (sprite.Address << 4) + (2 * ((sprite.LineOffset) % sprite.Size));
                var palette = GetPaletteFromBytes(sprite.PaletteType, System.MMU[paletteAddr], System.MMU[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    if (palette[j] == 0) // transparente
                        continue;

                    int coordX;

                    if (sprite.XFlip)
                        coordX = ((palette.Length - j) + sprite.X) - 1;
                    else
                        coordX = j + sprite.X;

                    if (coordX >= 0 && coordX < 160 && !sprite.Priority || GetPixel(coordX, State.LCD.LY) == 0xFFFFFFFF)
                        SetPixel(coordX, State.LCD.LY, palette[j]);
                }
            }
        }

        private void RenderBackgroundScanline(int tileStartAddress, int mapSelect, int i, byte line)
        {
            int addr = mapSelect + ((i + (State.LCD.SCX / 8)) % 32) + (line / 8 * 32);
            byte tileNumber = System.MMU[addr];
            int paletteAddr;

            if (tileStartAddress == 0x8000)
                paletteAddr = tileStartAddress + (tileNumber << 4) + (2 * (line % 8));
            else
                paletteAddr = ((tileNumber & 0x80) == 0x80 ? 0x8800 : 0x9000) + ((tileNumber & 0x7F) << 4) + (2 * (line % 8));

            var palette = GetPaletteFromBytes(PaletteType.BGP, System.MMU[paletteAddr], System.MMU[paletteAddr + 1]);

            for (int j = 0; j < palette.Length; j++)
            {
                int x = (j + (i * 8)) - (State.LCD.SCX % 8);

                if (x >= 0 && x < 160)
                    SetPixel(x, State.LCD.LY, palette[j]);
            }
        }
    }

    public enum GPUMode
    {
        HBlank = 0,
        VBlank = 1,
        ScanlineOAM = 2,
        ScanlineVRAM = 3
    }

    public enum PaletteType
    {
        BGP = 0,
        OBP0 = 1,
        OPB1 = 2
    }
}