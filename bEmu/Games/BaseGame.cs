using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using bEmu.Classes;
using bEmu.Components;
using bEmu.Core;
using bEmu.Extensions;
using bEmu.Factory;
using bEmu.Scalers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{

    public abstract class BaseGame : Game, IBaseGame 
    {
        public int Width { get; set; }
        public int Height { get; set; }
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch SpriteBatch { get; set; }
        protected int lastRenderedFrame;
        protected Thread mainThread;
        protected Fonts fonts;
        protected Rectangle destinationRectangle;
        protected OSD osd;
        protected IMenu currentMenu => Menus.Any() ? Menus.Peek() : null;
        public Stack<IMenu> Menus { get; }
        protected Texture2D BackBuffer { get; set; }
        protected int DrawCounter { get; private set; }
        public bool IsRunning { get; set; }
        public Options Options { get; set; }
        public ISystem System { get; }
        public IPPU Gpu => System.PPU;
        public IState State => System.State;
        public IMMU Mmu => System.MMU;
        public IAPU Apu => System.APU;
        public DateTime LastStartDate { get; private set; }
        public IScaler Scaler { get; protected set; }
        public double FPS => Math.Round(System.PPU.Frame / (DateTime.Now - LastStartDate).TotalSeconds, 1);

        public BaseGame(ISystem system, int width, int height, int pixelSize)
        {
            System = system;
            Width = width;
            Height = height;
            Options = new Options();
            Options.Size = pixelSize;
            Options.OptionChanged += OnOptionChanged;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsRunning = false;
            DrawCounter = 0;
            Menus = new Stack<IMenu>();
        }

        protected override void LoadContent()
        {
            fonts = new Fonts();
            fonts.Regular = Content.Load<SpriteFont>("Common/Regular");
            fonts.Title = Content.Load<SpriteFont>("Common/Title");
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            osd = new OSD(this, SpriteBatch, fonts.Regular);
            SetScaler();
            SetScreenSize(); 
            base.LoadContent();
        }

        protected void SetScreenSize()
        {
            int width = Width * Options.Size;
            int height = Height * Options.Size;
            
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            destinationRectangle = new Rectangle(0, 0, width, height);
            graphics.ApplyChanges();
        }

        protected void StartMainThread()
        {
            mainThread = new Thread(() =>
            {
                while (IsRunning)
                    UpdateGame();
            });
            
            mainThread.Start();
            LastStartDate = DateTime.Now;
            Gpu.Frame = 0;
            lastRenderedFrame = 0;
            DrawCounter = 0;
        }

        public void LoadGame(string file)
        {
            System.FileName = file;
            System.Reset();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardStateExtensions.UpdateState();

            if (currentMenu != null)
                currentMenu.Update();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Escape)) // sair
                StopGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F3)) // reiniciar jogo
                ResetGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Back)) // voltar
                Menus.TryPop(out IMenu menu);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F1)) // abrir menu
            {
                if (!Menus.Any())
                    Menus.Push(new MainMenu(this, SpriteBatch, fonts));
                else
                    Menus.Clear();
            }

            if ((currentMenu != null && IsRunning) || (currentMenu == null && !IsRunning))
                PauseResumeGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.P)) // pausar
            {
                if (currentMenu == null)
                {
                    if (IsRunning)
                        osd.InsertMessage(MessageType.Default, "Pausado");
                    else
                        osd.InsertMessage(MessageType.Default, "Em andamento");

                    PauseResumeGame();
                }
            }

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F2)) // mostrar informações
            {
                Options.ShowFPS = !Options.ShowFPS;
                OnOptionChanged(this, new OnOptionChangedEventArgs() { Property = "ShowFPS" });
            }

            UpdateGamePad(keyboardState);
            UpdateMessages();

            if (Gpu.Frame > lastRenderedFrame)
                Scaler.Update();

            base.Update(gameTime);
        }

        protected void PauseResumeGame()
        {
            IsRunning = !IsRunning;

            if (IsRunning)
                StartMainThread();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (Gpu.Frame > lastRenderedFrame)
            {
                BackBuffer.SetData(Scaler.ScaledFramebuffer.Data);
                lastRenderedFrame = Gpu.Frame;
            }

            SpriteBatch.Draw(BackBuffer, destinationRectangle, Color.White);
            osd.Draw();

            if (currentMenu != null)
                currentMenu.Draw();

            if (IsRunning)
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
        protected virtual void OnOptionChanged(object sender, OnOptionChangedEventArgs e)
        {
            switch (e.Property)
            {
                case "ShowFPS":
                    osd.RemoveMessage(MessageType.FPS);

                    if (Options.ShowFPS)
                        osd.InsertMessage(MessageType.FPS, string.Empty);

                    break;
                case "Frameskip":
                    Gpu.Frameskip = Options.Frameskip;
                    break;
                case "Scaler":
                    SetScaler();
                    break;
                case "Size":
                    SetScreenSize();
                    break;
            }
        }

        protected void SetScaler()
        {
            Scaler = ScalerFactory.Get(Options.Scaler, Options.Size);
            Scaler.Framebuffer = Gpu.Framebuffer;
            Scaler.Update();
            BackBuffer = new Texture2D(GraphicsDevice, Width * Scaler.ScaleFactor, Height * Scaler.ScaleFactor);
            BackBuffer.SetData(Scaler.ScaledFramebuffer.Data);
        }

        protected void UpdateMessages()
        {
            osd.Update();

            if (Options.ShowFPS && IsRunning)
                osd.UpdateMessage(MessageType.FPS, $"{FPS:0.0} fps");
        }

        public virtual void ResetGame()
        {
            osd.InsertMessage(MessageType.Default, "Jogo reiniciado");
            System.Reset();

            if (currentMenu != null)
                Menus.Clear();
        }

        public void LoadState()
        {
            bool success = System.LoadState();

            if (success)
                osd.InsertMessage(MessageType.Default, "Estado carregado");
            else
                osd.InsertMessage(MessageType.Default, "Nenhum estado encontrado");

            if (currentMenu != null)
                Menus.Clear();
        }
        public void SaveState()
        {
            System.SaveState();
            osd.InsertMessage(MessageType.Default, "Estado salvo");

            if (currentMenu != null)
                Menus.Clear();
        }
    }
}