using System.Diagnostics;
using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;
using bEmu.Core.GUI;
using bEmu.Core.GUI.Menus;
using bEmu.Core.GUI.Popups;
using bEmu.Core.IO;
using bEmu.Core.System;
using bEmu.Core.Video;
using bEmu.Core.Video.Scalers;
using bEmu.Systems.Factory;
using bEmu.Web.Drawers;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.JSInterop;

namespace bEmu.Web.Classes
{
    public class Main : IMain
    {
        private IDrawer<IOSD> osdDrawer;
        private IDrawer<IMenu> menuDrawer;
        private IDrawer<IPopup> popupDrawer;
        private IScaler scaler;
        private DateTime lastStartDate;
        private int lastRenderedFrame;
        private int drawCounter;
        private GamePadBuilder gamePadBuilder;
        private Stopwatch stopwatch;
        private double FPS => Math.Round(drawCounter / (DateTime.Now - lastStartDate).TotalSeconds, 1);

        public IOptions Options { get; set; }
        public IOSD Osd { get; }
        public bool IsRunning { get; set; }
        public ISystem System { get; set; }
        public MenuManager MenuManager { get; }
        public IPopupManager PopupManager { get; }
        public Wave Recording { get; private set; }
        public int Width => System is IVideoSystem ? (System as IVideoSystem).Width : 640; 
        public int Height => System is IVideoSystem ? (System as IVideoSystem).Height : 480; 
        public IList<string> PressedKeys { get; }
        public IJSRuntime Js { get; }

        public Main(Canvas2DContext context, IJSRuntime js) : this(context, js, SystemType.None, string.Empty) { } 

        public Main(Canvas2DContext context, IJSRuntime js, SystemType type, string fileName)
        {            
            MenuManager = new MenuManager(this);
            menuDrawer = new MenuDrawer(context);
            
            Osd = new OSD();
            osdDrawer = new OSDDrawer(context);

            PopupManager = new PopupManager(this);
            popupDrawer = new PopupDrawer(context);

            gamePadBuilder = new GamePadBuilder();
            PressedKeys = new List<string>();

            stopwatch = Stopwatch.StartNew();

            LoadSystem(SystemType.None, string.Empty);
            Js = js;
        }

        public void Initialize()
        {
            if (System.Type == SystemType.None)
                MenuManager.Open((new MainMenu(this)));
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

        public void Start()
        {
            IsRunning = true;
            lastStartDate = DateTime.Now;
            lastRenderedFrame = 0;
            drawCounter = 0;

            // if (Options.EnableSound)
            //     sound.Play();
        }

        public void Update()
        {
            // atualização do GamePad
            var gamePad = gamePadBuilder.Build(PressedKeys.ToArray());
            GamePadStateProvider.Instance.UpdateState(gamePad);

            // atualização dos itens renderizáveis
            MenuManager.Update();
            PopupManager.Update();

            MenuManager.UpdateControls(stopwatch.ElapsedMilliseconds);

            // atualização das funções
            UpdateCommands();

            // atualização das notificações
            Osd.Update();

            // atualização dos itens controláveis
            if (PopupManager.IsOpen)
                PopupManager.UpdateControls(stopwatch.ElapsedMilliseconds);
            else
                MenuManager.UpdateControls(stopwatch.ElapsedMilliseconds);

            // atualização do sistema em execução
            UpdateSystem(gamePad);
        }

        private void UpdateSystem(IGamePad gamePad)
        {
            if (IsRunning)
            {
                if (System is IVideoSystem)
                {
                    var system = System as IVideoSystem;
                    
                    if (system.Frame == lastRenderedFrame)
                    {
                        system.Update();

                        if (scaler.Frame < system.Frame && !system.SkipFrame)
                            scaler.Update(system.Frame);
                    }
                }

                if (System is IGamePadSystem)
                    (System as IGamePadSystem).UpdateGamePad(gamePad);

                if (Options.ShowFPS)
                    Osd.UpdateMessage(MessageType.FPS, $"{FPS:0.0} fps");

                if (System is IAudioSystem)
                {
                    if (System is not IVideoSystem)
                        System.Update();

                    // while (sound.PendingBufferCount < APU.MaxBufferPending)
                    // {
                    //     var soundBuffer = (System as IAudioSystem).SoundBuffer;

                    //     if (Options.EnableSound)
                    //         sound.SubmitBuffer(soundBuffer);

                    //     if (Recording != null)
                    //         Recording.AddBytes(soundBuffer);
                    // }
                }    
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

        public async Task Draw()
        {
            if (System is IVideoSystem)
            {
                var system = System as IVideoSystem;

                if (IsRunning && system.Frame > lastRenderedFrame)
                {
                    lastRenderedFrame = system.Frame;

                    if (!system.SkipFrame)
                        await Js.InvokeVoidAsync("writeFramebuffer", scaler.ScaledFramebuffer.Data, scaler.ScaledFramebuffer.Width, scaler.ScaledFramebuffer.Height);
                        
                    drawCounter++;
                }
            }

            if (MenuManager.IsOpen)
                menuDrawer.Draw(MenuManager.Current);

            if (PopupManager.IsOpen)
                popupDrawer.Draw(PopupManager.Current);

            osdDrawer.Draw(Osd);
        }

        public void StopGame()
        {
            System.Stop();
        }

        public void SetScaler()
        {
            if (System is IVideoSystem)
            {
                var system = System as IVideoSystem;
                scaler = ScalerFactory.Get(Options.Scaler, Options.Size, system.Framebuffer);
                scaler.Update(system.Frame);
            }
        }

        public void ResetGame()
        {
            IsRunning = false;

            Osd.InsertMessage(MessageType.Default, "Jogo reiniciado");
            LoadSystem(System.Type, System.ROM.FileName);

            MenuManager.CloseAll();
        }

        public void CloseGame()
        {
            Osd.InsertMessage(MessageType.Default, "Jogo fechado");
            LoadSystem(SystemType.None, string.Empty);
        }

        public void LoadState()
        {
            if (System is ISaveStateSystem)
            {
                bool success = (System as ISaveStateSystem).LoadState();

                if (success)
                    Osd.InsertMessage(MessageType.Default, "Estado carregado");
                else
                    Osd.InsertMessage(MessageType.Default, "Nenhum estado encontrado");
            }

            MenuManager.CloseAll();
        }

        public void SaveState()
        {
            if (System is ISaveStateSystem)
            {
                (System as ISaveStateSystem).SaveState();
                Osd.InsertMessage(MessageType.Default, "Estado salvo");
            }
            
            MenuManager.CloseAll();
        }

        public void SetSound(bool enable)
        {
            // if (enable)
            //     sound.Play();
            // else
            //     sound.Pause();
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

        public void SetScreenSize()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}