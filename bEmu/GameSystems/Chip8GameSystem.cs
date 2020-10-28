using bEmu.Components;
using bEmu.Core;
using bEmu.Systems;
using bEmu.Systems.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{

    public class Chip8GameSystem : IGameSystem
    {
        private const int CycleCount = 16;
        public SupportedSystems Type => SupportedSystems.Chip8;
        private SoundEffect tone;
        private SoundEffectInstance soundEffectInstance;
        private int cycle;
        public ISystem System { get; }
        private readonly Systems.Chip8.PPU gpu;
        private readonly Systems.Chip8.State state;
        private readonly Systems.Chip8.MMU mmu;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public IMainGame MainGame { get; }
        public int Frame { get => gpu.Frame; set => gpu.Frame = value; }
        public int Frameskip { get => gpu.Frameskip; set => gpu.Frameskip = value; }
        public Framebuffer Framebuffer { get => gpu.Framebuffer; }

        public int RefreshRate => 16;

        public Chip8GameSystem(IMainGame mainGame, string rom)
        {
            System = SystemFactory.Get(SupportedSystems.Chip8, rom) as Systems.Chip8.System;
            gpu = System.PPU as Systems.Chip8.PPU;
            state = System.State as Systems.Chip8.State;
            mmu = System.MMU as Systems.Chip8.MMU;
            MainGame = mainGame;
            MainGame.Options = new Options(MainGame);
            Width = 64;
            Height = 32;
            MainGame.Options.Size = 10;
        }

        public void Initialize()
        {
            System.MMU.LoadProgram(0x200);
        }

        public void LoadContent()
        {
            tone = MainGame.Content.Load<SoundEffect>("Chip8/tone");
            soundEffectInstance = tone.CreateInstance();
        }

        public void Update(GameTime gameTime)
        {
            if (state.Delay > 0)
                state.Delay--;

            if (state.Sound > 0)
                state.Sound--;

            UpdateSound();
            cycle = CycleCount;
        }

        public void UpdateGame()
        {
            lock (this)
            {
                while (cycle-- >= 0)
                {
                    System.Runner.StepCycle();

                    if (state.SuperChipMode && Width == 64)
                    {
                        Width *= 2;
                        Height *= 2;
                        MainGame.Options.Size /= 2;
                        MainGame.BackBuffer = new Texture2D(MainGame.GraphicsDevice, Width, Height);
                    }
                }
            }
        }

        private void UpdateSound()
        {
            if (state.Sound == 0)
            {
                soundEffectInstance.IsLooped = false;
                soundEffectInstance.Stop();
            }
            else if (state.Sound > 0 && !soundEffectInstance.IsLooped)
            {
                soundEffectInstance.IsLooped = true;
                soundEffectInstance.Play();
            }
        }

        public void UpdateGamePad(KeyboardState keyboardState)
        {
            var keys = new Keys[]
            {
                Keys.D1, Keys.D2, Keys.D3, Keys.D4,
                Keys.Q, Keys.W, Keys.E, Keys.R,
                Keys.A, Keys.S, Keys.D, Keys.F,
                Keys.Z, Keys.X, Keys.C, Keys.V
            };

            var keyboard = new int[] {
                0x1, 0x2, 0x3, 0xC,
                0x4, 0x5, 0x6, 0xD,
                0x7, 0x8, 0x9, 0xE,
                0xA, 0x0, 0xB, 0xF
            };

            for (int i = 0; i < keys.Length; i++)
                state.Keys[keyboard[i]] = keyboardState.IsKeyDown(keys[i]);
        }

        public void Draw(GameTime gameTime) 
        { 
            state.Draw = false;
        }

        public void StopGame() { }
    }
}