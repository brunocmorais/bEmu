using System;
using bEmu.Core.Model;

namespace bEmu.Core.VMs.Chip8
{
    public partial class Chip8 : BaseCPU
    {
        private State state;

        public override IState State
        {
            get { return state; }
        }

        public Chip8()
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
            for (int i = 0; i < state.Numbers.Length; i++)
                state.Memory[i] = state.Numbers[i];

            for (int i = 0; i < state.NumbersHiRes.Length; i++)
                state.Memory[i + 0x50] = state.NumbersHiRes[i];
        }

        public override IOpcode StepCycle()
        {
            var opcode = new Opcode(state.Memory[state.PC++], state.Memory[state.PC++]);
            base.StepCycle();

            switch (opcode.UShort & 0xF000)
            {
                case 0x0000: 
                    switch (opcode.UShort & 0x00F0)
                    {
                        case 0x00B0: ScrollUp(opcode.Nibble); break;
                        case 0x00C0: ScrollDown(opcode.Nibble); break;
                        case 0x00E0:
                            switch (opcode.UShort & 0x000F)
                            {
                                case 0x0000: Cls(); break;
                                case 0x000E: Ret(); break;
                            }
                            break;
                        case 0x00F0:
                            switch (opcode.UShort & 0x000F)
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
                case 0x1000: Jp(opcode.Nnn); break;
                case 0x2000: Call(opcode.Nnn); break;
                case 0x3000: SeWithByte(opcode.X, opcode.Kk); break;
                case 0x4000: SneWithByte(opcode.X, opcode.Kk); break;
                case 0x5000: Se(opcode.X, opcode.Y); break;
                case 0x6000: LdWithByte(opcode.X, opcode.Kk); break;
                case 0x7000: AddWithByte(opcode.X, opcode.Kk); break;
                case 0x8000: 
                    switch (opcode.UShort & 0x000F)
                    {
                        case 0x0000: Ld(opcode.X, opcode.Y); break;
                        case 0x0001: Or(opcode.X, opcode.Y); break;
                        case 0x0002: And(opcode.X, opcode.Y); break;
                        case 0x0003: Xor(opcode.X, opcode.Y); break;
                        case 0x0004: Add(opcode.X, opcode.Y); break;
                        case 0x0005: Sub(opcode.X, opcode.Y); break;
                        case 0x0006: Shr(opcode.X, opcode.Y); break;
                        case 0x0007: Subn(opcode.X, opcode.Y); break;
                        case 0x000E: Shl(opcode.X, opcode.Y); break;
                    }
                    break;
                case 0x9000: Sne(opcode.X, opcode.Y); break;
                case 0xA000: LdI(opcode.Nnn); break; 
                case 0xB000: JpV0(opcode.Nnn); break;
                case 0xC000: Rnd(opcode.X, opcode.Kk); break;
                case 0xD000: Drw(opcode.X, opcode.Y, opcode.Nibble); break;
                case 0xE000: 
                    switch (opcode.UShort & 0x00FF)
                    {
                        case 0x009E: Skp(opcode.X); break;
                        case 0x00A1: Sknp(opcode.X); break;
                    }
                    break;
                case 0xF000: 
                    switch (opcode.UShort & 0x00FF)
                    {
                        case 0x0007: LdVxDt(opcode.X); break;
                        case 0x000A: LdVxK(opcode.X); break;
                        case 0x0015: LdDtVx(opcode.X); break;
                        case 0x0018: LdStVx(opcode.X); break;
                        case 0x001E: AddIVx(opcode.X); break;
                        case 0x0029: LdFVx(opcode.X); break;
                        case 0x0030: LdHFVx(opcode.X); break;
                        case 0x0033: LdBVx(opcode.X); break;
                        case 0x0055: LdIVx(opcode.X); break;
                        case 0x0065: LdVxI(opcode.X); break;
                        case 0x0075: LdRVx(opcode.X); break;
                        case 0x0085: LdVxR(opcode.X); break;
                    }
                    break;
            }

            return opcode;
        }
    }
}