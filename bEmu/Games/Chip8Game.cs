using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bEmu.Core;
using State = bEmu.Systems.Chip8.State;
using System.Threading;
using Chip8 = bEmu.Systems.Chip8;
using bEmu.Factory;
using bEmu.Systems;
using bEmu.Components;
using bEmu.Scalers;

namespace bEmu
{
    public class Chip8Game : BaseGame
    {
        private const int CycleCount = 16;
        private SoundEffect tone;
        private SoundEffectInstance soundEffectInstance;
        private int cycle;
        private readonly Chip8.System system;
        private readonly Chip8.PPU gpu;
        private readonly State state;
        private readonly Chip8.MMU mmu;

        public Chip8Game(string rom) : base(SystemFactory.Get(SupportedSystems.Chip8), rom, 64, 32, 10) 
        {
            system = System as Chip8.System;
            gpu = Gpu as Chip8.PPU;
            state = State as State;
            mmu = Mmu as Chip8.MMU;
        }

        protected override void Initialize()
        {
            system.MMU.LoadProgram(Rom, 0x200);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            tone = Content.Load<SoundEffect>("Chip8/tone");
            soundEffectInstance = tone.CreateInstance();

            base.LoadContent();

            //state.SuperChipMode = true;

            //if (state.SuperChipMode && BackBuffer.Width == Width)
                BackBuffer = new Texture2D(GraphicsDevice, Width, Height);

            IsRunning = true;
            StartMainThread();
        }

        protected override void Update(GameTime gameTime)
        {
            if (state.Delay > 0)
                state.Delay--;

            if (state.Sound > 0)
                state.Sound--;

            UpdateSound();
            cycle = CycleCount;
            base.Update(gameTime);
        }

        public override void UpdateGame()
        {
            lock (this)
            {
                while (cycle-- >= 0)
                {
                    system.Runner.StepCycle();

                    if (state.SuperChipMode && Width == 64)
                    {
                        Width  *= 2;
                        Height *= 2;
                        BackBuffer = new Texture2D(GraphicsDevice, Width, Height);
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

        public override void UpdateGamePad(KeyboardState keyboardState)
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

        protected override void Draw (GameTime gameTime)
		{
            SpriteBatch.Begin();
            base.Draw (gameTime);
            SpriteBatch.End();
            state.Draw = false;
		}

        protected override void OnOptionChanged(object sender, OnOptionChangedEventArgs e)
        {
            base.OnOptionChanged(sender, e);
        }
    }
}