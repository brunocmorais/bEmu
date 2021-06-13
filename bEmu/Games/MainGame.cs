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
using System.Threading.Tasks;

namespace bEmu
{

    public class MainGame : Game, IMainGame
    {
        public SpriteBatch SpriteBatch { get; private set; }
        public Fonts Fonts { get; private set; }
        public OSD Osd { get; private set; }
        public GameMenu Menu { get; private set; }
        public Texture2D BackBuffer { get; set; }
        public int DrawCounter { get; private set; }
        public bool IsRunning { get; set; }
        public Options Options { get; set; }
        public DateTime LastStartDate { get; private set; }
        public IScaler Scaler { get; private set; }
        public IGameSystem GameSystem { get; private set; }
        public int LastRenderedFrame { get; private set; }
        private GraphicsDeviceManager graphics;
        private Rectangle destinationRectangle;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsRunning = false;
            DrawCounter = 0;
        }

        protected override void LoadContent()
        {
            Fonts = new Fonts();
            Fonts.Regular = Content.Load<SpriteFont>("Common/Regular");
            Fonts.Title = Content.Load<SpriteFont>("Common/Title");
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Osd = new OSD(this);

            LoadGameSystem(GameSystemFactory.GetEmptyGameSystem(this));
            Generic8080ContentProvider.Initialize(this);
            Chip8ContentProvider.Initialize(this);
            
            Menu = new GameMenu(this);
            Menu.OpenMainMenu();
        }

        private void LoadGameSystem(IGameSystem gameSystem)
        {
            GameSystem = gameSystem;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, GameSystem.System.RefreshRate);

            SetScaler();
            SetScreenSize(); 

            GameSystem.Initialize(0);
        }

        public void SetScreenSize()
        {
            int width = GameSystem.System.Width * Options.Size;
            int height = GameSystem.System.Height * Options.Size;
            
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            destinationRectangle = new Rectangle(0, 0, width, height);
            graphics.ApplyChanges();
        }

        public void LoadGame(SupportedSystems system, string file)
        {
            LoadGameSystem(GameSystemFactory.Get(system, this, file));
            Menu.CloseAll();
            Start();
        }

        private void Start()
        {
            IsRunning = true;
            LastStartDate = DateTime.Now;
            GameSystem.System.PPU.Frame = 0;
            LastRenderedFrame = 0;
            DrawCounter = 0;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardStateExtensions.UpdateState(gameTime);

            Menu.Update(gameTime);

            if (!Menu.IsOpen)
                UpdateGamePad(keyboardState);
                
            UpdateMessages();

            GameSystem.Update(gameTime);
        }

        public void Pause()
        {
            IsRunning = !IsRunning;

            if (IsRunning)
                Start();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (BackBuffer == null)
                return;

            SpriteBatch.Begin();

            if (IsRunning)
            {
                DrawCounter++;

                if (GameSystem.System.PPU.Frame > LastRenderedFrame)
                {
                    Scaler.Update(GameSystem.System.PPU.Frame);
                    BackBuffer.SetData(Scaler.ScaledFramebuffer.Data);
                    LastRenderedFrame = GameSystem.System.PPU.Frame;
                }
            }

            SpriteBatch.Draw(BackBuffer, destinationRectangle, Color.White);

            if (Menu.IsOpen)
                Menu.Current.Draw();

            Osd.Draw();
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
            Scaler.Framebuffer = GameSystem.System.PPU.Framebuffer;
            
            Scaler.Update(GameSystem.System.PPU.Frame);
            
            BackBuffer = new Texture2D(GraphicsDevice, GameSystem.System.Width * Scaler.ScaleFactor, GameSystem.System.Height * Scaler.ScaleFactor);
            BackBuffer.SetData(Scaler.ScaledFramebuffer.Data);
        }

        private void UpdateMessages()
        {
            Osd.Update();

            if (Options.ShowFPS && IsRunning)
            {
                var fps = Math.Round(GameSystem.System.PPU.Frame / (DateTime.Now - LastStartDate).TotalSeconds, 1);
                Osd.UpdateMessage(MessageType.FPS, $"{fps:0.0} fps");
            }
        }

        public virtual void ResetGame()
        {
            IsRunning = false;

            Osd.InsertMessage(MessageType.Default, "Jogo reiniciado");
            LoadGame(GameSystem.Type, GameSystem.System.FileName);

            Menu.CloseAll();
        }

        public virtual void CloseGame()
        {
            Osd.InsertMessage(MessageType.Default, "Jogo fechado");
            LoadGameSystem(GameSystemFactory.GetEmptyGameSystem(this));
        }

        public void LoadState()
        {
            if (GameSystem is GameSystem)
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
            if (GameSystem is GameSystem)
                return;

            GameSystem.System.SaveState();
            Osd.InsertMessage(MessageType.Default, "Estado salvo");

            Menu.CloseAll();
        }
    }
}