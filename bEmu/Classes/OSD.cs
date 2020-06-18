using System;
using System.Text;
using bEmu.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu
{
    public class OSD
    {
        private readonly SpriteBatch spriteBatch;
        private readonly SpriteFont font;
        private readonly ISystem system;
        public bool ShowFPS { get; set; }

        public OSD(ISystem system, SpriteBatch spriteBatch, SpriteFont font)
        {
            this.spriteBatch = spriteBatch;
            this.font = font;
            this.system = system;
        }

        public void Draw(GameTime gameTime)
        {
            string text = GetText(gameTime);
            
            if (!string.IsNullOrWhiteSpace(text))
                spriteBatch.DrawString(font, text, new Vector2(0, 0), Color.YellowGreen);
        }

        private string GetText(GameTime gameTime)
        {
            var sb = new StringBuilder();

            if (ShowFPS)
            {
                int frame = system.PPU.Frame;
                double fps = Math.Round(frame / gameTime.TotalGameTime.TotalSeconds, 1);
                sb.AppendLine($"{frame}@{fps} fps\ninst={system.State.Instructions}");
            }

            return sb.ToString();
        }
    }
}