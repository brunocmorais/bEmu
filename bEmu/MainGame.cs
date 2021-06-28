using System;
using bEmu.Components;
using bEmu.Extensions;
using bEmu.Core.Scalers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bEmu.Core.Factory;
using bEmu.Systems;
using Microsoft.Xna.Framework.Audio;
using bEmu.Core;
using bEmu.Systems.Factory;
using bEmu.Menus;
using bEmu.Systems.Gameboy.GPU.Palettes;
using bEmu.Core.Components;

namespace bEmu
{

    public class MainGame : Game, IMainGame
    {
        private GraphicsDeviceManager graphics;
        private Rectangle destinationRectangle;
        private IGamePad gamePad;
        private DateTime lastStartDate;
        private IScaler scaler;
        private int lastRenderedFrame;
        private DynamicSoundEffectInstance sound;
        private Texture2D backBuffer;
        private int drawCounter;
        private SupportedSystems type;
        public SpriteBatch SpriteBatch { get; set; }
        public Fonts Fonts { get; set; }
        public OSD Osd { get; set; }
        public GameMenu Menu { get; set; }
        public bool IsRunning { get; set; }
        public Options Options { get; set; }
        public ISystem System { get; set; }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsRunning = false;
            drawCounter = 0;
            sound = new DynamicSoundEffectInstance(22050, AudioChannels.Stereo);
            gamePad = new Core.GamePad();
        }

        protected override void LoadContent()
        {
            Fonts = new Fonts();
            Fonts.Regular = Content.Load<SpriteFont>("Common/Regular");
            Fonts.Title = Content.Load<SpriteFont>("Common/Title");
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Osd = new OSD(this);

            LoadSystem(0, string.Empty);
            
            Menu = new GameMenu(this);
            Menu.OpenMainMenu();
        }

        public void LoadSystem(SupportedSystems system, string file)
        {
            this.type = system;
            IOptions options = default;
            int size;

            switch (system)
            {
                case SupportedSystems.Chip8:
                    size = 5;
                    break;
                case SupportedSystems.Generic8080:
                    size = 2;
                    break;
                case SupportedSystems.GameBoy:
                    size = 2;
                    options = new Systems.Gameboy.Options(OptionChangedEvent, Options, size);
                    break;
                default:
                    size = 1;
                    break;
            }

            System = SystemFactory.Get(type, file);

            if (options == null)
                options = new Core.Components.Options(OptionChangedEvent, Options, size);

            Options = (Options) options;

            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, System.RefreshRate);

            SetScreenSize(); 
            SetScaler();

            if (type != 0)
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
            System.Frame = 0;
            lastRenderedFrame = 0;
            drawCounter = 0;

            if (Options.EnableSound)
                sound.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardStateExtensions.UpdateState(gameTime);

            Menu.Update(gameTime);

            if (!Menu.IsOpen)
            {
                gamePad.Reset();
                gamePad.AddPressedKeys(KeyboardStateExtensions.GetPressedGamePadKeys());

                System.UpdateGamePad(gamePad);
            }
                
            Osd.Update();

            if (Options.ShowFPS && IsRunning)
            {
                var fps = Math.Round(System.Frame / (DateTime.Now - lastStartDate).TotalSeconds, 1);
                Osd.UpdateMessage(MessageType.FPS, $"{fps:0.0} fps");
            }

            if (IsRunning && System.Frame <= lastRenderedFrame)
            {
                if (System.Cycles < 0)
                    System.ResetCycles();

                System.Update();
                
                while (sound.PendingBufferCount < 3 && Options.EnableSound)
                    sound.SubmitBuffer(System.SoundBuffer);
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

            SpriteBatch.Begin();

            if (IsRunning)
            {
                drawCounter++;

                if (System.Frame > lastRenderedFrame)
                {
                    scaler.Update(System.Frame);
                    backBuffer.SetData(scaler.ScaledFramebuffer.Data);
                    lastRenderedFrame = System.Frame;
                }
            }

            SpriteBatch.Draw(backBuffer, destinationRectangle, Color.White);

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

        public virtual void ResetGame()
        {
            IsRunning = false;

            Osd.InsertMessage(MessageType.Default, "Jogo reiniciado");
            LoadSystem(type, System.FileName);

            Menu.CloseAll();
        }

        public virtual void CloseGame()
        {
            Osd.InsertMessage(MessageType.Default, "Jogo fechado");
            LoadSystem(0, string.Empty);
        }

        public void LoadState()
        {
            if (type == 0) // nenhum sistema carregado
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
            if (type == 0)
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
                    (System as Systems.Gameboy.System).ColorPalette = ColorPaletteFactory.Get(gameBoyOptions.PaletteType);
                    break;
            }
        }
    }
}