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
using bEmu.Core.GamePad;
using bEmu.Core.GUI.Popups;
using bEmu.MonoGame.Drawers;
using bEmu.Core.Video;
using bEmu.Core.IO;

namespace bEmu.MonoGame
{
    public sealed class Main : Game, IMain
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly DynamicSoundEffectInstance sound;
        private Rectangle destinationRectangle;
        private Texture2D backBuffer;
        private SpriteBatch spriteBatch;
        private readonly GamePadBuilder gamePadBuilder;
        private DateTime lastStartDate;
        private int drawCounter;
        private IScaler scaler;
        private int lastRenderedFrame;
        private IDrawer<IOSD> osdDrawer;
        private IDrawer<IMenu> menuDrawer;
        private IDrawer<IPopup> popupDrawer;
        public Wave Recording { get; private set; }
        private double FPS => Math.Round(drawCounter / (DateTime.Now - lastStartDate).TotalSeconds, 1);
        public IOSD Osd { get; }
        public MenuManager MenuManager { get; }
        public IPopupManager PopupManager { get; }
        public bool IsRunning { get; private set; }
        public IOptions Options { get; private set; }
        public ISystem System { get; private set; }

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            IsRunning = false;
            sound = new DynamicSoundEffectInstance(APU.SampleRate, AudioChannels.Stereo);
            Osd = new OSD();
            gamePadBuilder = new GamePadBuilder();

            LoadSystem(SystemType.None, string.Empty);
            
            MenuManager = new MenuManager(this);
            PopupManager = new PopupManager(this);

            var regular = SpriteFontLoader.Get("font.ttf", GraphicsDevice, 20);
            var title = SpriteFontLoader.Get("font.ttf", GraphicsDevice, 24);
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            osdDrawer = new OSDDrawer(spriteBatch, GraphicsDevice, regular, title);
            menuDrawer = new MenuDrawer(spriteBatch, GraphicsDevice, regular, title);
            popupDrawer = new PopupDrawer(spriteBatch, GraphicsDevice, regular, title);

            MenuManager.Open((new MainMenu(this)));
        }

        protected override void Initialize() => SetScreenSize();

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

        public void Start()
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
            // atualização do GamePad
            var gamePad = gamePadBuilder.Build(Keyboard.GetState().GetPressedKeys());
            GamePadStateProvider.Instance.UpdateState(gamePad);

            // atualização dos itens renderizáveis em tela 
            MenuManager.Update();
            PopupManager.Update();

            // atualização das funções
            UpdateCommands();

            // atualização das notificações
            Osd.Update();

            // atualização dos itens controláveis
            if (PopupManager.IsOpen)
                PopupManager.UpdateControls(gameTime.TotalGameTime.TotalMilliseconds);
            else
                MenuManager.UpdateControls(gameTime.TotalGameTime.TotalMilliseconds);

            // atualização do sistema em execução
            UpdateSystem(gamePad);
        }

        private void UpdateSystem(IGamePad gamePad)
        {
            if (IsRunning && System.Frame == lastRenderedFrame)
            {
                System.UpdateGamePad(gamePad);

                if (Options.ShowFPS)
                    Osd.UpdateMessage(MessageType.FPS, $"{FPS:0.0} fps");
                
                UpdateSound();

                System.Update();

                if (scaler.Frame < System.Frame && !System.SkipFrame)
                    scaler.Update(System.Frame);
            }
        }

        private void UpdateSound()
        {
            while (sound.PendingBufferCount < APU.MaxBufferPending)
            {
                var soundBuffer = System.SoundBuffer;

                if (Options.EnableSound)
                    sound.SubmitBuffer(soundBuffer);

                if (Recording != null)
                    Recording.AddBytes(soundBuffer);
            }
        }

        private void UpdateCommands()
        {
            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F3)) // reiniciar jogo
                ResetGame();

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.P)) // pausar
            {
                if (!MenuManager.IsOpen)
                {
                    if (IsRunning)
                        Osd.InsertMessage(MessageType.Default, "Pausado");
                    else
                        Osd.InsertMessage(MessageType.Default, "Em andamento");

                    Pause();
                }
            }

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F2)) // mostrar informações
                Options.SetOption(nameof(Options.ShowFPS), false);

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F12)) // tirar snapshot
                Snapshot();

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F11)) // gravação
            {
                if (Recording == null)
                    StartRecording();
                else
                    StopRecording();
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
                menuDrawer.Draw(MenuManager.Current);

            if (PopupManager.IsOpen)
                popupDrawer.Draw(PopupManager.Current);

            osdDrawer.Draw(Osd);
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
            scaler = ScalerFactory.Get(Options.Scaler, Options.Size, System.Framebuffer);
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

        public void Snapshot()
        {
            var bitmap = Bitmap.From(scaler.ScaledFramebuffer);
            string fileName = $"snapshot_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.bmp";
            FileManager.Write(fileName, bitmap.ToBytes());
            Osd.InsertMessage(MessageType.Default, $"Snapshot '{fileName}' realizado!");
        }

        public void StartRecording()
        {
            Recording = new Wave();
            Osd.InsertMessage(MessageType.Default, "Iniciando gravação de som!");
        }

        public void StopRecording()
        {
            string fileName = $"rec_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.wav";
            FileManager.Write(fileName, Recording.ToBytes());
            Osd.InsertMessage(MessageType.Default, $"Gravação '{fileName}' realizada!");
            Recording = null;
        }
    }
}