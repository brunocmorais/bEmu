using System;
using System.Threading;
using bEmu.Classes;
using bEmu.Components;
using bEmu.Extensions;
using bEmu.Factory;
using bEmu.Core.Scalers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bEmu.Core.Factory;
using bEmu.Systems;
using bEmu.GameSystems;

namespace bEmu
{

    public class MainGame : Game, IMainGame
    {
        private GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch { get; private set; }
        private int lastRenderedFrame;
        private Thread mainThread;
        private Rectangle destinationRectangle;
        public Fonts Fonts { get; private set; }
        public OSD Osd { get; private set; }
        public GameMenu Menu { get; }
        public Texture2D BackBuffer { get; set; }
        public int DrawCounter { get; private set; }
        public bool IsRunning { get; set; }
        public Options Options { get; set; }
        public DateTime LastStartDate { get; private set; }
        public IScaler Scaler { get; private set; }
        public IGameSystem GameSystem { get; private set; }
        public double FPS => Math.Round(GameSystem.Frame / (DateTime.Now - LastStartDate).TotalSeconds, 1);

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsRunning = false;
            DrawCounter = 0;
            Menu = new GameMenu(this);
        }

        protected override void LoadContent()
        {
            Fonts = new Fonts();
            Fonts.Regular = Content.Load<SpriteFont>("Common/Regular");
            Fonts.Title = Content.Load<SpriteFont>("Common/Title");
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Osd = new OSD(this);
            LoadGameSystem(GameSystemFactory.GetDummyGameSystem(this));
            Menu.OpenMenu(new MainMenu(this));
        }

        private void LoadGameSystem(IGameSystem gameSystem)
        {
            GameSystem = gameSystem;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, GameSystem.RefreshRate);

            SetScaler();
            SetScreenSize(); 

            GameSystem.Initialize();
            GameSystem.LoadContent();
        }

        public void SetScreenSize()
        {
            int width = GameSystem.Width * Options.Size;
            int height = GameSystem.Height * Options.Size;
            
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            destinationRectangle = new Rectangle(0, 0, width, height);
            graphics.ApplyChanges();
        }

        private void StartMainThread()
        {
            mainThread = new Thread(() =>
            {
                while (IsRunning)
                    GameSystem.UpdateGame();
            });
            
            LastStartDate = DateTime.Now;
            GameSystem.Frame = 0;
            lastRenderedFrame = 0;
            DrawCounter = 0;
            mainThread.Start();
        }

        public void LoadGame(SupportedSystems system, string file)
        {
            Menu.CloseAll();

            LoadGameSystem(GameSystemFactory.Get(system, this, file));

            IsRunning = true;
            StartMainThread();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardStateExtensions.UpdateState(gameTime);

            Menu.Update(gameTime);

            UpdateGamePad(keyboardState);
            UpdateMessages();

            GameSystem.Update(gameTime);
        }

        public void Pause()
        {
            IsRunning = !IsRunning;

            if (IsRunning)
                StartMainThread();
            else
            {
                while (mainThread.IsAlive)
                    Thread.Sleep(50);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (GameSystem.Frame > lastRenderedFrame)
            {
                Scaler.Update();
                BackBuffer.SetData(Scaler.ScaledFramebuffer.Data);
                lastRenderedFrame = GameSystem.Frame;
            }

            SpriteBatch.Begin();
            SpriteBatch.Draw(BackBuffer, destinationRectangle, Color.White);

            if (Menu.IsOpen)
                Menu.Current.Draw();

            Osd.Draw();

            if (IsRunning)
                DrawCounter++;

            SpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            IsRunning = false;
        }

        public virtual void StopGame()
        {
            GameSystem.StopGame();
            Exit();
        }

        public virtual void UpdateGamePad(KeyboardState keyboardState)
        {
            GameSystem.UpdateGamePad(keyboardState);
        }

        public void SetScaler()
        {
            Scaler = ScalerFactory.Get(Options.Scaler, Options.Size);
            Scaler.Framebuffer = GameSystem.Framebuffer;
            Scaler.Update();
            BackBuffer = new Texture2D(GraphicsDevice, GameSystem.Width * Scaler.ScaleFactor, GameSystem.Height * Scaler.ScaleFactor);
            BackBuffer.SetData(Scaler.ScaledFramebuffer.Data);
        }

        private void UpdateMessages()
        {
            Osd.Update();

            if (Options.ShowFPS && IsRunning)
                Osd.UpdateMessage(MessageType.FPS, $"{FPS:0.0} fps");
        }

        public virtual void ResetGame()
        {
            if (GameSystem is DummyGameSystem)
                return;

            IsRunning = false;

            while (mainThread.IsAlive)
                Thread.Sleep(50);

            Osd.InsertMessage(MessageType.Default, "Jogo reiniciado");
            LoadGame(GameSystem.Type, GameSystem.System.FileName);

            Menu.CloseAll();
        }

        public virtual void CloseGame()
        {
            Osd.InsertMessage(MessageType.Default, "Jogo fechado");
            LoadGameSystem(GameSystemFactory.GetDummyGameSystem(this));
        }

        public void LoadState()
        {
            if (GameSystem is DummyGameSystem)
                return;

            bool success = GameSystem.System.LoadState();

            if (success)
                Osd.InsertMessage(MessageType.Default, "Estado carregado");
            else
                Osd.InsertMessage(MessageType.Default, "Nenhum estado encontrado");

            Menu.CloseAll();
        }

        public void SaveState()
        {
            if (GameSystem is DummyGameSystem)
                return;

            GameSystem.System.SaveState();
            Osd.InsertMessage(MessageType.Default, "Estado salvo");

            Menu.CloseAll();
        }
    }
}