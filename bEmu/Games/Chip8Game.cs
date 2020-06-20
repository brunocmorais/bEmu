using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bEmu.Core;
using State = bEmu.Core.Systems.Chip8.State;
using System.Threading;
using Chip8 = bEmu.Core.Systems.Chip8;

namespace bEmu
{
    public class Chip8Game : BaseGame<Chip8.System, State, MMU, Chip8.PPU, APU>
    {
        private const int CycleCount = 16;
        private SoundEffect tone;
        private SoundEffectInstance soundEffectInstance;
        private int cycle;

        public Chip8Game(string rom) : base(new Chip8.System(), rom, 64, 32, 10) { }

        protected override void Initialize()
        {
            System.MMU.LoadProgram(Rom, 0x200);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            tone = Content.Load<SoundEffect>("Chip8/tone");
            soundEffectInstance = tone.CreateInstance();

            base.LoadContent();

            State.SuperChipMode = true;

            if (State.SuperChipMode && BackBuffer.Width == Width)
                BackBuffer = new Texture2D(GraphicsDevice, Width * 2, Height * 2);
            
            IsRunning = true;
            StartMainThread();
        }

        protected override void Update(GameTime gameTime)
        {
            if (State.Delay > 0)
                State.Delay--;

            if (State.Sound > 0)
                State.Sound--;

            UpdateSound();
            cycle = CycleCount;
            base.Update(gameTime);
        }

        public override void UpdateGame()
        {
            lock (this)
            {
                while (cycle-- >= 0)
                    System.Runner.StepCycle();
            }
        }

        private void UpdateSound()
        {
            if (State.Sound == 0)
            {
                soundEffectInstance.IsLooped = false;
                soundEffectInstance.Stop();
            }
            else if (State.Sound > 0 && !soundEffectInstance.IsLooped)
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
                State.Keys[keyboard[i]] = keyboardState.IsKeyDown(keys[i]);
        }

        protected override void Draw (GameTime gameTime)
		{
            base.Draw (gameTime);
            State.Draw = false;
		}
    }
}