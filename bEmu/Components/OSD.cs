using System;
using System.Collections.Generic;
using System.Text;
using bEmu.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace bEmu.Components
{

    public class OSD
    {
        private readonly IMainGame game;
        private List<Message> messages;
        private readonly Texture2D background;

        public OSD(IMainGame game)
        {
            this.game = game;
            messages = new List<Message>();
            background = new Texture2D(game.GraphicsDevice, 1, 1);
            background.SetData(new [] { Color.FromNonPremultiplied(0, 0, 0, 0x80) });
        }

        public void Update()
        {
            messages.RemoveAll(x => messages.Where(x => x.ExpirationDate < DateTime.Now).Contains(x));
        }

        public void Draw()
        {
            if (!messages.Any())
                return;

            var sb = new StringBuilder();

            foreach (var message in messages)
                if (!string.IsNullOrWhiteSpace(message.Text))
                    sb.AppendLine(message.Text);

            if (sb.Length == 0)
                return;

            string text = sb.ToString().Substring(0, sb.Length - 1);

            var size = game.Fonts.Regular.MeasureString(text);
            Vector2 position = new Vector2(0, 0);
            
            game.SpriteBatch.Draw(background, position, new Rectangle(0, 0, (int) (size.X + 10), (int) (size.Y + 10)), Color.White);
            game.SpriteBatch.DrawString(game.Fonts.Regular, text, new Vector2(position.X + 5, position.Y + 5), Color.YellowGreen);
        }

        public void UpdateMessage(MessageType type, string messageText)
        {
            if (type == MessageType.Default)
                return;

            var message = messages.FirstOrDefault(x => x.Type == type);

            if (message == null)
                return;

            message.Text = messageText;
        }

        public void InsertMessage(MessageType type, string messageText)
        {
            DateTime expirationDate = type == MessageType.Default ? DateTime.Now.AddSeconds(3) : DateTime.MaxValue;
            messages.Add(new Message() { Text = messageText, Type = type, ExpirationDate = expirationDate });
        }

        public void RemoveMessage(MessageType type)
        {
            if (type == MessageType.Default)
                return;

            messages.RemoveAll(x => x.Type == type);
        }
    }

    public class Message
    {
        public string Text { get; set; }
        public DateTime ExpirationDate { get; set; }
        public MessageType Type { get; set; }
    }

    public enum MessageType
    {
        Default = 0,
        FPS = 1
    }
}