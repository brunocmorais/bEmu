using System;
using System.IO;
using System.Linq;
using bEmu.Core;

namespace bEmu.Systems.Chip8
{
    public class System : Core.System
    {
        public void SetSuperChipMode()
        {
            State state = (State as State);
            state.SuperChipMode = true;
            PPU.DefineSize(128, 64);
            state.R = new byte[8];
        }

        public void SetChip8Mode()
        {
            State state = (State as State);
            state.SuperChipMode = false;
            PPU = new PPU(state, 64, 32);
        }

        public override IState GetInitialState()
        {
            var state = new State(this);
            state.PC = 0x200;
            state.V = new byte[16];
            state.Stack = new ushort[16];
            state.Keys = new bool[16];
            state.R = new byte[8];

            return state;
        }

        public override void Initialize()
        {
            MMU = new MMU(this);
            base.Initialize();
            SetChip8Mode();
            Runner = new Core.VMs.Chip8.Chip8(this);

            for (int i = 0; i < Numbers.Length; i++)
                MMU[i] = Numbers[i];

            for (int i = 0; i < NumbersHiRes.Length; i++)
                MMU[i + 0x50] = NumbersHiRes[i];
        }

        public readonly byte[] Numbers = 
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, //0
            0x20, 0x60, 0x20, 0x20, 0x70, //1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, //2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, //3
            0x90, 0x90, 0xF0, 0x10, 0x10, //4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, //5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, //6
            0xF0, 0x10, 0x20, 0x40, 0x40, //7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, //8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, //9
            0xF0, 0x90, 0xF0, 0x90, 0x90, //A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, //B
            0xF0, 0x80, 0x80, 0x80, 0xF0, //C
            0xE0, 0x90, 0x90, 0x90, 0xE0, //D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, //E
            0xF0, 0x80, 0xF0, 0x80, 0x80  //F
        };

        public readonly byte[] NumbersHiRes = 
        {
            0x3C, 0x7E, 0xE7, 0xC3, 0xC3, 0xC3, 0xC3, 0xE7, 0x7E, 0x3C, // "0"
            0x18, 0x38, 0x58, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x3C, // "1"
            0x3E, 0x7F, 0xC3, 0x06, 0x0C, 0x18, 0x30, 0x60, 0xFF, 0xFF, // "2"
            0x3C, 0x7E, 0xC3, 0x03, 0x0E, 0x0E, 0x03, 0xC3, 0x7E, 0x3C, // "3"
            0x06, 0x0E, 0x1E, 0x36, 0x66, 0xC6, 0xFF, 0xFF, 0x06, 0x06, // "4"
            0xFF, 0xFF, 0xC0, 0xC0, 0xFC, 0xFE, 0x03, 0xC3, 0x7E, 0x3C, // "5"
            0x3E, 0x7C, 0xC0, 0xC0, 0xFC, 0xFE, 0xC3, 0xC3, 0x7E, 0x3C, // "6"
            0xFF, 0xFF, 0x03, 0x06, 0x0C, 0x18, 0x30, 0x60, 0x60, 0x60, // "7"
            0x3C, 0x7E, 0xC3, 0xC3, 0x7E, 0x7E, 0xC3, 0xC3, 0x7E, 0x3C, // "8"
            0x3C, 0x7E, 0xC3, 0xC3, 0x7F, 0x3F, 0x03, 0x03, 0x3E, 0x7C  // "9"
        };

        public System(string fileName) : base(fileName)
        {
        }

        public override void Reset()
        {
            base.Reset();
            
            for (int i = 0; i < Numbers.Length; i++)
                MMU[i] = Numbers[i];

            for (int i = 0; i < NumbersHiRes.Length; i++)
                MMU[i + 0x50] = NumbersHiRes[i];
        }

        public override bool LoadState()
        {
            if (!File.Exists(SaveStateName))
                return false;

            byte[] state = File.ReadAllBytes(SaveStateName);
            State.LoadState(state);

            byte[] framebuffer = state.TakeLast(PPU.Framebuffer.Data.Length).ToArray();
            PPU.Framebuffer.SetData(framebuffer);

            byte[] memory = new byte[MMU.Length];
            Array.Copy(state, state.Length - framebuffer.Length - memory.Length, memory, 0, memory.Length);
            MMU.LoadState(memory);

            return true;
        }

        public override void SaveState()
        {
            byte[] state = State.SaveState();
            byte[] mmu = MMU.SaveState();
            byte[] ppu = PPU.Framebuffer.Data;

            File.WriteAllBytes(SaveStateName, Enumerable.Concat(state, mmu).Concat(ppu).ToArray());
        }
    }
}