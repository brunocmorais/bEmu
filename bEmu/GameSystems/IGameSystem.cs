using bEmu.Core;
using bEmu.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{
    public interface IGameSystem
    {
        int Width { get; }
        int Height { get; }
        IMainGame MainGame { get; }
        int Frame { get; set; }
        int Frameskip { get; set; }
        Framebuffer Framebuffer { get; }
        int RefreshRate { get; }
        ISystem System { get; }
        SupportedSystems Type { get; }

        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
        void UpdateGame();
        void UpdateGamePad(KeyboardState keyboardState);
        void StopGame();
    }
}