using bEmu.Core;
using bEmu.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{
    public interface IGameSystem
    {
        IMainGame MainGame { get; }
        ISystem System { get; }
        SupportedSystems Type { get; }
        void Initialize(int address);
        void Update();
        void UpdateGamePad(KeyboardState keyboardState);
        void StopGame();
    }
}