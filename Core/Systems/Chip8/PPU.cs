using System;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Core.Systems.Chip8
{
    public class PPU : Core.PPU
    {
        private readonly Pixel[,] gfx;

        public PPU(System system, int width, int height) : base(system, width, height) 
        { 
            gfx = new Pixel[width, height];
        }

        public State State => System.State as State;

        void DrawNextTime()
        {
            State.Draw = true;
        }

        bool IsPixelOn(Pixel pixel)
        {
            return (pixel.R + pixel.G + pixel.B == 765);
        }

        public override Pixel this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x > Width || y > Height)
                    throw new Exception("Tentativa de obter coordenada de vídeo fora do intervalo válido.");

                return gfx[x, y];
            }
            set
            {
                if (x < 0 || y < 0 || x > Width || y > Height)
                    throw new Exception("Tentativa de setar coordenada de vídeo fora do intervalo válido.");

                gfx[x, y] = value;
            }
        }

        public void ScrollRight()
        {
            for (int i = Width - 1; i >= 0; i--)
            {
                for (int j = Height - 1; j >= 0; j--)
                {
                    if ((i - 4) < 0)
                        this[i, j] = Pixel.Off;
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
                        this[i, j] = Pixel.Off;
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
                        this[i, j] = Pixel.Off;
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
                        this[i, j] = Pixel.Off;
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
                sprite[i] = System.MMU[State.I + i];

            byte coordX = State.V[x];
            byte coordY = State.V[y];
            bool collision = false;

            for (int i = 0; i < n; i++)
            {
                byte originalSprite = 0;

                for (int j = 0; j < 8; j++)
                {
                    bool pixel = IsPixelOn(this[(coordX + j) % Width, coordY]);
                    originalSprite |= (byte) ((pixel ? 1 : 0) << (7 - j));
                }
                
                byte resultSprite = (byte) (originalSprite ^ sprite[i]);

                if ((originalSprite & resultSprite) != originalSprite)
                    collision = true;

                for (int j = 7; j >= 0; j--)
                {
                    bool pixel = ((resultSprite & (0x1 << j)) >> j) == 1; 
                    this[(byte) ((coordX + (7 - j)) % Width), coordY] = pixel ? Pixel.On : Pixel.Off;
                }

                coordY++;
                coordY %= (byte) Height;
            }

            State.V[0xF] = (byte) (collision ? 1 : 0);
            DrawNextTime();
        }

        private void DrwSuperChip(byte x, byte y)
        {
            ushort[] sprite = new ushort[16];

            for (int i = 0; i < 16; i++)
                sprite[i] = BitUtils.GetWordFrom2Bytes(System.MMU[State.I + (2 * i) + 1], System.MMU[State.I + (2 * i)]);

            byte coordX = State.V[x];
            byte coordY = State.V[y];
            bool collision = false;

            for (int i = 0; i < 16; i++)
            {
                ushort originalSprite = 0;

                for (int j = 0; j < 16; j++)
                {
                    bool pixel = IsPixelOn(this[(coordX + j) % Width, coordY]);
                    originalSprite |= (ushort) ((pixel ? 1 : 0) << (15 - j));
                }
                
                ushort resultSprite = (ushort) (originalSprite ^ sprite[i]);

                if ((originalSprite & resultSprite) != originalSprite)
                    collision = true;

                for (int j = 15; j >= 0; j--)
                {
                    bool pixel = ((resultSprite & (0x1 << j)) >> j) == 1; 
                    this[(byte) ((coordX + (15 - j)) % Width), coordY] = pixel ? Pixel.On : Pixel.Off;
                }

                coordY++;
                coordY %= (byte) Height;
            }

            State.V[0xF] = (byte) (collision ? 1 : 0);
            DrawNextTime();
        }

        public void ClearScreen()
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    this[i, j] = Pixel.Off;

            State.Draw = true;
        }
    }
}