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
        private int tamanhoPixel;
        private int width;
        private int height;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private bool isRunning;
        private Keys[] frameskipKeys;
        private int lastRenderedFrame;
        private Thread thread;
        private SpriteFont font;
        private Texture2D backBuffer;
        private Rectangle destinationRectangle;
        private OSD osd;
        protected string Rom { get; }
        protected int DrawCounter { get; private set; }
        protected TSystem System { get; }
        protected TGPU Gpu { get; }
        protected TState State { get; }
        protected TMMU Mmu { get; }
        protected TAPU Apu { get; }

        public BaseGame(TSystem system, string rom, int width, int height, int tamanhoPixel)
        {
            System = system;
            State = (TState) System.State;
            Mmu = (TMMU) System.MMU;
            Gpu = (TGPU) System.PPU;
            Apu = (TAPU) System.APU;
            this.width = width;
            this.height = height;
            this.tamanhoPixel = tamanhoPixel;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = this.width * this.tamanhoPixel;
            graphics.PreferredBackBufferHeight = this.height * this.tamanhoPixel;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Rom = rom;
            isRunning = false;
            frameskipKeys = new Keys[]
            {
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            };
            DrawCounter = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Common/Font");
            backBuffer = new Texture2D(GraphicsDevice, width, height);
            destinationRectangle = new Rectangle(0, 0, width * tamanhoPixel, height * tamanhoPixel);
            osd = new OSD(System, spriteBatch, font);

            isRunning = true;

            thread = new Thread(() =>
            {
                while (isRunning)
                    UpdateGame();
            });

            thread.Start();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardStateExtensions.UpdateState();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Escape))
                StopGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F1))
            {
                isRunning = false;
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
                backBuffer.SetData(Gpu.FrameBuffer);

            lastRenderedFrame = Gpu.Frame;
            
            spriteBatch.Begin();
            spriteBatch.Draw(backBuffer, destinationRectangle, Color.White);
            osd.Draw(gameTime);
            spriteBatch.End();
            DrawCounter++;

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            isRunning = false;
        }

        public abstract void UpdateGame();
        public abstract void UpdateGamePad(KeyboardState keyboardState);
        public abstract void StopGame();
    }
}