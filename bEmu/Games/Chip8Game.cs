using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bEmu.Core;
using State = bEmu.Core.Systems.Chip8.State;

namespace bEmu
{
    public class Chip8Game : Game
    {
        private const int TamanhoPixel = 10;
        private const int Width = 64;
        private const int Height = 32;
        private const int CycleCount = 15;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SoundEffect tone;
        private SoundEffectInstance soundEffectInstance;
        private Texture2D backBuffer;
        private Core.Systems.Chip8.System system;
        private string rom;
        private Rectangle destinationRectangle;
        private State State => system.State as State;
        
        public Chip8Game(string rom)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Width * TamanhoPixel;
            graphics.PreferredBackBufferHeight = Height * TamanhoPixel;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.rom = rom;
            destinationRectangle = new Rectangle(0, 0, Width * TamanhoPixel, Height * TamanhoPixel);
        }

        protected override void Initialize()
        {
            system = new Core.Systems.Chip8.System();
            system.MMU.LoadProgram(rom, 0x200);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tone = Content.Load<SoundEffect>("Chip8/tone");
            soundEffectInstance = tone.CreateInstance();
            backBuffer = new Texture2D(GraphicsDevice, Width, Height);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || State.Halted)
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                Initialize();

            if (State.SuperChipMode && backBuffer.Width == Width)
                backBuffer = new Texture2D(GraphicsDevice, Width * 2, Height * 2);

            int cycle = CycleCount;

            while (cycle-- >= 0)
                system.Runner.StepCycle();

            if (State.Delay > 0)
                State.Delay--;
            
            if (State.Sound > 0)
                State.Sound--;

            UpdateKeys(Keyboard.GetState(), State);
            UpdateSound(State);

            base.Update(gameTime);
        }

        private void UpdateSound(State state)
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

        private void UpdateKeys(KeyboardState keyboardState, State state)
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
            if (!State.Draw)
                return;

			GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin ();

            backBuffer.SetData(system.PPU.FrameBuffer);
            spriteBatch.Draw(backBuffer, destinationRectangle, Color.White);

            spriteBatch.End ();
            State.Draw = false;

            base.Draw (gameTime);
		}
    }
}