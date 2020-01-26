using System;

namespace bEmu.Chip8
{
    public partial class CPU
    {
        private State state;

        public State State
        {
            get { return state; }
        }

        public CPU()
        {
            state = new State();
            state.Memory = new byte[0x1000];
            state.Gfx = new bool[64, 32];
            state.PC = 0x200;
            state.V = new byte[16];
            state.Stack = new ushort[16];
            state.Keys = new bool[16];

            UpdateNumbersInMemory();
        }

        private void UpdateNumbersInMemory()
        {
            for (int i = 0; i < State.Numbers.Length; i++)
                state.Memory[i] = State.Numbers[i];
        }

        public void StepCycle()
        {
            var op = new Opcode((ushort) ((state.Memory[state.PC++] << 8) | (state.Memory[state.PC++])));

            switch (op.Value & 0xF000)
            {
                case 0x0000: 
                    switch (op.Value & 0x00F0)
                    {
                        case 0x00B0: ScrollUp(op.Nibble); break;
                        case 0x00C0: ScrollDown(op.Nibble); break;
                        case 0x00E0:
                            switch (op.Value & 0x000F)
                            {
                                case 0x0000: Cls(); break;
                                case 0x000E: Ret(); break;
                            }
                            break;
                        case 0x00F0:
                            switch (op.Value & 0x000F)
                            {
                                case 0x000B: ScrollLeft(); break;
                                case 0x000C: ScrollRight(); break;
                                case 0x000D: Quit(); break;
                                case 0x000E: Chip8Mode(); break;
                                case 0x000F: SuperChipMode(); break;
                            }
                            break;
                    }
                    break;
                case 0x1000: Jp(op.Nnn); break;
                case 0x2000: Call(op.Nnn); break;
                case 0x3000: SeWithByte(op.X, op.Kk); break;
                case 0x4000: SneWithByte(op.X, op.Kk); break;
                case 0x5000: Se(op.X, op.Y); break;
                case 0x6000: LdWithByte(op.X, op.Kk); break;
                case 0x7000: AddWithByte(op.X, op.Kk); break;
                case 0x8000: 
                    switch (op.Value & 0x000F)
                    {
                        case 0x0000: Ld(op.X, op.Y); break;
                        case 0x0001: Or(op.X, op.Y); break;
                        case 0x0002: And(op.X, op.Y); break;
                        case 0x0003: Xor(op.X, op.Y); break;
                        case 0x0004: Add(op.X, op.Y); break;
                        case 0x0005: Sub(op.X, op.Y); break;
                        case 0x0006: Shr(op.X, op.Y); break;
                        case 0x0007: Subn(op.X, op.Y); break;
                        case 0x000E: Shl(op.X, op.Y); break;
                    }
                    break;
                case 0x9000: Sne(op.X, op.Y); break;
                case 0xA000: LdI(op.Nnn); break; 
                case 0xB000: JpV0(op.Nnn); break;
                case 0xC000: Rnd(op.X, op.Kk); break;
                case 0xD000: Drw(op.X, op.Y, op.Nibble); break;
                case 0xE000: 
                    switch (op.Value & 0x00FF)
                    {
                        case 0x009E: Skp(op.X); break;
                        case 0x00A1: Sknp(op.X); break;
                    }
                    break;
                case 0xF000: 
                    switch (op.Value & 0x00FF)
                    {
                        case 0x0007: LdVxDt(op.X); break;
                        case 0x000A: LdVxK(op.X); break;
                        case 0x0015: LdDtVx(op.X); break;
                        case 0x0018: LdStVx(op.X); break;
                        case 0x001E: AddIVx(op.X); break;
                        case 0x0029: LdFVx(op.X); break;
                        case 0x0030: LdHFVx(op.X); break;
                        case 0x0033: LdBVx(op.X); break;
                        case 0x0055: LdIVx(op.X); break;
                        case 0x0065: LdVxI(op.X); break;
                        case 0x0075: LdRVx(op.X); break;
                        case 0x0085: LdVxR(op.X); break;
                    }
                    break;
            }
        }
    }
}