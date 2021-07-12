using System;
using bEmu.Core.Memory;
using bEmu.Core.CPU;

namespace bEmu.Core.CPU.Intel8080
{

    public class Disassembler : Core.CPU.Disassembler
    {
        public Disassembler(IMMU mmu) : base(mmu) {}

        public override Instruction GetInstruction(int pointer)
        {
            byte opcode = mmu[pointer];
            byte minInstruction = pointer + 1 < mmu.Length ? mmu[pointer + 1] : (byte)0;
            int maxInstruction = pointer + 2 < mmu.Length ? (mmu[pointer + 2] << 8) | minInstruction : 0;

            switch (opcode)
            {
                case 0x00:
                    return new Instruction("NOP", 1, pointer);
                case 0x01:
                    return new Instruction($"LXI B, #${maxInstruction:x}", 3, pointer);
                case 0x02:
                    return new Instruction("STAX B", 1, pointer);
                case 0x03:
                    return new Instruction("INX B", 1, pointer);
                case 0x04:
                    return new Instruction("INR B", 1, pointer);
                case 0x05:
                    return new Instruction("DCR B", 1, pointer);
                case 0x06:
                    return new Instruction($"MVI B, #${minInstruction:x}", 2, pointer);
                case 0x07:
                    return new Instruction("RLC", 1, pointer);
                case 0x08:
                    return new Instruction("NOP", 1, pointer);
                case 0x09:
                    return new Instruction("DAD B", 1, pointer);
                case 0x0a:
                    return new Instruction("LDAX B", 1, pointer);
                case 0x0b:
                    return new Instruction("DCX B", 1, pointer);
                case 0x0c:
                    return new Instruction("INR C", 1, pointer);
                case 0x0d:
                    return new Instruction("DCR C", 1, pointer);
                case 0x0e:
                    return new Instruction($"MVI C, #${minInstruction:x}", 2, pointer);
                case 0x0f:
                    return new Instruction("RRC", 1, pointer);
                case 0x10:
                    return new Instruction("NOP", 1, pointer);
                case 0x11:
                    return new Instruction($"LXI D, #${maxInstruction:x}", 3, pointer);
                case 0x12:
                    return new Instruction("STAX D", 1, pointer);
                case 0x13:
                    return new Instruction("INX D", 1, pointer);
                case 0x14:
                    return new Instruction("INR D", 1, pointer);
                case 0x15:
                    return new Instruction("DCR D", 1, pointer);
                case 0x16:
                    return new Instruction($"MVI D, #${minInstruction:x}", 2, pointer);
                case 0x17:
                    return new Instruction("RAL", 1, pointer);
                case 0x18:
                    return new Instruction("NOP", 1, pointer);
                case 0x19:
                    return new Instruction("DAD D", 1, pointer);
                case 0x1a:
                    return new Instruction("LDAX D", 1, pointer);
                case 0x1b:
                    return new Instruction("DCX D", 1, pointer);
                case 0x1c:
                    return new Instruction("INR E", 1, pointer);
                case 0x1d:
                    return new Instruction("DCR E", 1, pointer);
                case 0x1e:
                    return new Instruction($"MVI E, #${minInstruction:x}", 2, pointer);
                case 0x1f:
                    return new Instruction("RAR", 1, pointer);
                case 0x20:
                    return new Instruction("RIM", 1, pointer);
                case 0x21:
                    return new Instruction($"LXI H, #${maxInstruction:x}", 3, pointer);
                case 0x22:
                    return new Instruction($"SHLD #${maxInstruction:x}", 3, pointer);
                case 0x23:
                    return new Instruction("INX H", 1, pointer);
                case 0x24:
                    return new Instruction("INR H", 1, pointer);
                case 0x25:
                    return new Instruction("DCR H", 1, pointer);
                case 0x26:
                    return new Instruction($"MVI H, #${minInstruction:x}", 2, pointer);
                case 0x27:
                    return new Instruction("DAA", 1, pointer);
                case 0x28:
                    return new Instruction("NOP", 1, pointer);
                case 0x29:
                    return new Instruction("DAD H", 1, pointer);
                case 0x2a:
                    return new Instruction($"LHLD #${maxInstruction:x}", 3, pointer);
                case 0x2b:
                    return new Instruction("DCX H", 1, pointer);
                case 0x2c:
                    return new Instruction("INR L", 1, pointer);
                case 0x2d:
                    return new Instruction("DCR L", 1, pointer);
                case 0x2e:
                    return new Instruction($"MVI L, #${minInstruction:x}", 1, pointer);
                case 0x2f:
                    return new Instruction("CMA", 1, pointer);
                case 0x30:
                    return new Instruction("SIM", 1, pointer);
                case 0x31:
                    return new Instruction($"LXI SP, #${maxInstruction:x}", 3, pointer);
                case 0x32:
                    return new Instruction($"STA #${maxInstruction:x}", 3, pointer);
                case 0x33:
                    return new Instruction("INX SP", 1, pointer);
                case 0x34:
                    return new Instruction("INR M", 1, pointer);
                case 0x35:
                    return new Instruction("DCR M", 1, pointer);
                case 0x36:
                    return new Instruction($"MVI M, #${minInstruction:x}", 2, pointer);
                case 0x37:
                    return new Instruction("STC", 1, pointer);
                case 0x38:
                    return new Instruction("NOP", 1, pointer);
                case 0x39:
                    return new Instruction("DAD SP", 1, pointer);
                case 0x3a:
                    return new Instruction($"LDA #${maxInstruction:x}", 3, pointer);
                case 0x3b:
                    return new Instruction("DCX SP", 1, pointer);
                case 0x3c:
                    return new Instruction("INR A", 1, pointer);
                case 0x3d:
                    return new Instruction("DCR A", 1, pointer);
                case 0x3e:
                    return new Instruction($"MVI A, #${minInstruction:x}", 2, pointer);
                case 0x3f:
                    return new Instruction("CMC", 1, pointer);
                case 0x40:
                    return new Instruction("MOV B,B", 1, pointer);
                case 0x41:
                    return new Instruction("MOV B,C", 1, pointer);
                case 0x42:
                    return new Instruction("MOV B,D", 1, pointer);
                case 0x43:
                    return new Instruction("MOV B,E", 1, pointer);
                case 0x44:
                    return new Instruction("MOV B,H", 1, pointer);
                case 0x45:
                    return new Instruction("MOV B,L", 1, pointer);
                case 0x46:
                    return new Instruction("MOV B,M", 1, pointer);
                case 0x47:
                    return new Instruction("MOV B,A", 1, pointer);
                case 0x48:
                    return new Instruction("MOV C,B", 1, pointer);
                case 0x49:
                    return new Instruction("MOV C,C", 1, pointer);
                case 0x4a:
                    return new Instruction("MOV C,D", 1, pointer);
                case 0x4b:
                    return new Instruction("MOV C,E", 1, pointer);
                case 0x4c:
                    return new Instruction("MOV C,H", 1, pointer);
                case 0x4d:
                    return new Instruction("MOV C,L", 1, pointer);
                case 0x4e:
                    return new Instruction("MOV C,M", 1, pointer);
                case 0x4f:
                    return new Instruction("MOV C,A", 1, pointer);
                case 0x50:
                    return new Instruction("MOV D,B", 1, pointer);
                case 0x51:
                    return new Instruction("MOV D,C", 1, pointer);
                case 0x52:
                    return new Instruction("MOV D,D", 1, pointer);
                case 0x53:
                    return new Instruction("MOV D,E", 1, pointer);
                case 0x54:
                    return new Instruction("MOV D,H", 1, pointer);
                case 0x55:
                    return new Instruction("MOV D,L", 1, pointer);
                case 0x56:
                    return new Instruction("MOV D,M", 1, pointer);
                case 0x57:
                    return new Instruction("MOV D,A", 1, pointer);
                case 0x58:
                    return new Instruction("MOV E,B", 1, pointer);
                case 0x59:
                    return new Instruction("MOV E,C", 1, pointer);
                case 0x5a:
                    return new Instruction("MOV E,D", 1, pointer);
                case 0x5b:
                    return new Instruction("MOV E,E", 1, pointer);
                case 0x5c:
                    return new Instruction("MOV E,H", 1, pointer);
                case 0x5d:
                    return new Instruction("MOV E,L", 1, pointer);
                case 0x5e:
                    return new Instruction("MOV E,M", 1, pointer);
                case 0x5f:
                    return new Instruction("MOV E,A", 1, pointer);
                case 0x60:
                    return new Instruction("MOV H,B", 1, pointer);
                case 0x61:
                    return new Instruction("MOV H,C", 1, pointer);
                case 0x62:
                    return new Instruction("MOV H,D", 1, pointer);
                case 0x63:
                    return new Instruction("MOV H,E", 1, pointer);
                case 0x64:
                    return new Instruction("MOV H,H", 1, pointer);
                case 0x65:
                    return new Instruction("MOV H,L", 1, pointer);
                case 0x66:
                    return new Instruction("MOV H,M", 1, pointer);
                case 0x67:
                    return new Instruction("MOV H,A", 1, pointer);
                case 0x68:
                    return new Instruction("MOV L,B", 1, pointer);
                case 0x69:
                    return new Instruction("MOV L,C", 1, pointer);
                case 0x6a:
                    return new Instruction("MOV L,D", 1, pointer);
                case 0x6b:
                    return new Instruction("MOV L,E", 1, pointer);
                case 0x6c:
                    return new Instruction("MOV L,H", 1, pointer);
                case 0x6d:
                    return new Instruction("MOV L,L", 1, pointer);
                case 0x6e:
                    return new Instruction("MOV L,M", 1, pointer);
                case 0x6f:
                    return new Instruction("MOV L,A", 1, pointer);
                case 0x70:
                    return new Instruction("MOV M,B", 1, pointer);
                case 0x71:
                    return new Instruction("MOV M,C", 1, pointer);
                case 0x72:
                    return new Instruction("MOV M,D", 1, pointer);
                case 0x73:
                    return new Instruction("MOV M,E", 1, pointer);
                case 0x74:
                    return new Instruction("MOV M,H", 1, pointer);
                case 0x75:
                    return new Instruction("MOV M,L", 1, pointer);
                case 0x76:
                    return new Instruction("HLT", 1, pointer);
                case 0x77:
                    return new Instruction("MOV M,A", 1, pointer);
                case 0x78:
                    return new Instruction("MOV A,B", 1, pointer);
                case 0x79:
                    return new Instruction("MOV A,C", 1, pointer);
                case 0x7a:
                    return new Instruction("MOV A,D", 1, pointer);
                case 0x7b:
                    return new Instruction("MOV A,E", 1, pointer);
                case 0x7c:
                    return new Instruction("MOV A,H", 1, pointer);
                case 0x7d:
                    return new Instruction("MOV A,L", 1, pointer);
                case 0x7e:
                    return new Instruction("MOV A,M", 1, pointer);
                case 0x7f:
                    return new Instruction("MOV A,A", 1, pointer);
                case 0x80:
                    return new Instruction("ADD B", 1, pointer);
                case 0x81:
                    return new Instruction("ADD C", 1, pointer);
                case 0x82:
                    return new Instruction("ADD D", 1, pointer);
                case 0x83:
                    return new Instruction("ADD E", 1, pointer);
                case 0x84:
                    return new Instruction("ADD H", 1, pointer);
                case 0x85:
                    return new Instruction("ADD L", 1, pointer);
                case 0x86:
                    return new Instruction("ADD M", 1, pointer);
                case 0x87:
                    return new Instruction("ADD A", 1, pointer);
                case 0x88:
                    return new Instruction("ADC B", 1, pointer);
                case 0x89:
                    return new Instruction("ADC C", 1, pointer);
                case 0x8a:
                    return new Instruction("ADC D", 1, pointer);
                case 0x8b:
                    return new Instruction("ADC E", 1, pointer);
                case 0x8c:
                    return new Instruction("ADC H", 1, pointer);
                case 0x8d:
                    return new Instruction("ADC L", 1, pointer);
                case 0x8e:
                    return new Instruction("ADC M", 1, pointer);
                case 0x8f:
                    return new Instruction("ADC A", 1, pointer);
                case 0x90:
                    return new Instruction("SUB B", 1, pointer);
                case 0x91:
                    return new Instruction("SUB C", 1, pointer);
                case 0x92:
                    return new Instruction("SUB D", 1, pointer);
                case 0x93:
                    return new Instruction("SUB E", 1, pointer);
                case 0x94:
                    return new Instruction("SUB H", 1, pointer);
                case 0x95:
                    return new Instruction("SUB L", 1, pointer);
                case 0x96:
                    return new Instruction("SUB M", 1, pointer);
                case 0x97:
                    return new Instruction("SUB A", 1, pointer);
                case 0x98:
                    return new Instruction("SBB B", 1, pointer);
                case 0x99:
                    return new Instruction("SUB C", 1, pointer);
                case 0x9a:
                    return new Instruction("SUB D", 1, pointer);
                case 0x9b:
                    return new Instruction("SUB E", 1, pointer);
                case 0x9c:
                    return new Instruction("SUB H", 1, pointer);
                case 0x9d:
                    return new Instruction("SUB L", 1, pointer);
                case 0x9e:
                    return new Instruction("SUB M", 1, pointer);
                case 0x9f:
                    return new Instruction("SUB A", 1, pointer);
                case 0xa0:
                    return new Instruction("ANA B", 1, pointer);
                case 0xa1:
                    return new Instruction("ANA C", 1, pointer);
                case 0xa2:
                    return new Instruction("ANA D", 1, pointer);
                case 0xa3:
                    return new Instruction("ANA E", 1, pointer);
                case 0xa4:
                    return new Instruction("ANA H", 1, pointer);
                case 0xa5:
                    return new Instruction("ANA L", 1, pointer);
                case 0xa6:
                    return new Instruction("ANA M", 1, pointer);
                case 0xa7:
                    return new Instruction("ANA A", 1, pointer);
                case 0xa8:
                    return new Instruction("XRA B", 1, pointer);
                case 0xa9:
                    return new Instruction("XRA C", 1, pointer);
                case 0xaa:
                    return new Instruction("XRA D", 1, pointer);
                case 0xab:
                    return new Instruction("XRA E", 1, pointer);
                case 0xac:
                    return new Instruction("XRA H", 1, pointer);
                case 0xad:
                    return new Instruction("XRA L", 1, pointer);
                case 0xae:
                    return new Instruction("XRA M", 1, pointer);
                case 0xaf:
                    return new Instruction("XRA A", 1, pointer);
                case 0xb0:
                    return new Instruction("ORA B", 1, pointer);
                case 0xb1:
                    return new Instruction("ORA C", 1, pointer);
                case 0xb2:
                    return new Instruction("ORA D", 1, pointer);
                case 0xb3:
                    return new Instruction("ORA E", 1, pointer);
                case 0xb4:
                    return new Instruction("ORA H", 1, pointer);
                case 0xb5:
                    return new Instruction("ORA L", 1, pointer);
                case 0xb6:
                    return new Instruction("ORA M", 1, pointer);
                case 0xb7:
                    return new Instruction("ORA A", 1, pointer);
                case 0xb8:
                    return new Instruction("CMP B", 1, pointer);
                case 0xb9:
                    return new Instruction("CMP C", 1, pointer);
                case 0xba:
                    return new Instruction("CMP D", 1, pointer);
                case 0xbb:
                    return new Instruction("CMP E", 1, pointer);
                case 0xbc:
                    return new Instruction("CMP H", 1, pointer);
                case 0xbd:
                    return new Instruction("CMP L", 1, pointer);
                case 0xbe:
                    return new Instruction("CMP M", 1, pointer);
                case 0xbf:
                    return new Instruction("CMP A", 1, pointer);
                case 0xc0:
                    return new Instruction("RNZ", 1, pointer);
                case 0xc1:
                    return new Instruction("POP B", 1, pointer);
                case 0xc2:
                    return new Instruction($"JNZ #${maxInstruction:x}", 3, pointer);
                case 0xc3:
                    return new Instruction($"JMP #${maxInstruction:x}", 3, pointer);
                case 0xc4:
                    return new Instruction($"CNZ #${maxInstruction:x}", 3, pointer);
                case 0xc5:
                    return new Instruction("PUSH B", 1, pointer);
                case 0xc6:
                    return new Instruction($"ADI #${minInstruction:x}", 2, pointer);
                case 0xc7:
                    return new Instruction("RST 0", 1, pointer);
                case 0xc8:
                    return new Instruction("RZ", 1, pointer);
                case 0xc9:
                    return new Instruction("RET", 1, pointer);
                case 0xca:
                    return new Instruction($"JZ #${maxInstruction:x}", 3, pointer);
                case 0xcb:
                    return new Instruction("NOP", 1, pointer);
                case 0xcc:
                    return new Instruction($"CZ #${maxInstruction:x}", 3, pointer);
                case 0xcd:
                    return new Instruction($"CALL #${maxInstruction:x}", 3, pointer);
                case 0xce:
                    return new Instruction($"ACI #${minInstruction:x}", 2, pointer);
                case 0xcf:
                    return new Instruction("RST 1", 1, pointer);
                case 0xd0:
                    return new Instruction("RNC", 1, pointer);
                case 0xd1:
                    return new Instruction("POP D", 1, pointer);
                case 0xd2:
                    return new Instruction($"JNC #${maxInstruction:x}", 3, pointer);
                case 0xd3:
                    return new Instruction($"OUT #${minInstruction:x}", 2, pointer);
                case 0xd4:
                    return new Instruction($"CNC #${maxInstruction:x}", 3, pointer);
                case 0xd5:
                    return new Instruction("PUSH D", 1, pointer);
                case 0xd6:
                    return new Instruction($"SUI #${minInstruction:x}", 2, pointer);
                case 0xd7:
                    return new Instruction("RST 2", 1, pointer);
                case 0xd8:
                    return new Instruction("RC", 1, pointer);
                case 0xd9:
                    return new Instruction("NOP", 1, pointer);
                case 0xda:
                    return new Instruction($"JC #${maxInstruction:x}", 3, pointer);
                case 0xdb:
                    return new Instruction($"IN #${minInstruction:x}", 2, pointer);
                case 0xdc:
                    return new Instruction($"CC #${maxInstruction:x}", 3, pointer);
                case 0xdd:
                    return new Instruction("NOP", 1, pointer);
                case 0xde:
                    return new Instruction($"SBI #${maxInstruction:x}", 3, pointer);
                case 0xdf:
                    return new Instruction("RST 3", 1, pointer);
                case 0xe0:
                    return new Instruction("RPO", 1, pointer);
                case 0xe1:
                    return new Instruction("POP H", 1, pointer);
                case 0xe2:
                    return new Instruction($"JPO #${maxInstruction:x}", 3, pointer);
                case 0xe3:
                    return new Instruction("XHTL", 1, pointer);
                case 0xe4:
                    return new Instruction($"CPO #${maxInstruction:x}", 3, pointer);
                case 0xe5:
                    return new Instruction("PUSH H", 1, pointer);
                case 0xe6:
                    return new Instruction($"ANI #${minInstruction:x}", 2, pointer);
                case 0xe7:
                    return new Instruction("RST 4", 1, pointer);
                case 0xe8:
                    return new Instruction("RPE", 1, pointer);
                case 0xe9:
                    return new Instruction("PCHL", 1, pointer);
                case 0xea:
                    return new Instruction($"JPE #${maxInstruction:x}", 3, pointer);
                case 0xeb:
                    return new Instruction("XCHG", 1, pointer);
                case 0xec:
                    return new Instruction($"CPE #${maxInstruction:x}", 3, pointer);
                case 0xed:
                    return new Instruction("NOP", 1, pointer);
                case 0xee:
                    return new Instruction($"XRI #${minInstruction:x}", 2, pointer);
                case 0xef:
                    return new Instruction("RST 5", 1, pointer);
                case 0xf0:
                    return new Instruction("RP", 1, pointer);
                case 0xf1:
                    return new Instruction("POP PSW", 1, pointer);
                case 0xf2:
                    return new Instruction($"JP #${maxInstruction:x}", 3, pointer);
                case 0xf3:
                    return new Instruction("DI", 1, pointer);
                case 0xf4:
                    return new Instruction($"CP #${maxInstruction:x}", 3, pointer);
                case 0xf5:
                    return new Instruction("PUSH PSW", 1, pointer);
                case 0xf6:
                    return new Instruction($"ORI #${minInstruction:x}", 2, pointer);
                case 0xf7:
                    return new Instruction("RST 6", 1, pointer);
                case 0xf8:
                    return new Instruction("RM", 1, pointer);
                case 0xf9:
                    return new Instruction("SPHL", 1, pointer);
                case 0xfa:
                    return new Instruction($"JM #${maxInstruction:x}", 3, pointer);
                case 0xfb:
                    return new Instruction("EI", 1, pointer);
                case 0xfc:
                    return new Instruction($"CM #${maxInstruction:x}", 3, pointer);
                case 0xfd:
                    return new Instruction("NOP", 1, pointer);
                case 0xfe:
                    return new Instruction($"CPI #${minInstruction:x}", 2, pointer);
                case 0xff:
                    return new Instruction("RST 7", 1, pointer);
                default:
                    throw new Exception("Instrução não implementada!");
            }
        }
    }
}