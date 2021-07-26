using System.Linq;
using System.Text;
using bEmu.Core.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu.MonoGame.Drawers
{
    public class OSDDrawer : Drawer<IOSD>
    {
        public OSDDrawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont regular, SpriteFont title) : base(spriteBatch, graphicsDevice, regular, title)
        {
        }

        public override void Draw(IOSD osd)
        {
            var messages = osd.Messages;

            if (!messages.Any())
                return;

            var sb = new StringBuilder();

            foreach (var message in messages)
                if (!string.IsNullOrWhiteSpace(message.Text))
                    sb.AppendLine(message.Text.Trim());

            if (sb.Length == 0)
                return;

            string text = sb.ToString().Trim();

            var size = Regular.MeasureString(text);
            Vector2 position = new Vector2(0, 0);

            SpriteBatch.Draw(Transparent, position, new Rectangle(0, 0, (int)(size.X + 10), (int)(size.Y + 10)), Color.White);
            SpriteBatch.DrawString(Regular, text, new Vector2(position.X + 5, position.Y + 5), Color.YellowGreen);
        }
    }
}