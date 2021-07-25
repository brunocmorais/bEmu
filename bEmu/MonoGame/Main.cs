using System;
using bEmu.Core.Video.Scalers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using bEmu.Systems.Factory;
using bEmu.Core.GUI.Menus;
using bEmu.Core.Enums;
using bEmu.Core.GUI;
using bEmu.Core.Audio;
using bEmu.Core.System;
using bEmu.Core.Input;
using bEmu.Core.GUI.Popups;

namespace bEmu.MonoGame
{
    public sealed class Main : Game, IMain
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly DynamicSoundEffectInstance sound;
        private readonly GamePadBuilder gamePadBuilder;
        private Rectangle destinationRectangle;
        private DateTime lastStartDate;
        private int drawCounter;
        private IScaler scaler;
        private int lastRenderedFrame;
        private Texture2D backBuffer;
        private IDrawer drawer;
        private SpriteBatch spriteBatch;
        private double FPS => Math.Round(drawCounter / (DateTime.Now - lastStartDate).TotalSeconds, 1);
        public IOSD Osd { get; }
        public IMenuManager MenuManager { get; }
        public IPopupManager PopupManager { get; }
        public bool IsRunning { get; private set; }
        public IOptions Options { get; private set; }
        public ISystem System { get; private set; }

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsRunning = false;
            sound = new DynamicSoundEffectInstance(APU.SampleRate, AudioChannels.Stereo);
            Osd = new OSD();
            gamePadBuilder = new GamePadBuilder();

            LoadSystem(0, string.Empty);
            
            MenuManager = new MenuManager(this);
            PopupManager = new PopupManager(this);
        }

        protected override void LoadContent()
        {
            var regular = Content.Load<SpriteFont>("Common/Regular");
            var title = Content.Load<SpriteFont>("Common/Title");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawer = new Drawer(spriteBatch, GraphicsDevice, regular, title);

            MenuManager.OpenMainMenu();
        }

        public void LoadSystem(SystemType system, string file)
        {
            System = SystemFactory.Get(system, file);
            Options = OptionsFactory.Build(this);

            if (system != SystemType.None)
            {
                try
                {
                    System.LoadProgram();
                    MenuManager.CloseAll();

                    SetScreenSize();
                    SetScaler();

                    Start();
                }
                catch (Exception ex)
                {
                    LoadSystem(SystemType.None, string.Empty);
                    PopupManager.ShowErrorDialog("Erro", "Houve um erro do sistema ao carregar o jogo selecionado.", ex);
                }
            }
            else
            {
                SetScreenSize();
                SetScaler();
            }
        }

        public void SetScreenSize()
        {
            graphics.PreferredBackBufferWidth = System.Width * Options.Size;
            graphics.PreferredBackBufferHeight = System.Height * Options.Size;

            destinationRectangle = new Rectangle(0, 0, System.Width * Options.Size, System.Height * Options.Size);
            graphics.ApplyChanges();
        }

        private void Start()
        {
            IsRunning = true;
            lastStartDate = DateTime.Now;
            lastRenderedFrame = 0;
            drawCounter = 0;

            if (Options.EnableSound)
                sound.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            var gamePad = gamePadBuilder.Build(Keyboard.GetState().GetPressedKeys());

            GamePadStateProvider.Instance.UpdateState(gamePad);
            Osd.Update();

            if (PopupManager.IsOpen)
                PopupManager.Update(gameTime.TotalGameTime.TotalMilliseconds);
            else
                MenuManager.Update(gameTime.TotalGameTime.TotalMilliseconds);

            if (IsRunning && System.Frame == lastRenderedFrame)
            {
                System.UpdateGamePad(gamePad);

                if (Options.ShowFPS)
                    Osd.UpdateMessage(MessageType.FPS, $"{FPS:0.0} fps");

                while (sound.PendingBufferCount < APU.MaxBufferPending && Options.EnableSound)
                {
                    try 
                    {
                        sound.SubmitBuffer(System.SoundBuffer);
                    }
                    catch { }
                }

                System.Update();

                if (scaler.Frame < System.Frame && !System.SkipFrame)
                    scaler.Update(System.Frame);
            }
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

            if (backBuffer == null)
                return;

            spriteBatch.Begin();

            if (IsRunning && System.Frame > lastRenderedFrame)
            {
                lastRenderedFrame = System.Frame;

                if (!System.SkipFrame)
                    backBuffer.SetData(scaler.ScaledFramebuffer.Data);
                    
                drawCounter++;
            }

            spriteBatch.Draw(backBuffer, destinationRectangle, Color.White);

            if (MenuManager.IsOpen)
                drawer.Draw(MenuManager.Current);

            if (PopupManager.IsOpen)
                drawer.Draw(PopupManager.Current);

            drawer.Draw(Osd);
            spriteBatch.End();
        }

        protected override void UnloadContent()
        {
            IsRunning = false;
        }

        public void StopGame()
        {
            System.Stop();
            Exit();
        }

        public void SetScaler()
        {
            scaler = ScalerFactory.Get(Options.Scaler, Options.Size);
            scaler.Framebuffer = System.Framebuffer;
            scaler.Update(System.Frame);
            
            backBuffer = new Texture2D(GraphicsDevice, System.Width * scaler.ScaleFactor, System.Height * scaler.ScaleFactor);
            backBuffer.SetData(scaler.ScaledFramebuffer.Data);
        }

        public void ResetGame()
        {
            IsRunning = false;

            Osd.InsertMessage(MessageType.Default, "Jogo reiniciado");
            LoadSystem(System.Type, System.FileName);

            MenuManager.CloseAll();
        }

        public void CloseGame()
        {
            Osd.InsertMessage(MessageType.Default, "Jogo fechado");
            LoadSystem(SystemType.None, string.Empty);
        }

        public void LoadState()
        {
            if (System.Type == SystemType.None)
                return;

            bool success = System.LoadState();

            if (success)
                Osd.InsertMessage(MessageType.Default, "Estado carregado");
            else
                Osd.InsertMessage(MessageType.Default, "Nenhum estado encontrado");

            MenuManager.CloseAll();
        }

        public void SaveState()
        {
            if (System.Type == SystemType.None)
                return;

            System.SaveState();
            Osd.InsertMessage(MessageType.Default, "Estado salvo");

            MenuManager.CloseAll();
        }

        public void SetSound(bool enable)
        {
            if (enable)
                sound.Play();
            else
                sound.Pause();
        }
    }
}