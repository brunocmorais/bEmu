using System;
using bEmu.Core;
using bEmu.Systems.Chip8;

namespace bEmu.Core.VMs.Chip8
{
    public partial class Chip8 : VM<bEmu.Systems.Chip8.State, bEmu.Systems.Chip8.PPU>
    {
        private Random random;
        
        public Chip8(ISystem system) : base(system) 
        { 
            random = new Random();
        }

        public override IOpcode StepCycle()
        {
            var opcode = new Opcode(MMU[State.PC++], MMU[State.PC++]);
            base.StepCycle();

            switch (opcode.UShort & 0xF000)
            {
                case 0x0000: 
                    switch (opcode.UShort & 0x00F0)
                    {
                        case 0x00B0: PPU.ScrollUp(opcode.Nibble); break;
                        case 0x00C0: PPU.ScrollDown(opcode.Nibble); break;
                        case 0x00E0:
                            switch (opcode.UShort & 0x000F)
                            {
                                case 0x0000: PPU.ClearScreen(); break;
                                case 0x000E: Ret(); break;
                            }
                            break;
                        case 0x00F0:
                            switch (opcode.UShort & 0x000F)
                            {
                                case 0x000B: PPU.ScrollLeft(); break;
                                case 0x000C: PPU.ScrollRight(); break;
                                case 0x000D: Quit(); break;
                                case 0x000E: (System as Systems.Chip8.System).SetChip8Mode(); break;
                                case 0x000F: (System as Systems.Chip8.System).SetSuperChipMode(); break;
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
                case 0xD000: PPU.Drw(opcode.X, opcode.Y, opcode.Nibble); break;
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