using System;
using System.Linq;
using System.Threading;
using bEmu.Classes;
using bEmu.Components;
using bEmu.Core;
using bEmu.Extensions;
using bEmu.Scalers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{

    public abstract class BaseGame : Game, IBaseGame 
    {
        protected int PixelSize { get; }
        protected int Width { get; }
        protected int Height { get; }
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch SpriteBatch { get; set; }
        protected int lastRenderedFrame;
        protected Thread thread;
        protected Fonts fonts;
        protected Rectangle destinationRectangle;
        protected OSD osd;
        protected Texture2D BackBuffer { get; set; }
        protected string Rom { get; }
        protected int DrawCounter { get; private set; }
        public bool IsRunning { get; set; }
        public Options Options { get; set; }
        public ISystem System { get; }
        public IPPU Gpu => System.PPU;
        public IState State => System.State;
        public IMMU Mmu => System.MMU;
        public IAPU Apu => System.APU;
        public DateTime LastStartDate { get; private set; }
        public MainMenu MainMenu { get; private set; }
        public IScaler Scaler { get; protected set; }

        public BaseGame(ISystem system, string rom, int width, int height, int pixelSize)
        {
            System = system;
            this.Width = width;
            this.Height = height;
            this.PixelSize = pixelSize;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = this.Width * this.PixelSize;
            graphics.PreferredBackBufferHeight = this.Height * this.PixelSize;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Rom = rom;
            IsRunning = false;
            DrawCounter = 0;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            fonts = new Fonts();
            fonts.Regular = Content.Load<SpriteFont>("Common/Regular");
            fonts.Title = Content.Load<SpriteFont>("Common/Title");
            Scaler = new EagleScaler(Gpu.Framebuffer);
            BackBuffer = new Texture2D(GraphicsDevice, Width * Scaler.ScaleFactor, Height * Scaler.ScaleFactor);
            destinationRectangle = new Rectangle(0, 0, Width * PixelSize, Height * PixelSize);
            osd = new OSD(this, SpriteBatch, fonts.Regular);
            MainMenu = new MainMenu(this, SpriteBatch, fonts, Width * PixelSize, Height * PixelSize);
            base.LoadContent();
        }

        public void StartMainThread()
        {
            thread = new Thread(() =>
            {
                while (IsRunning)
                    UpdateGame();
            });
            
            thread.Start();
            LastStartDate = DateTime.Now;
            Gpu.Frame = 0;
            lastRenderedFrame = 0;
            DrawCounter = 0;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardStateExtensions.UpdateState();
            MainMenu.Update();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Escape)) // sair
                StopGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F3)) // reiniciar jogo
            {
                IsRunning = false;
                Initialize();
            }

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F1)) // abrir menu
            {
                MainMenu.IsOpen = !MainMenu.IsOpen;

                if ((MainMenu.IsOpen && IsRunning) || (!MainMenu.IsOpen && !IsRunning))
                    PauseResumeGame();
            }

            if (KeyboardStateExtensions.HasBeenPressed(Keys.P)) // pausar
                PauseResumeGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F2)) // mostrar informações
                Options.ShowFPS = !Options.ShowFPS;

            UpdateGamePad(keyboardState);
            base.Update(gameTime);
        }

        private void PauseResumeGame()
        {
            IsRunning = !IsRunning;

            if (IsRunning)
                StartMainThread();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (Gpu.Frame > lastRenderedFrame)
            {
                Scaler.Update();
                BackBuffer.SetData(Scaler.Scaled.Data);
                //BackBuffer.SetData(Gpu.Framebuffer.Data);
            }

            lastRenderedFrame = Gpu.Frame;

            SpriteBatch.Draw(BackBuffer, destinationRectangle, Color.White);
            MainMenu.Draw();
            osd.Draw();

            if (IsRunning)
                DrawCounter++;

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            IsRunning = false;
        }

        public virtual void StopGame()
        {
            Exit();
        }

        public abstract void UpdateGame();
        public abstract void UpdateGamePad(KeyboardState keyboardState);
        protected virtual void OnOptionChanged(object sender, OnOptionChangedEventArgs e)
        {
            switch (e.Property)
            {
                case "Frameskip":
                    Gpu.Frameskip = Options.Frameskip;
                    break;
            }
        }
    }
}