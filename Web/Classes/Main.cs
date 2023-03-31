using System.Diagnostics;
using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;
using bEmu.Core.GUI;
using bEmu.Core.GUI.Menus;
using bEmu.Core.GUI.Popups;
using bEmu.Core.System;
using bEmu.Core.Video.Scalers;
using bEmu.Systems.Factory;
using bEmu.Web.Drawers;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace bEmu.Web.Classes
{
    public class Main : IMain
    {
        private MenuDrawer menuDrawer;
        private IScaler scaler;
        private DateTime lastStartDate;
        private int lastRenderedFrame;
        private int drawCounter;
        private GamePadBuilder gamePadBuilder;
        private Stopwatch stopwatch;

        public IOptions Options { get; set; }
        public IOSD Osd { get; }
        public bool IsRunning { get; set; }
        public ISystem System { get; set; }
        public MenuManager MenuManager { get; }
        public IPopupManager PopupManager { get; }
        public Wave Recording { get; }
        public int Width { get; }
        public int Height { get; }
        public string PressedKey { get; set; }

        public Main(Canvas2DContext context, BECanvasComponent canvas)
        {
            Width = (int) canvas.Width;
            Height = (int) canvas.Height;
            MenuManager = new MenuManager(this);
            menuDrawer = new MenuDrawer(context, canvas);
            gamePadBuilder = new GamePadBuilder();
            stopwatch = Stopwatch.StartNew();

            LoadSystem(SystemType.None, string.Empty);

            MenuManager.Open(new MainMenu(this));
            menuDrawer.Draw(MenuManager.Current);
        }

        public void CloseGame()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void LoadState()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            var gamePad = gamePadBuilder.Build(new string[] { PressedKey });
            GamePadStateProvider.Instance.UpdateState(gamePad);

            MenuManager.Update();

            MenuManager.UpdateControls(stopwatch.ElapsedMilliseconds);

            //UpdateCommands();
        }

        public void LoadSystem(SystemType system, string file)
        {
            if (system == SystemType.None)
            {
                System = SystemFactory.Instance.GetEmptySystem();
                Options = OptionsFactory.Instance.Get(system, this);

                SetScreenSize();
                SetScaler();
            }
            else
            {
                System = SystemFactory.Instance.Get(system, file);
                Options = OptionsFactory.Instance.Get(system, this);

                try
                {
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
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void ResetGame()
        {
            throw new NotImplementedException();
        }

        public void SaveState()
        {
            throw new NotImplementedException();
        }

        public void SetScaler()
        {
            if (System is IVideoSystem)
            {
                var system = System as IVideoSystem;
                scaler = ScalerFactory.Get(Options.Scaler, Options.Size, system.Framebuffer);
                scaler.Update(system.Frame);
                
                // backBuffer = new Texture2D(GraphicsDevice, Width * scaler.ScaleFactor, Height * scaler.ScaleFactor);
                // backBuffer.SetData(scaler.ScaledFramebuffer.Data);
            }
        }

        public void SetScreenSize()
        {
            
        }

        public void SetSound(bool enable)
        {
            throw new NotImplementedException();
        }

        public void Snapshot()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            IsRunning = true;
            lastStartDate = DateTime.Now;
            lastRenderedFrame = 0;
            drawCounter = 0;

            // if (Options.EnableSound)
            //     sound.Play();
        }

        public void StartRecording()
        {
            throw new NotImplementedException();
        }

        public void StopGame()
        {
            throw new NotImplementedException();
        }

        public void StopRecording()
        {
            throw new NotImplementedException();
        }
    }
}