using bEmu.Core.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu.MonoGame.Drawers
{
    public abstract class Drawer<T> : IDrawer<T> where T : Core.GUI.IDrawable
    {
        protected readonly SpriteBatch SpriteBatch;
        protected readonly SpriteFont Regular;
        protected readonly SpriteFont Title;
        protected readonly Texture2D Transparent;
        protected readonly Texture2D Black;
        protected readonly Texture2D White;

        public Drawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont regular, SpriteFont title)
        {
            SpriteBatch = spriteBatch;
            Regular = regular;
            Title = title;
            
            Transparent = new Texture2D(graphicsDevice, 1, 1);
            Transparent.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 0x80) });
            
            Black = new Texture2D(graphicsDevice, 1, 1);
            Black.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 0xFF) });
            
            White = new Texture2D(graphicsDevice, 1, 1);
            White.SetData(new[] { Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xB0) });
        }

        public abstract void Draw(T obj);
    }
}