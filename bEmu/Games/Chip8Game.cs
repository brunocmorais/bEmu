using System;
using System.IO;
using bEmu.Core.VMs.Chip8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{
    public class Chip8Game : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D whiteRect;
        Texture2D whiteRectHiRes;
        SoundEffect tone;
        SoundEffectInstance soundEffectInstance;
        Chip8 interpreter;
        const int tamanhoPixel = 10;
        const int tamanhoPixelHiRes = tamanhoPixel / 2;
        const int width = 64 * tamanhoPixel;
        const int height = 32 * tamanhoPixel;
        private const int CycleCount = 10;
        string rom;
        
        public Chip8Game(string rom)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.rom = rom;
        }

        protected override void Initialize()
        {
            interpreter = new Chip8();
            interpreter.State.LoadProgram(rom, 0x200);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            whiteRect = new Texture2D(GraphicsDevice, tamanhoPixel, tamanhoPixel);
            whiteRectHiRes = new Texture2D(GraphicsDevice, tamanhoPixelHiRes, tamanhoPixelHiRes);

            Color[] whiteColor = new Color[tamanhoPixel * tamanhoPixel];
            Color[] whiteColorHiRes = new Color[tamanhoPixelHiRes * tamanhoPixelHiRes];
            
            for(int i = 0; i < whiteColor.Length; i++) 
                whiteColor[i] = Color.White;

            for(int i = 0; i < whiteColorHiRes.Length; i++) 
                whiteColorHiRes[i] = Color.White;
            
            whiteRect.SetData(whiteColor);
            whiteRectHiRes.SetData(whiteColorHiRes);

            tone = Content.Load<SoundEffect>("Chip8/tone");
            soundEffectInstance = tone.CreateInstance();
        }

        protected override void Update(GameTime gameTime)
        {
            var state = interpreter.State as State;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || state.Halted)
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                Initialize();

            int cycle = CycleCount;

            while (cycle-- >= 0)
                interpreter.StepCycle();

            if (state.Delay > 0)
                state.Delay--;
            
            if (state.Sound > 0)
                state.Sound--;

            UpdateKeys(Keyboard.GetState(), state);
            UpdateSound(state);

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
            var state = interpreter.State as State;

            if (!state.Draw)
                return;

			GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin ();

            var texture = state.SuperChipMode ? whiteRectHiRes : whiteRect;
            int pixelSize = state.SuperChipMode ? tamanhoPixelHiRes : tamanhoPixel;
            
			for (int i = 0; i < state.Gfx.GetLength(0); i++) 
				for (int j = 0; j < state.Gfx.GetLength(1); j++)
                    if (state.Gfx[i, j]) 
                        spriteBatch.Draw (texture, new Vector2(i * pixelSize, j * pixelSize), Color.White);

			spriteBatch.End ();
            state.Draw = false;

            base.Draw (gameTime);
		}
    }
}