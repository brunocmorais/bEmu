using bEmu.Components;
using bEmu.Core;
using bEmu.Systems;
using bEmu.Systems.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{

    public class DummyGameSystem : IGameSystem
    {
        public SupportedSystems Type => 0;
        public int Width => 640;
        public int Height => 480;
        public Texture2D BackBuffer { get; private set; }
        public IMainGame MainGame { get; }
        public int Frame { get; set; }
        public int Frameskip { get; set; }
        public Framebuffer Framebuffer => framebuffer;
        public int RefreshRate => 16;
        public ISystem System => null;

        private readonly Framebuffer framebuffer;

        public DummyGameSystem(IMainGame mainGame)
        {
            framebuffer = new Framebuffer(Width, Height);
            MainGame = mainGame;
            MainGame.Options = new Options(MainGame);
            MainGame.Options.Size = 1;
        }

        public void Initialize() { }
        public void LoadContent() { }

        public void Update(GameTime gameTime) { }
        public void UpdateGame() { }
        public void UpdateGamePad(KeyboardState keyboardState) { }
        public void Draw(GameTime gameTime) { }
        public void StopGame() { }
    }
}