using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.Chip8
{
    public class Chip8Game : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D whiteRect;
        Texture2D whiteRectHiRes;
        SoundEffect tone;
        SoundEffectInstance soundEffectInstance;
        CPU cpu;
        const int tamanhoPixel = 10;
        const int tamanhoPixelHiRes = tamanhoPixel / 2;
        const int width = 64 * tamanhoPixel;
        const int height = 32 * tamanhoPixel;
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
            cpu = new CPU();
            cpu.State.LoadProgram(File.ReadAllBytes(rom));

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

            tone = Content.Load<SoundEffect>("tone");
            soundEffectInstance = tone.CreateInstance();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || cpu.State.Quit)
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                Initialize();

            if (cpu.State.Delay > 0)
                cpu.State.Delay--;
            
            if (cpu.State.Sound > 0)
                cpu.State.Sound--;

            UpdateKeys(Keyboard.GetState());
            UpdateSound();

            base.Update(gameTime);
        }

        private void UpdateSound()
        {
            if (cpu.State.Sound == 0)
            {
                soundEffectInstance.IsLooped = false;
                soundEffectInstance.Stop();
            }
            else if (cpu.State.Sound > 0 && !soundEffectInstance.IsLooped)
            {
                soundEffectInstance.IsLooped = true;
                soundEffectInstance.Play();
            }
        }

        private void UpdateKeys(KeyboardState keyboardState)
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
                cpu.State.Keys[keyboard[i]] = keyboardState.IsKeyDown(keys[i]);
        }

        protected override void Draw (GameTime gameTime)
		{
            int cycle = 10;

            while (cycle-- >= 0)
                cpu.StepCycle();

            if (!cpu.State.Draw)
                return;

            base.Draw (gameTime);

			GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin ();

            var texture = cpu.State.SuperChipMode ? whiteRectHiRes : whiteRect;
            int pixelSize = cpu.State.SuperChipMode ? tamanhoPixelHiRes : tamanhoPixel;
            
			for (int i = 0; i < cpu.State.Gfx.GetLength(0); i++) 
				for (int j = 0; j < cpu.State.Gfx.GetLength(1); j++)
                    if (cpu.State.Gfx[i, j]) 
                        spriteBatch.Draw (texture, new Vector2(i * pixelSize, j * pixelSize), Color.White);

			spriteBatch.End ();
            cpu.State.Draw = false;
		}
    }
}