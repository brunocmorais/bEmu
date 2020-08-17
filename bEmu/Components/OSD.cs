using System;
using System.Text;
using bEmu.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu.Components
{

    public class OSD : IDrawable
    {
        private readonly SpriteBatch spriteBatch;
        private readonly SpriteFont font;
        private readonly IBaseGame game;
        private string text = "60 fps";

        public OSD(IBaseGame game, SpriteBatch spriteBatch, SpriteFont font)
        {
            this.spriteBatch = spriteBatch;
            this.font = font;
            this.game = game;
        }

        public void Draw()
        {
            if (!game.Options.ShowFPS)
                return;

            if (game.IsRunning)
                UpdateText();

            if (!string.IsNullOrWhiteSpace(text))
                spriteBatch.DrawString(font, text, new Vector2(0, 0), Color.YellowGreen);
        }

        private void UpdateText()
        {
            text = $"{Math.Round(game.System.PPU.Frame / (DateTime.Now - game.LastStartDate).TotalSeconds, 1)} fps";
        }
    }
}