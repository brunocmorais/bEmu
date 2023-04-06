using System.Text;
using bEmu.Core.GUI;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace bEmu.Web.Drawers
{
    public class OSDDrawer : Drawer<IOSD>
    {
        public OSDDrawer(Canvas2DContext context) : base(context)
        {
        }

        public override async void Draw(IOSD osd)
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

            await Context.SetFontAsync(Regular);
            var size = (await Context.MeasureTextAsync(text)).Width;

            await Context.SetFillStyleAsync(Transparent);
            await Context.FillRectAsync(0, 0, size + 10, RegularSize + 10);

            await Context.SetFillStyleAsync(LightGreen);
            await Context.FillTextAsync(text, 5, 5);
        }
    }
}