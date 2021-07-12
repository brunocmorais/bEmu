using System;
using bEmu.Components;
using bEmu.Core.Video.Scalers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using bEmu.Core;
using bEmu.Systems.Factory;
using bEmu.Core.UI.Menus;
using bEmu.Core.Components;
using bEmu.Core.Util;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.UI;
using bEmu.Core.Audio;

namespace bEmu
{

    public class MainGame : Game, IMainGame
    {
        private readonly IGamePad gamePad;
        private readonly GraphicsDeviceManager graphics;
        private readonly DynamicSoundEffectInstance sound;
        private Rectangle destinationRectangle;
        private DateTime lastStartDate;
        private int drawCounter;
        private IScaler scaler;
        private int lastRenderedFrame;
        private Texture2D backBuffer;
        private IDrawer drawer;
        private SpriteBatch spriteBatch;
        public IOSD Osd { get; }
        public IGameMenu Menu { get; private set; }
        public bool IsRunning { get; private set; }
        public IOptions Options { get; private set; }
        public ISystem System { get; private set; }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsRunning = false;
            sound = new DynamicSoundEffectInstance(APU.SampleRate, AudioChannels.Stereo);
            gamePad = new Core.Input.GamePad();
            Osd = new OSD();
        }

        protected override void LoadContent()
        {
            var fonts = new Fonts();
            fonts.Regular = Content.Load<SpriteFont>("Common/Regular");
            fonts.Title = Content.Load<SpriteFont>("Common/Title");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawer = new Drawer(spriteBatch, GraphicsDevice, fonts);

            LoadSystem(0, string.Empty);
            Menu = new GameMenu(this);
            Menu.OpenMainMenu();
        }

        public void LoadSystem(SystemType system, string file)
        {
            IOptions options = default;
            int size;

            switch (system)
            {
                case SystemType.Chip8:
                    size = 5;
                    break;
                case SystemType.Generic8080:
                    size = 2;
                    break;
                case SystemType.GameBoy:
                    size = 2;
                    options = new Systems.Gameboy.Options(OptionChangedEvent, Options, size);
                    break;
                default:
                    size = 1;
                    break;
            }

            System = SystemFactory.Get(system, file);

            if (options == null)
                options = new Core.Components.Options(OptionChangedEvent, Options, size);

            Options = (Options) options;

            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, System.RefreshRate);

            SetScreenSize(); 
            SetScaler();

            if (system != 0)
            {
                System.LoadProgram();
                Menu.CloseAll();
                Start();
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
            KeyboardState keyboardState = Keyboard.GetState();
            var gamePadState = GamePadStateCreator.Create(keyboardState.GetPressedKeys());
            
            GamePadUtils.UpdateState(gamePadState);

            Menu.Update(gameTime.TotalGameTime.TotalMilliseconds);

            if (!Menu.IsOpen)
            {
                gamePad.Reset();
                gamePad.AddPressedKeys(GamePadUtils.GetPressedGamePadKeys());

                System.UpdateGamePad(gamePad);
            }
                
            Osd.Update();

            if (IsRunning && System.Frame == lastRenderedFrame)
            {
                if (Options.ShowFPS)
                    Osd.UpdateMessage(MessageType.FPS, $"{GetFPS():0.0} fps");

                if (System.Cycles < 0)
                    System.ResetCycles();

                while (sound.PendingBufferCount < APU.MaxBufferPending && Options.EnableSound)
                    sound.SubmitBuffer(System.SoundBuffer);

                System.Update();
            }
        }

        private double GetFPS()
        {
            return Math.Round(drawCounter / (DateTime.Now - lastStartDate).TotalSeconds, 1);
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
                scaler.Update(System.Frame);
                backBuffer.SetData(scaler.ScaledFramebuffer.Data);
                lastRenderedFrame = System.Frame;
                drawCounter++;
            }

            spriteBatch.Draw(backBuffer, destinationRectangle, Color.White);

            if (Menu.IsOpen)
                drawer.Draw(Menu.Current);

            drawer.Draw(Osd);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            IsRunning = false;
        }

        public virtual void StopGame()
        {
            System.Stop();
            Exit();
        }

        private void SetScaler()
        {
            scaler = ScalerFactory.Get(Options.Scaler, Options.Size);
            scaler.Framebuffer = System.Framebuffer;
            scaler.Update(System.Frame);
            
            backBuffer = new Texture2D(GraphicsDevice, System.Width * scaler.ScaleFactor, System.Height * scaler.ScaleFactor);
            backBuffer.SetData(scaler.ScaledFramebuffer.Data);
        }

        public virtual void ResetGame()
        {
            IsRunning = false;

            Osd.InsertMessage(MessageType.Default, "Jogo reiniciado");
            LoadSystem(System.Type, System.FileName);

            Menu.CloseAll();
        }

        public virtual void CloseGame()
        {
            Osd.InsertMessage(MessageType.Default, "Jogo fechado");
            LoadSystem(0, string.Empty);
        }

        public void LoadState()
        {
            if (System.Type == 0) // nenhum sistema carregado
                return;

            bool success = System.LoadState();

            if (success)
                Osd.InsertMessage(MessageType.Default, "Estado carregado");
            else
                Osd.InsertMessage(MessageType.Default, "Nenhum estado encontrado");

            Menu.CloseAll();
        }

        public void SaveState()
        {
            if (System.Type == 0)
                return;

            System.SaveState();
            Osd.InsertMessage(MessageType.Default, "Estado salvo");

            Menu.CloseAll();
        }

        public void SetSound(bool enable)
        {
            if (enable)
                sound.Play();
            else
                sound.Pause();
        }

        private void OptionChangedEvent(object sender, OnOptionChangedEventArgs e)
        {
            var options = sender as Core.Components.Options;
            var gameBoyOptions = sender as Systems.Gameboy.Options;

            switch (e.Property)
            {
                case nameof(options.ShowFPS):
                    Osd.RemoveMessage(MessageType.FPS);

                    if (options.ShowFPS)
                        Osd.InsertMessage(MessageType.FPS, string.Empty);

                    break;
                case nameof(options.Frameskip):
                    System.Frameskip = options.Frameskip;
                    break;
                case nameof(options.Scaler):
                    SetScaler();
                    break;
                case nameof(options.Size):
                    SetScreenSize();
                    break;
                case nameof(options.EnableSound):
                    SetSound(options.EnableSound);
                    break;
                case nameof(gameBoyOptions.PaletteType):
                    (System as Systems.Gameboy.System).SetColorPalette(gameBoyOptions.PaletteType);
                    break;
            }
        }
    }
}