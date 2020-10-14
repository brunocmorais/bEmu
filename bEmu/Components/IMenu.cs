using Microsoft.Xna.Framework;

namespace bEmu.Components
{
    public interface IMenu : IDrawable
    {
        string Title { get; }
        void Update(GameTime gameTime);
    }
}