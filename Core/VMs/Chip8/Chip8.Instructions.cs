using System;
using System.Linq;
using System.Threading;
using bEmu.Core.Model;
using bEmu.Core.Util;

namespace bEmu.Core.VMs.Chip8
{
    public partial class Chip8 : ICPU
    {
        Random random = new Random();

        public void Cls()
        {
            for (int i = 0; i < state.Gfx.GetLength(0); i++)
                for (int j = 0; j < state.Gfx.GetLength(1); j++)
                    state.Gfx[i, j] = false;

            state.Draw = true;
        }

        public void Ret()
        {
            state.SP--;
            state.PC = state.Stack[state.SP];
        }

        public void Jp(ushort addr)
        {
            state.PC = addr;
        }

        public void Call(ushort addr)
        {
            state.Stack[state.SP] = state.PC;
            state.SP++;
            state.PC = addr;
        }

        private void LdVxI(byte x)
        {
            for (int i = 0; i <= x; i++)
                state.V[i] = state.Memory[i + state.I];
        }

        private void LdIVx(byte x)
        {
            for (int i = 0; i <= x; i++)
                state.Memory[i + state.I] = state.V[i];
        }

        private void LdBVx(byte x)
        {
            string number = state.V[x].ToString().PadLeft(3, '0');
            state.Memory[state.I] = Convert.ToByte(number[0] - 48); 
            state.Memory[state.I + 1] = Convert.ToByte(number[1] - 48);
            state.Memory[state.I + 2] = Convert.ToByte(number[2] - 48);
        }

        private void LdFVx(byte x)
        {
            byte value = state.V[x];
            state.I = (byte) (0x5 * value);
        }

        private void LdHFVx(byte x)
        {
            byte value = state.V[x];
            state.I = (byte) (0x50 + (0xA * value));
        }

        private void AddIVx(byte x)
        {
            if (state.I + state.V[x] > 0xFFF)
                state.V[0xF] = 1;
            else
                state.V[0xF] = 1;

            state.I += state.V[x];

        }

        private void LdStVx(byte x)
        {
            state.Sound = state.V[x];
        }

        private void LdDtVx(byte x)
        {
            state.Delay = state.V[x];
        }

        private void LdVxK(byte x)
        {
            if (!state.Keys.Any(y => y))
                state.PC -= 2;
            else
                for (byte i = 0; i < state.Keys.Length; i++)
                    if (state.Keys[i])
                        state.V[x] = i; 
        }

        private void LdVxDt(byte x)
        {
            state.V[x] = state.Delay;
        }

        private void Skp(byte x)
        {
            if (state.Keys[state.V[x]])
                state.PC += 2;
        }

        private void Sknp(byte x)
        {
            if (!state.Keys[state.V[x]])
                state.PC += 2;
        }

        private void Drw(byte x, byte y, byte n)
        {
            if (n == 0)
                DrwSuperChip(x, y);
            else
            {
                byte[] sprite = new byte[n];

                for (int i = 0; i < n; i++)
                    sprite[i] = state.Memory[state.I + i];

                byte coordX = state.V[x];
                byte coordY = state.V[y];
                bool collision = false;

                for (int i = 0; i < n; i++)
                {
                    byte originalSprite = 0;

                    for (int j = 0; j < 8; j++)
                    {
                        bool pixel = state.Gfx[(coordX + j) % state.Gfx.GetLength(0), coordY];
                        originalSprite |= (byte) ((pixel ? 1 : 0) << (7 - j));
                    }
                    
                    byte resultSprite = (byte) (originalSprite ^ sprite[i]);

                    if ((originalSprite & resultSprite) != originalSprite)
                        collision = true;

                    for (int j = 7; j >= 0; j--)
                    {
                        bool pixel = ((resultSprite & (0x1 << j)) >> j) == 1; 
                        state.Gfx[(byte) ((coordX + (7 - j)) % state.Gfx.GetLength(0)), coordY] = pixel;
                    }

                    coordY++;
                    coordY %= (byte) state.Gfx.GetLength(1);
                }

                state.V[0xF] = (byte) (collision ? 1 : 0);
                state.Draw = true;
            }
        }

        private void DrwSuperChip(byte x, byte y)
        {
            ushort[] sprite = new ushort[16];

            for (int i = 0; i < 16; i++)
                sprite[i] = GeneralUtils.Get16BitNumber(state.Memory[state.I + (2 * i) + 1], state.Memory[state.I + (2 * i)]);

            byte coordX = state.V[x];
            byte coordY = state.V[y];
            bool collision = false;

            for (int i = 0; i < 16; i++)
            {
                ushort originalSprite = 0;

                for (int j = 0; j < 16; j++)
                {
                    bool pixel = state.Gfx[(coordX + j) % state.Gfx.GetLength(0), coordY];
                    originalSprite |= (ushort) ((pixel ? 1 : 0) << (15 - j));
                }
                
                ushort resultSprite = (ushort) (originalSprite ^ sprite[i]);

                if ((originalSprite & resultSprite) != originalSprite)
                    collision = true;

                for (int j = 15; j >= 0; j--)
                {
                    bool pixel = ((resultSprite & (0x1 << j)) >> j) == 1; 
                    state.Gfx[(byte) ((coordX + (15 - j)) % state.Gfx.GetLength(0)), coordY] = pixel;
                }

                coordY++;
                coordY %= (byte) state.Gfx.GetLength(1);
            }

            state.V[0xF] = (byte) (collision ? 1 : 0);
            state.Draw = true;
        }

        private void Rnd(byte x, byte kk)
        {
            byte rnd = (byte) random.Next(0x100);
            state.V[x] = (byte) (rnd & kk);
        }

        private void JpV0(ushort addr)
        {
            state.PC = (ushort) (addr + state.V[0]);
        }

        private void LdI(ushort addr)
        {
            state.I = addr;
        }

        private void Sne(byte x, byte y)
        {
            if (state.V[x] != state.V[y])
                state.PC += 2;
        }

        private void Shl(byte x, byte y)
        {
            state.V[0xF] = (byte) ((state.V[x] & 0x80) == 0x80 ? 1 : 0);
            state.V[x] <<= 1;
        }

        private void Subn(byte x, byte y)
        {
            state.V[0xF] = (byte) (state.V[y] > state.V[x] ? 1 : 0);
            state.V[x] = (byte) (state.V[y] - state.V[x]);
        }

        private void Shr(byte x, byte y)
        {
            state.V[0xF] = (byte) ((state.V[x] & 0x1) == 1 ? 1 : 0);
            state.V[x] >>= 1;
        }

        private void Sub(byte x, byte y)
        {
            state.V[0xF] = (byte) (state.V[x] > state.V[y] ? 1 : 0);
            state.V[x] -= state.V[y];
        }

        private void Add(byte x, byte y)
        {
            state.V[0xF] = (byte) (state.V[x] + state.V[y] > 0xFF ? 1 : 0);
            state.V[x] += state.V[y];
        }

        private void Xor(byte x, byte y)
        {
            state.V[x] ^= state.V[y];
        }

        private void And(byte x, byte y)
        {
            state.V[x] &= state.V[y];
        }

        private void Or(byte x, byte y)
        {
            state.V[x] |= state.V[y];
        }

        private void Ld(byte x, byte y)
        {
            state.V[x] = state.V[y];
        }

        private void AddWithByte(byte x, byte kk)
        {
            state.V[x] += kk;
        }

        private void LdWithByte(byte x, byte kk)
        {
            state.V[x] = kk;
        }

        private void Se(byte x, byte y)
        {
            if (state.V[x] == state.V[y])
                state.PC += 2;
        }

        private void SneWithByte(byte x, byte kk)
        {
            if (state.V[x] != kk)
                state.PC += 2;
        }

        private void SeWithByte(byte x, byte kk)
        {
            if (state.V[x] == kk)
                state.PC += 2;
        }

        private void ScrollDown(byte nibble)
        {
            var newGfx = new bool[state.Gfx.GetLength(0), state.Gfx.GetLength(1)];

            for (int i = 0; i < newGfx.GetLength(0); i++)
                for (int j = 0; j < newGfx.GetLength(1) - nibble; j++)
                    newGfx[i, (j + nibble)] = state.Gfx[i, j];

            state.Gfx = newGfx;
            state.Draw = true;
        }

        private void ScrollUp(byte nibble)
        {
            var newGfx = new bool[state.Gfx.GetLength(0), state.Gfx.GetLength(1)];

            for (int i = 0; i < newGfx.GetLength(0); i++)
                for (int j = nibble; j < newGfx.GetLength(1); j++)
                    newGfx[i, (j - nibble)] = state.Gfx[i, j];

            state.Gfx = newGfx;
            state.Draw = true;
        }

        private void Quit()
        {
            state.Halted = true;
        }

        private void SuperChipMode()
        {
            state.SuperChipMode = true;
            state.Gfx = new bool[128, 64];
            state.R = new byte[8];
        }

        private void Chip8Mode()
        {
            state.SuperChipMode = false;
            state.Gfx = new bool[64, 32];
        }

        private void LdVxR(byte x)
        {
            state.V[x] = state.R[x];
        }

        private void LdRVx(byte x)
        {
            state.R[x] = state.V[x];
        }

        private void ScrollRight()
        {
            for (int i = state.Gfx.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = state.Gfx.GetLength(1) - 1; j >= 0; j--)
                {
                    if ((i - 4) < 0)
                        state.Gfx[i, j] = false;
                    else
                        state.Gfx[i, j] = state.Gfx[i - 4, j];
                }   
            }
        }

        private void ScrollLeft()
        {
            for (int i = 0; i < state.Gfx.GetLength(0); i++)
            {
                for (int j = 0; j < state.Gfx.GetLength(1); j++)
                {
                    if ((i + 4) > state.Gfx.GetLength(0))
                        state.Gfx[i, j] = false;
                    else
                        state.Gfx[i, j] = state.Gfx[i + 4, j];
                }   
            }

            state.Draw = true;
        }
    }
}