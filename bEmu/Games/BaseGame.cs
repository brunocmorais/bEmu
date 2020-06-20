using System;
using System.Linq;
using System.Threading;
using bEmu.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{

    public abstract class BaseGame<TSystem, TState, TMMU, TGPU, TAPU> : Game, IBaseGame 
        where TSystem : ISystem
        where TState : IState
        where TMMU : IMMU
        where TGPU : IPPU
        where TAPU : IAPU
    {
        protected int PixelSize { get; }
        protected int Width { get; }
        protected int Height { get; }
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        protected bool IsRunning { get; set; }
        private Keys[] frameskipKeys;
        private int lastRenderedFrame;
        private Thread thread;
        private SpriteFont font;
        private Rectangle destinationRectangle;
        private OSD osd;
        protected Texture2D BackBuffer { get; set; }
        protected string Rom { get; }
        protected int DrawCounter { get; private set; }
        protected TSystem System { get; }
        protected TGPU Gpu { get; }
        protected TState State { get; }
        protected TMMU Mmu { get; }
        protected TAPU Apu { get; }

        public BaseGame(TSystem system, string rom, int width, int height, int pixelSize)
        {
            System = system;
            State = (TState) System.State;
            Mmu = (TMMU) System.MMU;
            Gpu = (TGPU) System.PPU;
            Apu = (TAPU) System.APU;
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
            frameskipKeys = new Keys[]
            {
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            };
            DrawCounter = 0;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Common/Font");
            BackBuffer = new Texture2D(GraphicsDevice, Width, Height);
            destinationRectangle = new Rectangle(0, 0, Width * PixelSize, Height * PixelSize);
            osd = new OSD(System, spriteBatch, font);

            thread = new Thread(() =>
            {
                while (IsRunning)
                    UpdateGame();
            });

            base.LoadContent();
        }

        public void StartMainThread()
        {
            thread.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardStateExtensions.UpdateState();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Escape))
                StopGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F1))
            {
                IsRunning = false;
                Initialize();
            }

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F2))
                osd.ShowFPS = !osd.ShowFPS;

            if (frameskipKeys.Any(x => KeyboardStateExtensions.HasBeenPressed(x)))
                Gpu.Frameskip = (int)(frameskipKeys.First(x => KeyboardStateExtensions.HasBeenPressed(x))) - 48;

            UpdateGamePad(keyboardState);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (Gpu.Frame > lastRenderedFrame)
                BackBuffer.SetData(Gpu.FrameBuffer);

            lastRenderedFrame = Gpu.Frame;
            
            spriteBatch.Begin();
            spriteBatch.Draw(BackBuffer, destinationRectangle, Color.White);
            osd.Draw(gameTime);
            spriteBatch.End();
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
    }
}