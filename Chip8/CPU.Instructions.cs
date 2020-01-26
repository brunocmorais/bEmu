using System;
using System.Linq;
using System.Threading;

namespace bEmu.Chip8
{
    public partial class CPU
    {
        Random random = new Random();

        public void Cls()
        {
            for (int i = 0; i < 64; i++)
                for (int j = 0; j < 32; j++)
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
            if (!state.Keys.Any(x => x))
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
                    bool pixel = state.Gfx[(coordX + j) % 64, coordY];
                    originalSprite |= (byte) ((pixel ? 1 : 0) << (7 - j));
                }
                
                byte resultSprite = (byte) (originalSprite ^ sprite[i]);

                if ((originalSprite & resultSprite) != originalSprite)
                    collision = true;

                for (int j = 7; j >= 0; j--)
                {
                    bool pixel = ((resultSprite & (0x1 << j)) >> j) == 1; 
                    state.Gfx[(byte) ((coordX + (7 - j)) % 64), coordY] = pixel;
                }

                coordY++;
                coordY %= 32;
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
    }
}