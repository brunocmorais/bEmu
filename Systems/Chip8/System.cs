using System;
using System.IO;
using System.Linq;
using bEmu.Core;

namespace bEmu.Systems.Chip8
{
    public class System : Core.System
    {
        public override int Width => 128; 
        public override int Height => 64;
        public override int RefreshRate => 16;
        public override int CycleCount => 8;

        public void SetSuperChipMode()
        {
            (State as State).SuperChipMode = true;
        }

        public void SetChip8Mode()
        {
            (State as State).SuperChipMode = false;
        }

        public override IState GetInitialState()
        {
            var state = new State(this);
            state.PC = 0x200;
            state.V = new byte[16];
            state.Stack = new ushort[16];
            state.GamePad = new GamePad();
            state.R = new byte[8];

            return state;
        }

        public override void Initialize()
        {
            MMU = new MMU(this);
            base.Initialize();
            SetChip8Mode();
            PPU = new PPU((State) State, Width, Height);
            Runner = new Core.VMs.Chip8.Chip8(this);
            InitializeBIOS();
        }

        public System(string fileName) : base(fileName)
        {
        }

        public override void Reset()
        {
            base.Reset();

            InitializeBIOS();
        }

        private void InitializeBIOS()
        {
            for (int i = 0; i < BIOS.Numbers.Length; i++)
                MMU[i] = BIOS.Numbers[i];

            for (int i = 0; i < BIOS.NumbersHiRes.Length; i++)
                MMU[i + 0x50] = BIOS.NumbersHiRes[i];
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

        public override void Update()
        {
            var state = (Systems.Chip8.State) State;

            if (state.Delay > 0)
                state.Delay--;

            if (state.Sound > 0)
                state.Sound--;

            while (Cycles >= 0)
            {
                var opcode = Runner.StepCycle();
                Cycles -= opcode.CyclesTaken;
            }
        }

        public override void Stop() { }

        public override void UpdateGamePad(IGamePad gamePad)
        {
            var state = (State) State;
            state.GamePad = (GamePad) gamePad;
        }
    }
}