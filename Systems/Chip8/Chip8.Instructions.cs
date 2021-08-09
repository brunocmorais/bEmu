using System;
using System.Linq;
using System.Threading;
using bEmu.Core;
using bEmu.Core.CPU;
using bEmu.Core.Util;

namespace bEmu.Systems.Chip8
{
    public partial class Chip8 : VM<State, MMU, PPU, APU>
    {
        private void Ret()
        {
            State.SP--;
            State.PC = State.Stack[State.SP];
        }

        private void Jp(ushort addr)
        {
            State.PC = addr;
        }

        private void Call(ushort addr)
        {
            State.Stack[State.SP] = State.PC;
            State.SP++;
            State.PC = addr;
        }

        private void LdVxI(byte x)
        {
            for (int i = 0; i <= x; i++)
                State.V[i] = MMU[i + State.I];
        }

        private void LdIVx(byte x)
        {
            for (int i = 0; i <= x; i++)
                MMU[i + State.I] = State.V[i];
        }

        private void LdBVx(byte x)
        {
            string number = State.V[x].ToString().PadLeft(3, '0');
            MMU[State.I] = Convert.ToByte(number[0] - 48); 
            MMU[State.I + 1] = Convert.ToByte(number[1] - 48);
            MMU[State.I + 2] = Convert.ToByte(number[2] - 48);
        }

        private void LdFVx(byte x)
        {
            byte value = State.V[x];
            State.I = (byte) (0x5 * value);
        }

        private void LdHFVx(byte x)
        {
            byte value = State.V[x];
            State.I = (byte) (0x50 + (0xA * value));
        }

        private void AddIVx(byte x)
        {
            if (State.I + State.V[x] > 0xFFF)
                State.V[0xF] = 1;
            else
                State.V[0xF] = 1;

            State.I += State.V[x];

        }

        private void LdStVx(byte x)
        {
            State.Sound = State.V[x];
        }

        private void LdDtVx(byte x)
        {
            State.Delay = State.V[x];
        }

        private void LdVxK(byte x)
        {
            if (!State.Keys.Any(y => y))
                State.PC -= 2;
            else
                for (byte i = 0; i < State.Keys.Length; i++)
                    if (State.Keys[i])
                        State.V[x] = i; 
        }

        private void LdVxDt(byte x)
        {
            State.V[x] = State.Delay;
        }

        private void Skp(byte x)
        {
            if (State.Keys[State.V[x]])
                State.PC += 2;
        }

        private void Sknp(byte x)
        {
            if (!State.Keys[State.V[x]])
                State.PC += 2;
        }

        private void Rnd(byte x, byte kk)
        {
            byte rnd = (byte) random.Next(0x100);
            State.V[x] = (byte) (rnd & kk);
        }

        private void JpV0(ushort addr)
        {
            State.PC = (ushort) (addr + State.V[0]);
        }

        private void LdI(ushort addr)
        {
            State.I = addr;
        }

        private void Sne(byte x, byte y)
        {
            if (State.V[x] != State.V[y])
                State.PC += 2;
        }

        private void Shl(byte x, byte y)
        {
            State.V[0xF] = (byte) ((State.V[x] & 0x80) == 0x80 ? 1 : 0);
            State.V[x] <<= 1;
        }

        private void Subn(byte x, byte y)
        {
            State.V[0xF] = (byte) (State.V[y] > State.V[x] ? 1 : 0);
            State.V[x] = (byte) (State.V[y] - State.V[x]);
        }

        private void Shr(byte x, byte y)
        {
            State.V[0xF] = (byte) ((State.V[x] & 0x1) == 1 ? 1 : 0);
            State.V[x] >>= 1;
        }

        private void Sub(byte x, byte y)
        {
            State.V[0xF] = (byte) (State.V[x] > State.V[y] ? 1 : 0);
            State.V[x] -= State.V[y];
        }

        private void Add(byte x, byte y)
        {
            State.V[0xF] = (byte) (State.V[x] + State.V[y] > 0xFF ? 1 : 0);
            State.V[x] += State.V[y];
        }

        private void Xor(byte x, byte y)
        {
            State.V[x] ^= State.V[y];
        }

        private void And(byte x, byte y)
        {
            State.V[x] &= State.V[y];
        }

        private void Or(byte x, byte y)
        {
            State.V[x] |= State.V[y];
        }

        private void Ld(byte x, byte y)
        {
            State.V[x] = State.V[y];
        }

        private void AddWithByte(byte x, byte kk)
        {
            State.V[x] += kk;
        }

        private void LdWithByte(byte x, byte kk)
        {
            State.V[x] = kk;
        }

        private void Se(byte x, byte y)
        {
            if (State.V[x] == State.V[y])
                State.PC += 2;
        }

        private void SneWithByte(byte x, byte kk)
        {
            if (State.V[x] != kk)
                State.PC += 2;
        }

        private void SeWithByte(byte x, byte kk)
        {
            if (State.V[x] == kk)
                State.PC += 2;
        }

        private void Quit()
        {
            State.Halted = true;
        }

        private void LdVxR(byte x)
        {
            State.V[x] = State.R[x];
        }

        private void LdRVx(byte x)
        {
            State.R[x] = State.V[x];
        }
    }
}