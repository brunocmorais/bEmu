using System;

namespace bEmu.Core.CPUs.Z80
{
    public class Disassembler : Core.Disassembler
    {
        public Disassembler(IMMU mmu) : base(mmu) { }

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
                    return new Instruction($"LD BC, #${maxInstruction:x}", 3, pointer);
                case 0x02:
                    return new Instruction("LD (BC), A", 1, pointer);
                case 0x03:
                    return new Instruction("INC BC", 1, pointer);
                case 0x04:
                    return new Instruction("INC B", 1, pointer);
                case 0x05:
                    return new Instruction("DEC B", 1, pointer);
                case 0x06:     
                    return new Instruction($"LD B, #${minInstruction:x}", 2, pointer);
                case 0x07:
                    return new Instruction("RLCA", 1, pointer);
                case 0x08:
                    return new Instruction($"LD (#${maxInstruction:x}), SP", 3, pointer);
                case 0x09:
                    return new Instruction("ADD HL, BC", 1, pointer);
                case 0x0a:
                    return new Instruction("LD A, (BC)", 1, pointer);
                case 0x0b:
                    return new Instruction("DEC BC", 1, pointer);
                case 0x0c:
                    return new Instruction("INC C", 1, pointer);
                case 0x0d:
                    return new Instruction("DEC C", 1, pointer);
                case 0x0e:
                    return new Instruction($"LD C, #${minInstruction:x}", 2, pointer);
                case 0x0f:
                    return new Instruction("RRCA", 1, pointer);
                case 0x10:
                    return new Instruction("STOP 0", 1, pointer);
                case 0x11:
                    return new Instruction($"LD DE, #${maxInstruction:x}", 3, pointer);
                case 0x12:
                    return new Instruction("LD (DE), A", 1, pointer);
                case 0x13:
                    return new Instruction("INC DE", 1, pointer);
                case 0x14:
                    return new Instruction("INC D", 1, pointer);
                case 0x15:
                    return new Instruction("DEC D", 1, pointer);
                case 0x16:
                    return new Instruction($"LD D, #${minInstruction:x}", 2, pointer);
                case 0x17:
                    return new Instruction("RLA", 1, pointer);
                case 0x18:
                    return new Instruction($"JR #${minInstruction:x}", 2, pointer);
                case 0x19:
                    return new Instruction("ADD HL, DE", 1, pointer);
                case 0x1a:
                    return new Instruction("LD A, (DE)", 1, pointer);
                case 0x1b:
                    return new Instruction("DEC DE", 1, pointer);
                case 0x1c:
                    return new Instruction("INC E", 1, pointer);
                case 0x1d:
                    return new Instruction("DEC E", 1, pointer);
                case 0x1e:
                    return new Instruction($"LD E, #${minInstruction:x}", 2, pointer);
                case 0x1f:
                    return new Instruction("RRA", 1, pointer);
                case 0x20:
                    return new Instruction($"JR NZ, #${minInstruction:x}", 2, pointer);
                case 0x21:
                    return new Instruction($"LD HL, #${maxInstruction:x}", 3, pointer);
                case 0x22:
                    return new Instruction("LD (HL+), A", 1, pointer);
                case 0x23:
                    return new Instruction("INC HL", 1, pointer);
                case 0x24:
                    return new Instruction("INC H", 1, pointer);
                case 0x25:
                    return new Instruction("DEC H", 1, pointer);
                case 0x26:
                    return new Instruction($"LD H, #${minInstruction:x}", 2, pointer);
                case 0x27:
                    return new Instruction("DAA", 1, pointer);
                case 0x28:
                    return new Instruction($"JR Z, #${minInstruction:x}", 2, pointer);
                case 0x29:
                    return new Instruction("ADD HL, HL", 1, pointer);
                case 0x2a:
                    return new Instruction("LD A, (HL+)", 1, pointer);
                case 0x2b:
                    return new Instruction("DEC HL", 1, pointer);
                case 0x2c:
                    return new Instruction("INC L", 1, pointer);
                case 0x2d:
                    return new Instruction("DEC L", 1, pointer);
                case 0x2e:
                    return new Instruction($"LD L, #${minInstruction:x}", 2, pointer);
                case 0x2f:
                    return new Instruction("CPL", 1, pointer);
                case 0x30:
                    return new Instruction($"JR NC, #${minInstruction:x}", 2, pointer);
                case 0x31:
                    return new Instruction($"LD SP, #${maxInstruction:x}", 3, pointer);
                case 0x32:
                    return new Instruction("LD (HL-), A", 1, pointer);
                case 0x33:
                    return new Instruction("INC SP", 1, pointer);
                case 0x34:
                    return new Instruction("INC (HL)", 1, pointer);
                case 0x35:
                    return new Instruction("DEC (HL)", 1, pointer);
                case 0x36:
                    return new Instruction($"LD (HL), #${minInstruction:x}", 2, pointer);
                case 0x37:
                    return new Instruction("SCF", 1, pointer);
                case 0x38:
                    return new Instruction($"JR C, #${minInstruction:x}", 2, pointer);
                case 0x39:
                    return new Instruction("ADD HL, SP", 1, pointer);
                case 0x3a:
                    return new Instruction("LD A, (HL-)", 1, pointer);
                case 0x3b:
                    return new Instruction("DEC SP", 1, pointer);
                case 0x3c:
                    return new Instruction("INC A", 1, pointer);
                case 0x3d:
                    return new Instruction("DEC A", 1, pointer);
                case 0x3e:
                    return new Instruction($"LD A, #${minInstruction:x}", 2, pointer);
                case 0x3f:
                    return new Instruction("CCF", 1, pointer);
                case 0x40:
                    return new Instruction("LD B, B", 1, pointer);
                case 0x41:
                    return new Instruction("LD B, C", 1, pointer);
                case 0x42:
                    return new Instruction("LD B, D", 1, pointer);
                case 0x43:
                    return new Instruction("LD B, E", 1, pointer);
                case 0x44:
                    return new Instruction("LD B, H", 1, pointer);
                case 0x45:
                    return new Instruction("LD B, L", 1, pointer);
                case 0x46:
                    return new Instruction("LD B, (HL)", 1, pointer);
                case 0x47:
                    return new Instruction("LD B, A", 1, pointer);
                case 0x48:
                    return new Instruction("LD C, B", 1, pointer);
                case 0x49:
                    return new Instruction("LD C, C", 1, pointer);
                case 0x4a:
                    return new Instruction("LD C, D", 1, pointer);
                case 0x4b:
                    return new Instruction("LD C, E", 1, pointer);
                case 0x4c:
                    return new Instruction("LD C, H", 1, pointer);
                case 0x4d:
                    return new Instruction("LD C, L", 1, pointer);
                case 0x4e:
                    return new Instruction("LD C, (HL)", 1, pointer);
                case 0x4f:
                    return new Instruction("LD C, A", 1, pointer);
                case 0x50:
                    return new Instruction("LD D, B", 1, pointer);
                case 0x51:
                    return new Instruction("LD D, C", 1, pointer);
                case 0x52:
                    return new Instruction("LD D, D", 1, pointer);
                case 0x53:
                    return new Instruction("LD D, E", 1, pointer);
                case 0x54:
                    return new Instruction("LD D, H", 1, pointer);
                case 0x55:
                    return new Instruction("LD D, L", 1, pointer);
                case 0x56:
                    return new Instruction("LD D, (HL)", 1, pointer);
                case 0x57:
                    return new Instruction("LD D, A", 1, pointer);
                case 0x58:
                    return new Instruction("LD E, B", 1, pointer);
                case 0x59:
                    return new Instruction("LD E, C", 1, pointer);
                case 0x5a:
                    return new Instruction("LD E, D", 1, pointer);
                case 0x5b:
                    return new Instruction("LD E, E", 1, pointer);
                case 0x5c:
                    return new Instruction("LD E, H", 1, pointer);
                case 0x5d:
                    return new Instruction("LD E, L", 1, pointer);
                case 0x5e:
                    return new Instruction("LD E, (HL)", 1, pointer);
                case 0x5f:
                    return new Instruction("LD E, A", 1, pointer);
                case 0x60:
                    return new Instruction("LD H, B", 1, pointer);
                case 0x61:
                    return new Instruction("LD H, C", 1, pointer);
                case 0x62:
                    return new Instruction("LD H, D", 1, pointer);
                case 0x63:
                    return new Instruction("LD H, E", 1, pointer);
                case 0x64:
                    return new Instruction("LD H, H", 1, pointer);
                case 0x65:
                    return new Instruction("LD H, L", 1, pointer);
                case 0x66:
                    return new Instruction("LD H, (HL)", 1, pointer);
                case 0x67:
                    return new Instruction("LD H, A", 1, pointer);
                case 0x68:
                    return new Instruction("LD L, B", 1, pointer);
                case 0x69:
                    return new Instruction("LD L, C", 1, pointer);
                case 0x6a:
                    return new Instruction("LD L, D", 1, pointer);
                case 0x6b:
                    return new Instruction("LD L, E", 1, pointer);
                case 0x6c:
                    return new Instruction("LD L, H", 1, pointer);
                case 0x6d:
                    return new Instruction("LD L, L", 1, pointer);
                case 0x6e:
                    return new Instruction("LD L, (HL)", 1, pointer);
                case 0x6f:
                    return new Instruction("LD L, A", 1, pointer);
                case 0x70:
                    return new Instruction("LD (HL), B", 1, pointer);
                case 0x71:
                    return new Instruction("LD (HL), C", 1, pointer);
                case 0x72:
                    return new Instruction("LD (HL), D", 1, pointer);
                case 0x73:
                    return new Instruction("LD (HL), E", 1, pointer);
                case 0x74:
                    return new Instruction("LD (HL), H", 1, pointer);
                case 0x75:
                    return new Instruction("LD (HL), L", 1, pointer);
                case 0x76:
                    return new Instruction("HALT", 1, pointer);
                case 0x77:
                    return new Instruction("LD (HL), A", 1, pointer);
                case 0x78:
                    return new Instruction("LD A, B", 1, pointer);
                case 0x79:
                    return new Instruction("LD A, C", 1, pointer);
                case 0x7a:
                    return new Instruction("LD A, D", 1, pointer);
                case 0x7b:
                    return new Instruction("LD A, E", 1, pointer);
                case 0x7c:
                    return new Instruction("LD A, H", 1, pointer);
                case 0x7d:
                    return new Instruction("LD A, L", 1, pointer);
                case 0x7e:
                    return new Instruction("LD A, (HL)", 1, pointer);
                case 0x7f:
                    return new Instruction("LD A, A", 1, pointer);
                case 0x80:
                    return new Instruction("ADD A, B", 1, pointer);
                case 0x81:
                    return new Instruction("ADD A, C", 1, pointer);
                case 0x82:
                    return new Instruction("ADD A, D", 1, pointer);
                case 0x83:
                    return new Instruction("ADD A, E", 1, pointer);
                case 0x84:
                    return new Instruction("ADD A, H", 1, pointer);
                case 0x85:
                    return new Instruction("ADD A, L", 1, pointer);
                case 0x86:
                    return new Instruction("ADD A, (HL)", 1, pointer);
                case 0x87:
                    return new Instruction("ADD A, A", 1, pointer);
                case 0x88:
                    return new Instruction("ADC A, B", 1, pointer);
                case 0x89:
                    return new Instruction("ADC A, C", 1, pointer);
                case 0x8a:
                    return new Instruction("ADC A, D", 1, pointer);
                case 0x8b:
                    return new Instruction("ADC A, E", 1, pointer);
                case 0x8c:
                    return new Instruction("ADC A, H", 1, pointer);
                case 0x8d:
                    return new Instruction("ADC A, L", 1, pointer);
                case 0x8e:
                    return new Instruction("ADC A, (HL)", 1, pointer);
                case 0x8f:
                    return new Instruction("ADC A, A", 1, pointer);
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
                    return new Instruction("SUB (HL)", 1, pointer);
                case 0x97:
                    return new Instruction("SUB A", 1, pointer);
                case 0x98:
                    return new Instruction("SBC A, B", 1, pointer);
                case 0x99:
                    return new Instruction("SBC A, C", 1, pointer);
                case 0x9a:
                    return new Instruction("SBC A, D", 1, pointer);
                case 0x9b:
                    return new Instruction("SBC A, E", 1, pointer);
                case 0x9c:
                    return new Instruction("SBC A, H", 1, pointer);
                case 0x9d:
                    return new Instruction("SBC A, L", 1, pointer);
                case 0x9e:
                    return new Instruction("SBC A, (HL)", 1, pointer);
                case 0x9f:
                    return new Instruction("SBC A, A", 1, pointer);
                case 0xa0:
                    return new Instruction("AND B", 1, pointer);
                case 0xa1:
                    return new Instruction("AND C", 1, pointer);
                case 0xa2:
                    return new Instruction("AND D", 1, pointer);
                case 0xa3:
                    return new Instruction("AND E", 1, pointer);
                case 0xa4:
                    return new Instruction("AND H", 1, pointer);
                case 0xa5:
                    return new Instruction("AND L", 1, pointer);
                case 0xa6:
                    return new Instruction("AND (HL)", 1, pointer);
                case 0xa7:
                    return new Instruction("AND A", 1, pointer);
                case 0xa8:
                    return new Instruction("XOR B", 1, pointer);
                case 0xa9:
                    return new Instruction("XOR C", 1, pointer);
                case 0xaa:
                    return new Instruction("XOR D", 1, pointer);
                case 0xab:
                    return new Instruction("XOR E", 1, pointer);
                case 0xac:
                    return new Instruction("XOR H", 1, pointer);
                case 0xad:
                    return new Instruction("XOR L", 1, pointer);
                case 0xae:
                    return new Instruction("XOR (HL)", 1, pointer);
                case 0xaf:
                    return new Instruction("XOR A", 1, pointer);
                case 0xb0:
                    return new Instruction("OR B", 1, pointer);
                case 0xb1:
                    return new Instruction("OR C", 1, pointer);
                case 0xb2:
                    return new Instruction("OR D", 1, pointer);
                case 0xb3:
                    return new Instruction("OR E", 1, pointer);
                case 0xb4:
                    return new Instruction("OR H", 1, pointer);
                case 0xb5:
                    return new Instruction("OR L", 1, pointer);
                case 0xb6:
                    return new Instruction("OR (HL)", 1, pointer);
                case 0xb7:
                    return new Instruction("OR A", 1, pointer);
                case 0xb8:
                    return new Instruction("CP B", 1, pointer);
                case 0xb9:
                    return new Instruction("CP C", 1, pointer);
                case 0xba:
                    return new Instruction("CP D", 1, pointer);
                case 0xbb:
                    return new Instruction("CP E", 1, pointer);
                case 0xbc:
                    return new Instruction("CP H", 1, pointer);
                case 0xbd:
                    return new Instruction("CP L", 1, pointer);
                case 0xbe:
                    return new Instruction("CP (HL)", 1, pointer);
                case 0xbf:
                    return new Instruction("CP A", 1, pointer);
                case 0xc0:
                    return new Instruction("RET NZ", 1, pointer);
                case 0xc1:
                    return new Instruction("POP BC", 1, pointer);
                case 0xc2:
                    return new Instruction($"JP NZ, #${maxInstruction:x}", 3, pointer);
                case 0xc3:
                    return new Instruction($"JP #${maxInstruction:x}", 3, pointer);
                case 0xc4:
                    return new Instruction($"CALL NZ, #${maxInstruction:x}", 3, pointer);
                case 0xc5:
                    return new Instruction("PUSH BC", 1, pointer);
                case 0xc6:
                    return new Instruction($"ADD A, #${minInstruction:x}", 2, pointer);
                case 0xc7:
                    return new Instruction("RST 00H", 1, pointer);
                case 0xc8:
                    return new Instruction("RET Z", 1, pointer);
                case 0xc9:
                    return new Instruction("RET", 1, pointer);
                case 0xca:
                    return new Instruction($"JP Z, #${maxInstruction:x}", 3, pointer);
                case 0xcb:
                    return GetCBInstruction(mmu[pointer + 1], pointer);
                case 0xcc:
                    return new Instruction($"CALL Z, #${maxInstruction:x}", 3, pointer);
                case 0xcd:
                    return new Instruction($"CALL #${maxInstruction:x}", 3, pointer);
                case 0xce:
                    return new Instruction($"ADC A, #${minInstruction:x}", 2, pointer);
                case 0xcf:
                    return new Instruction("RST 08H", 1, pointer);
                case 0xd0:
                    return new Instruction("RET NC", 1, pointer);
                case 0xd1:
                    return new Instruction("POP DE", 1, pointer);
                case 0xd2:
                    return new Instruction($"JP NC, #${maxInstruction:x}", 3, pointer);
                case 0xd3:
                    return Instruction.Empty;
                case 0xd4:
                    return new Instruction($"CALL NC, #${maxInstruction:x}", 3, pointer);
                case 0xd5:
                    return new Instruction("PUSH DE", 1, pointer);
                case 0xd6:
                    return new Instruction($"SUB #${minInstruction:x}", 2, pointer);
                case 0xd7:
                    return new Instruction("RST 10H", 1, pointer);
                case 0xd8:
                    return new Instruction("RET C", 1, pointer);
                case 0xd9:
                    return new Instruction("RETI", 1, pointer);
                case 0xda:
                    return new Instruction($"JP C, #${maxInstruction:x}", 3, pointer);
                case 0xdb:
                    return Instruction.Empty;
                case 0xdc:
                    return new Instruction($"CALL C, #${maxInstruction:x}", 3, pointer);
                case 0xdd:
                    return Instruction.Empty;
                case 0xde:
                    return new Instruction($"SBC A, #${minInstruction:x}", 2, pointer);
                case 0xdf:
                    return new Instruction("RST 18H", 1, pointer);
                case 0xe0:
                    return new Instruction($"LDH (#${minInstruction:x}), A", 2, pointer);
                case 0xe1:
                    return new Instruction("POP HL", 1, pointer);
                case 0xe2:
                    return new Instruction("LD (C), A", 1, pointer);
                case 0xe3:
                    return Instruction.Empty;
                case 0xe4:
                    return Instruction.Empty;
                case 0xe5:
                    return new Instruction("PUSH HL", 1, pointer);
                case 0xe6:
                    return new Instruction($"AND #${minInstruction:x}", 2, pointer);
                case 0xe7:
                    return new Instruction("RST 20H", 1, pointer);
                case 0xe8:
                    return new Instruction($"ADD SP, #${minInstruction:x}", 2, pointer);
                case 0xe9:
                    return new Instruction("JP (HL)", 1, pointer);
                case 0xea:
                    return new Instruction($"LD (#${maxInstruction:x}), A", 3, pointer);
                case 0xeb:
                    return Instruction.Empty;
                case 0xec:
                    return Instruction.Empty;
                case 0xed:
                    return Instruction.Empty;
                case 0xee:
                    return new Instruction($"XOR #${minInstruction:x}", 2, pointer);
                case 0xef:
                    return new Instruction("RST 28H", 1, pointer);
                case 0xf0:
                    return new Instruction($"LDH A, (#${minInstruction:x})", 2, pointer);
                case 0xf1:
                    return new Instruction("POP AF", 1, pointer);
                case 0xf2:
                    return new Instruction("LD A, (C)", 1, pointer);
                case 0xf3:
                    return new Instruction("DI", 1, pointer);
                case 0xf4:
                    return Instruction.Empty;
                case 0xf5:
                    return new Instruction("PUSH AF", 1, pointer);
                case 0xf6:
                    return new Instruction($"OR #${minInstruction:x}", 2, pointer);
                case 0xf7:
                    return new Instruction("RST 30H", 1, pointer);
                case 0xf8:
                    return new Instruction($"LD HL, SP+#${minInstruction:x}", 2, pointer);
                case 0xf9:
                    return new Instruction("LD SP, HL", 1, pointer);
                case 0xfa:
                    return new Instruction($"LD A, (#${maxInstruction:x})", 3, pointer);
                case 0xfb:
                    return new Instruction("EI", 1, pointer);
                case 0xfc:
                    return Instruction.Empty;
                case 0xfd:
                    return Instruction.Empty;
                case 0xfe:
                    return new Instruction($"CP #${minInstruction:x}", 2, pointer);
                case 0xff:
                    return new Instruction("RST 38H", 1, pointer);
            }

            throw new Exception();
        }

        private Instruction GetCBInstruction(byte instruction, int pointer)
        {
            switch (instruction)
            {
                case 0x00:
                    return new Instruction("RLC B", 2, pointer);
                case 0x01:
                    return new Instruction("RLC C", 2, pointer);
                case 0x02:
                    return new Instruction("RLC D", 2, pointer);
                case 0x03:
                    return new Instruction("RLC E", 2, pointer);
                case 0x04:
                    return new Instruction("RLC H", 2, pointer);
                case 0x05:
                    return new Instruction("RLC L", 2, pointer);
                case 0x06:
                    return new Instruction("RLC (HL)", 2, pointer);
                case 0x07:
                    return new Instruction("RLC A", 2, pointer);
                case 0x08:
                    return new Instruction("RRC B", 2, pointer);
                case 0x09:
                    return new Instruction("RRC C", 2, pointer);
                case 0x0a:
                    return new Instruction("RRC D", 2, pointer);
                case 0x0b:
                    return new Instruction("RRC E", 2, pointer);
                case 0x0c:
                    return new Instruction("RRC H", 2, pointer);
                case 0x0d:
                    return new Instruction("RRC L", 2, pointer);
                case 0x0e:
                    return new Instruction("RRC (HL)", 2, pointer);
                case 0x0f:
                    return new Instruction("RRC A", 2, pointer);
                case 0x10:
                    return new Instruction("RL B", 2, pointer);
                case 0x11:
                    return new Instruction("RL C", 2, pointer);
                case 0x12:
                    return new Instruction("RL D", 2, pointer);
                case 0x13:
                    return new Instruction("RL E", 2, pointer);
                case 0x14:
                    return new Instruction("RL H", 2, pointer);
                case 0x15:
                    return new Instruction("RL L", 2, pointer);
                case 0x16:
                    return new Instruction("RL (HL)", 2, pointer);
                case 0x17:
                    return new Instruction("RL A", 2, pointer);
                case 0x18:
                    return new Instruction("RR B", 2, pointer);
                case 0x19:
                    return new Instruction("RR C", 2, pointer);
                case 0x1a:
                    return new Instruction("RR D", 2, pointer);
                case 0x1b:
                    return new Instruction("RR E", 2, pointer);
                case 0x1c:
                    return new Instruction("RR H", 2, pointer);
                case 0x1d:
                    return new Instruction("RR L", 2, pointer);
                case 0x1e:
                    return new Instruction("RR (HL)", 2, pointer);
                case 0x1f:
                    return new Instruction("RR A", 2, pointer);
                case 0x20:
                    return new Instruction("SLA B", 2, pointer);
                case 0x21:
                    return new Instruction("SLA C", 2, pointer);
                case 0x22:
                    return new Instruction("SLA D", 2, pointer);
                case 0x23:
                    return new Instruction("SLA E", 2, pointer);
                case 0x24:
                    return new Instruction("SLA H", 2, pointer);
                case 0x25:
                    return new Instruction("SLA L", 2, pointer);
                case 0x26:
                    return new Instruction("SLA (HL)", 2, pointer);
                case 0x27:
                    return new Instruction("SLA A", 2, pointer);
                case 0x28:
                    return new Instruction("SRA B", 2, pointer);
                case 0x29:
                    return new Instruction("SRA C", 2, pointer);
                case 0x2a:
                    return new Instruction("SRA D", 2, pointer);
                case 0x2b:
                    return new Instruction("SRA E", 2, pointer);
                case 0x2c:
                    return new Instruction("SRA H", 2, pointer);
                case 0x2d:
                    return new Instruction("SRA L", 2, pointer);
                case 0x2e:
                    return new Instruction("SRA (HL)", 2, pointer);
                case 0x2f:
                    return new Instruction("SRA A", 2, pointer);
                case 0x30:
                    return new Instruction("SWAP B", 2, pointer);
                case 0x31:
                    return new Instruction("SWAP C", 2, pointer);
                case 0x32:
                    return new Instruction("SWAP D", 2, pointer);
                case 0x33:
                    return new Instruction("SWAP E", 2, pointer);
                case 0x34:
                    return new Instruction("SWAP H", 2, pointer);
                case 0x35:
                    return new Instruction("SWAP L", 2, pointer);
                case 0x36:
                    return new Instruction("SWAP (HL)", 2, pointer);
                case 0x37:
                    return new Instruction("SWAP A", 2, pointer);
                case 0x38:
                    return new Instruction("SRL B", 2, pointer);
                case 0x39:
                    return new Instruction("SRL C", 2, pointer);
                case 0x3a:
                    return new Instruction("SRL D", 2, pointer);
                case 0x3b:
                    return new Instruction("SRL E", 2, pointer);
                case 0x3c:
                    return new Instruction("SRL H", 2, pointer);
                case 0x3d:
                    return new Instruction("SRL L", 2, pointer);
                case 0x3e:
                    return new Instruction("SRL (HL)", 2, pointer);
                case 0x3f:
                    return new Instruction("SRL A", 2, pointer);
                case 0x40:
                    return new Instruction("BIT 0, B", 2, pointer);
                case 0x41:
                    return new Instruction("BIT 0, C", 2, pointer);
                case 0x42:
                    return new Instruction("BIT 0, D", 2, pointer);
                case 0x43:
                    return new Instruction("BIT 0, E", 2, pointer);
                case 0x44:
                    return new Instruction("BIT 0, H", 2, pointer);
                case 0x45:
                    return new Instruction("BIT 0, L", 2, pointer);
                case 0x46:
                    return new Instruction("BIT 0, (HL)", 2, pointer);
                case 0x47:
                    return new Instruction("BIT 0, A", 2, pointer);
                case 0x48:
                    return new Instruction("BIT 1, B", 2, pointer);
                case 0x49:
                    return new Instruction("BIT 1, C", 2, pointer);
                case 0x4a:
                    return new Instruction("BIT 1, D", 2, pointer);
                case 0x4b:
                    return new Instruction("BIT 1, E", 2, pointer);
                case 0x4c:
                    return new Instruction("BIT 1, H", 2, pointer);
                case 0x4d:
                    return new Instruction("BIT 1, L", 2, pointer);
                case 0x4e:
                    return new Instruction("BIT 1, (HL)", 2, pointer);
                case 0x4f:
                    return new Instruction("BIT 1, A", 2, pointer);
                case 0x50:
                    return new Instruction("BIT 2, B", 2, pointer);
                case 0x51:
                    return new Instruction("BIT 2, C", 2, pointer);
                case 0x52:
                    return new Instruction("BIT 2, D", 2, pointer);
                case 0x53:
                    return new Instruction("BIT 2, E", 2, pointer);
                case 0x54:
                    return new Instruction("BIT 2, H", 2, pointer);
                case 0x55:
                    return new Instruction("BIT 2, L", 2, pointer);
                case 0x56:
                    return new Instruction("BIT 2, (HL)", 2, pointer);
                case 0x57:
                    return new Instruction("BIT 2, A", 2, pointer);
                case 0x58:
                    return new Instruction("BIT 3, B", 2, pointer);
                case 0x59:
                    return new Instruction("BIT 3, C", 2, pointer);
                case 0x5a:
                    return new Instruction("BIT 3, D", 2, pointer);
                case 0x5b:
                    return new Instruction("BIT 3, E", 2, pointer);
                case 0x5c:
                    return new Instruction("BIT 3, H", 2, pointer);
                case 0x5d:
                    return new Instruction("BIT 3, L", 2, pointer);
                case 0x5e:
                    return new Instruction("BIT 3, (HL)", 2, pointer);
                case 0x5f:
                    return new Instruction("BIT 3, A", 2, pointer);
                case 0x60:
                    return new Instruction("BIT 4, B", 2, pointer);
                case 0x61:
                    return new Instruction("BIT 4, C", 2, pointer);
                case 0x62:
                    return new Instruction("BIT 4, D", 2, pointer);
                case 0x63:
                    return new Instruction("BIT 4, E", 2, pointer);
                case 0x64:
                    return new Instruction("BIT 4, H", 2, pointer);
                case 0x65:
                    return new Instruction("BIT 4, L", 2, pointer);
                case 0x66:
                    return new Instruction("BIT 4, (HL)", 2, pointer);
                case 0x67:
                    return new Instruction("BIT 4, A", 2, pointer);
                case 0x68:
                    return new Instruction("BIT 5, B", 2, pointer);
                case 0x69:
                    return new Instruction("BIT 5, C", 2, pointer);
                case 0x6a:
                    return new Instruction("BIT 5, D", 2, pointer);
                case 0x6b:
                    return new Instruction("BIT 5, E", 2, pointer);
                case 0x6c:
                    return new Instruction("BIT 5, H", 2, pointer);
                case 0x6d:
                    return new Instruction("BIT 5, L", 2, pointer);
                case 0x6e:
                    return new Instruction("BIT 5, (HL)", 2, pointer);
                case 0x6f:
                    return new Instruction("BIT 5, A", 2, pointer);
                case 0x70:
                    return new Instruction("BIT 6, B", 2, pointer);
                case 0x71:
                    return new Instruction("BIT 6, C", 2, pointer);
                case 0x72:
                    return new Instruction("BIT 6, D", 2, pointer);
                case 0x73:
                    return new Instruction("BIT 6, E", 2, pointer);
                case 0x74:
                    return new Instruction("BIT 6, H", 2, pointer);
                case 0x75:
                    return new Instruction("BIT 6, L", 2, pointer);
                case 0x76:
                    return new Instruction("BIT 6, (HL)", 2, pointer);
                case 0x77:
                    return new Instruction("BIT 6, A", 2, pointer);
                case 0x78:
                    return new Instruction("BIT 7, B", 2, pointer);
                case 0x79:
                    return new Instruction("BIT 7, C", 2, pointer);
                case 0x7a:
                    return new Instruction("BIT 7, D", 2, pointer);
                case 0x7b:
                    return new Instruction("BIT 7, E", 2, pointer);
                case 0x7c:
                    return new Instruction("BIT 7, H", 2, pointer);
                case 0x7d:
                    return new Instruction("BIT 7, L", 2, pointer);
                case 0x7e:
                    return new Instruction("BIT 7, (HL)", 2, pointer);
                case 0x7f:
                    return new Instruction("BIT 7, A", 2, pointer);
                case 0x80:
                    return new Instruction("RES 0, B", 2, pointer);
                case 0x81:
                    return new Instruction("RES 0, C", 2, pointer);
                case 0x82:
                    return new Instruction("RES 0, D", 2, pointer);
                case 0x83:
                    return new Instruction("RES 0, E", 2, pointer);
                case 0x84:
                    return new Instruction("RES 0, H", 2, pointer);
                case 0x85:
                    return new Instruction("RES 0, L", 2, pointer);
                case 0x86:
                    return new Instruction("RES 0, (HL)", 2, pointer);
                case 0x87:
                    return new Instruction("RES 0, A", 2, pointer);
                case 0x88:
                    return new Instruction("RES 1, B", 2, pointer);
                case 0x89:
                    return new Instruction("RES 1, C", 2, pointer);
                case 0x8a:
                    return new Instruction("RES 1, D", 2, pointer);
                case 0x8b:
                    return new Instruction("RES 1, E", 2, pointer);
                case 0x8c:
                    return new Instruction("RES 1, H", 2, pointer);
                case 0x8d:
                    return new Instruction("RES 1, L", 2, pointer);
                case 0x8e:
                    return new Instruction("RES 1, (HL)", 2, pointer);
                case 0x8f:
                    return new Instruction("RES 1, A", 2, pointer);
                case 0x90:
                    return new Instruction("RES 2, B", 2, pointer);
                case 0x91:
                    return new Instruction("RES 2, C", 2, pointer);
                case 0x92:
                    return new Instruction("RES 2, D", 2, pointer);
                case 0x93:
                    return new Instruction("RES 2, E", 2, pointer);
                case 0x94:
                    return new Instruction("RES 2, H", 2, pointer);
                case 0x95:
                    return new Instruction("RES 2, L", 2, pointer);
                case 0x96:
                    return new Instruction("RES 2, (HL)", 2, pointer);
                case 0x97:
                    return new Instruction("RES 2, A", 2, pointer);
                case 0x98:
                    return new Instruction("RES 3, B", 2, pointer);
                case 0x99:
                    return new Instruction("RES 3, C", 2, pointer);
                case 0x9a:
                    return new Instruction("RES 3, D", 2, pointer);
                case 0x9b:
                    return new Instruction("RES 3, E", 2, pointer);
                case 0x9c:
                    return new Instruction("RES 3, H", 2, pointer);
                case 0x9d:
                    return new Instruction("RES 3, L", 2, pointer);
                case 0x9e:
                    return new Instruction("RES 3, (HL)", 2, pointer);
                case 0x9f:
                    return new Instruction("RES 3, A", 2, pointer);
                case 0xa0:
                    return new Instruction("RES 4, B", 2, pointer);
                case 0xa1:
                    return new Instruction("RES 4, C", 2, pointer);
                case 0xa2:
                    return new Instruction("RES 4, D", 2, pointer);
                case 0xa3:
                    return new Instruction("RES 4, E", 2, pointer);
                case 0xa4:
                    return new Instruction("RES 4, H", 2, pointer);
                case 0xa5:
                    return new Instruction("RES 4, L", 2, pointer);
                case 0xa6:
                    return new Instruction("RES 4, (HL)", 2, pointer);
                case 0xa7:
                    return new Instruction("RES 4, A", 2, pointer);
                case 0xa8:
                    return new Instruction("RES 5, B", 2, pointer);
                case 0xa9:
                    return new Instruction("RES 5, C", 2, pointer);
                case 0xaa:
                    return new Instruction("RES 5, D", 2, pointer);
                case 0xab:
                    return new Instruction("RES 5, E", 2, pointer);
                case 0xac:
                    return new Instruction("RES 5, H", 2, pointer);
                case 0xad:
                    return new Instruction("RES 5, L", 2, pointer);
                case 0xae:
                    return new Instruction("RES 5, (HL)", 2, pointer);
                case 0xaf:
                    return new Instruction("RES 5, A", 2, pointer);
                case 0xb0:
                    return new Instruction("RES 6, B", 2, pointer);
                case 0xb1:
                    return new Instruction("RES 6, C", 2, pointer);
                case 0xb2:
                    return new Instruction("RES 6, D", 2, pointer);
                case 0xb3:
                    return new Instruction("RES 6, E", 2, pointer);
                case 0xb4:
                    return new Instruction("RES 6, H", 2, pointer);
                case 0xb5:
                    return new Instruction("RES 6, L", 2, pointer);
                case 0xb6:
                    return new Instruction("RES 6, (HL)", 2, pointer);
                case 0xb7:
                    return new Instruction("RES 6, A", 2, pointer);
                case 0xb8:
                    return new Instruction("RES 7, B", 2, pointer);
                case 0xb9:
                    return new Instruction("RES 7, C", 2, pointer);
                case 0xba:
                    return new Instruction("RES 7, D", 2, pointer);
                case 0xbb:
                    return new Instruction("RES 7, E", 2, pointer);
                case 0xbc:
                    return new Instruction("RES 7, H", 2, pointer);
                case 0xbd:
                    return new Instruction("RES 7, L", 2, pointer);
                case 0xbe:
                    return new Instruction("RES 7, (HL)", 2, pointer);
                case 0xbf:
                    return new Instruction("RES 7, A", 2, pointer);
                case 0xc0:
                    return new Instruction("SET 0, B", 2, pointer);
                case 0xc1:
                    return new Instruction("SET 0, C", 2, pointer);
                case 0xc2:
                    return new Instruction("SET 0, D", 2, pointer);
                case 0xc3:
                    return new Instruction("SET 0, E", 2, pointer);
                case 0xc4:
                    return new Instruction("SET 0, H", 2, pointer);
                case 0xc5:
                    return new Instruction("SET 0, L", 2, pointer);
                case 0xc6:
                    return new Instruction("SET 0, (HL)", 2, pointer);
                case 0xc7:
                    return new Instruction("SET 0, A", 2, pointer);
                case 0xc8:
                    return new Instruction("SET 1, B", 2, pointer);
                case 0xc9:
                    return new Instruction("SET 1, C", 2, pointer);
                case 0xca:
                    return new Instruction("SET 1, D", 2, pointer);
                case 0xcb:
                    return new Instruction("SET 1, E", 2, pointer);
                case 0xcc:
                    return new Instruction("SET 1, H", 2, pointer);
                case 0xcd:
                    return new Instruction("SET 1, L", 2, pointer);
                case 0xce:
                    return new Instruction("SET 1, (HL)", 2, pointer);
                case 0xcf:
                    return new Instruction("SET 1, A", 2, pointer);
                case 0xd0:
                    return new Instruction("SET 2, B", 2, pointer);
                case 0xd1:
                    return new Instruction("SET 2, C", 2, pointer);
                case 0xd2:
                    return new Instruction("SET 2, D", 2, pointer);
                case 0xd3:
                    return new Instruction("SET 2, E", 2, pointer);
                case 0xd4:
                    return new Instruction("SET 2, H", 2, pointer);
                case 0xd5:
                    return new Instruction("SET 2, L", 2, pointer);
                case 0xd6:
                    return new Instruction("SET 2, (HL)", 2, pointer);
                case 0xd7:
                    return new Instruction("SET 2, A", 2, pointer);
                case 0xd8:
                    return new Instruction("SET 3, B", 2, pointer);
                case 0xd9:
                    return new Instruction("SET 3, C", 2, pointer);
                case 0xda:
                    return new Instruction("SET 3, D", 2, pointer);
                case 0xdb:
                    return new Instruction("SET 3, E", 2, pointer);
                case 0xdc:
                    return new Instruction("SET 3, H", 2, pointer);
                case 0xdd:
                    return new Instruction("SET 3, L", 2, pointer);
                case 0xde:
                    return new Instruction("SET 3, (HL)", 2, pointer);
                case 0xdf:
                    return new Instruction("SET 3, A", 2, pointer);
                case 0xe0:
                    return new Instruction("SET 4, B", 2, pointer);
                case 0xe1:
                    return new Instruction("SET 4, C", 2, pointer);
                case 0xe2:
                    return new Instruction("SET 4, D", 2, pointer);
                case 0xe3:
                    return new Instruction("SET 4, E", 2, pointer);
                case 0xe4:
                    return new Instruction("SET 4, H", 2, pointer);
                case 0xe5:
                    return new Instruction("SET 4, L", 2, pointer);
                case 0xe6:
                    return new Instruction("SET 4, (HL)", 2, pointer);
                case 0xe7:
                    return new Instruction("SET 4, A", 2, pointer);
                case 0xe8:
                    return new Instruction("SET 5, B", 2, pointer);
                case 0xe9:
                    return new Instruction("SET 5, C", 2, pointer);
                case 0xea:
                    return new Instruction("SET 5, D", 2, pointer);
                case 0xeb:
                    return new Instruction("SET 5, E", 2, pointer);
                case 0xec:
                    return new Instruction("SET 5, H", 2, pointer);
                case 0xed:
                    return new Instruction("SET 5, L", 2, pointer);
                case 0xee:
                    return new Instruction("SET 5, (HL)", 2, pointer);
                case 0xef:
                    return new Instruction("SET 5, A", 2, pointer);
                case 0xf0:
                    return new Instruction("SET 6, B", 2, pointer);
                case 0xf1:
                    return new Instruction("SET 6, C", 2, pointer);
                case 0xf2:
                    return new Instruction("SET 6, D", 2, pointer);
                case 0xf3:
                    return new Instruction("SET 6, E", 2, pointer);
                case 0xf4:
                    return new Instruction("SET 6, H", 2, pointer);
                case 0xf5:
                    return new Instruction("SET 6, L", 2, pointer);
                case 0xf6:
                    return new Instruction("SET 6, (HL)", 2, pointer);
                case 0xf7:
                    return new Instruction("SET 6, A", 2, pointer);
                case 0xf8:
                    return new Instruction("SET 7, B", 2, pointer);
                case 0xf9:
                    return new Instruction("SET 7, C", 2, pointer);
                case 0xfa:
                    return new Instruction("SET 7, D", 2, pointer);
                case 0xfb:
                    return new Instruction("SET 7, E", 2, pointer);
                case 0xfc:
                    return new Instruction("SET 7, H", 2, pointer);
                case 0xfd:
                    return new Instruction("SET 7, L", 2, pointer);
                case 0xfe:
                    return new Instruction("SET 7, (HL)", 2, pointer);
                case 0xff:
                    return new Instruction("SET 7, A", 2, pointer);
            }

            throw new Exception();
        }
    }
}