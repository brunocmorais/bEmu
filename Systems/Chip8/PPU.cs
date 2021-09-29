using System;
using bEmu.Core;
using bEmu.Core.CPU;
using bEmu.Core.System;
using bEmu.Core.Util;
using bEmu.Core.Video;

namespace bEmu.Systems.Chip8
{
    public class PPU : Core.Video.PPU
    {
        private State state;
        public PPU(State state, int width, int height) : base(state.System as IVideoGameSystem, width, height) 
        { 
            this.state = state;
        }

        private void DrawNextTime()
        {
            state.Draw = true;
            Frame++;
        }

        new int Width => ((State) state).SuperChipMode ? base.Width : base.Width / 2;
        new int Height => ((State) state).SuperChipMode ? base.Height : base.Height / 2;

        public void ScrollRight()
        {
            for (int i = Width - 1; i >= 0; i--)
            {
                for (int j = Height - 1; j >= 0; j--)
                {
                    if ((i - 4) < 0)
                        this[i, j] = (0x000000FF);
                    else
                        this[i, j] = this[i - 4, j];
                }
            }

            DrawNextTime();
        }

        public void ScrollLeft()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if ((i + 4) > Width)
                        this[i, j] = (0x000000FF);
                    else
                        this[i, j] = this[i + 4, j];
                }   
            }

            DrawNextTime();
        }

        public void ScrollDown(byte nibble)
        {
            for (int i = Width - 1; i >= 0; i--)
            {
                for (int j = Height - 1; j >= 0; j--)
                {
                    if ((j - nibble) < 0)
                        this[i, j] = (0x000000FF);
                    else
                        this[i, j] = this[i, j - nibble];
                }   
            }

            DrawNextTime();
        }

        public void ScrollUp(byte nibble)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if ((j + nibble) > Width)
                        this[i, j] = (0x000000FF);
                    else
                        this[i, j] = this[i, j + nibble];
                }   
            }

            DrawNextTime();
        }

        public void Drw(byte x, byte y, byte n)
        {
            if (n == 0)
            {
                DrwSuperChip(x, y);
                return;
            }
            
            byte[] sprite = new byte[n];

            for (int i = 0; i < n; i++)
                sprite[i] = System.MMU[state.I + i];

            byte coordX = state.V[x];
            byte coordY = state.V[y];
            bool collision = false;

            for (int i = 0; i < n; i++)
            {
                byte originalSprite = 0;

                for (int j = 0; j < 8; j++)
                {
                    bool pixel = this[(coordX + j) % Width, coordY] == 0xFFFFFFFF;
                    originalSprite |= (byte) ((pixel ? 1 : 0) << (7 - j));
                }
                
                byte resultSprite = (byte) (originalSprite ^ sprite[i]);

                if ((originalSprite & resultSprite) != originalSprite)
                    collision = true;

                for (int j = 7; j >= 0; j--)
                {
                    bool pixel = ((resultSprite & (0x1 << j)) >> j) == 1; 
                    this[(byte) ((coordX + (7 - j)) % Width), coordY] = (pixel ? 0xFFFFFFFF : 0x000000FF);
                }

                coordY++;
                coordY %= (byte) Height;
            }

            state.V[0xF] = (byte) (collision ? 1 : 0);
            DrawNextTime();
        }

        private void DrwSuperChip(byte x, byte y)
        {
            ushort[] sprite = new ushort[16];

            for (int i = 0; i < 16; i++)
                sprite[i] = BitFacade.GetWordFrom2Bytes(System.MMU[state.I + (2 * i) + 1], System.MMU[state.I + (2 * i)]);

            byte coordX = state.V[x];
            byte coordY = state.V[y];
            bool collision = false;

            for (int i = 0; i < 16; i++)
            {
                ushort originalSprite = 0;

                for (int j = 0; j < 16; j++)
                {
                    bool pixel = this[(coordX + j) % Width, coordY] == (0xFFFFFFFF);
                    originalSprite |= (ushort) ((pixel ? 1 : 0) << (15 - j));
                }
                
                ushort resultSprite = (ushort) (originalSprite ^ sprite[i]);

                if ((originalSprite & resultSprite) != originalSprite)
                    collision = true;

                for (int j = 15; j >= 0; j--)
                {
                    bool pixel = ((resultSprite & (0x1 << j)) >> j) == 1; 
                    this[(byte) ((coordX + (15 - j)) % Width), coordY] = (pixel ? 0xFFFFFFFF : 0x000000FF);
                }

                coordY++;
                coordY %= (byte) Height;
            }

            state.V[0xF] = (byte) (collision ? 1 : 0);
            DrawNextTime();
        }

        public void ClearScreen()
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    this[i, j] = (0x000000FF);

            DrawNextTime();
        }

        private Pixel this[int x, int y]
        {
            get
            {
                if (((State) state).SuperChipMode)
                    return Framebuffer[x, y];
                else
                    return Framebuffer[(x * 2) + 1, (y * 2) + 1];
            }
            set
            {
                if (((State) state).SuperChipMode)
                    Framebuffer[x, y] = value;
                else
                    Framebuffer.SetScaledPixel(new ScaledPixel(2, value), x, y);
            }
        }

        public override void StepCycle() { }
    }
}