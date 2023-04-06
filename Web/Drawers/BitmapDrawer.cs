using bEmu.Core.GUI;
using bEmu.Core.Video;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;

namespace bEmu.Web.Drawers
{
    public class BitmapDrawer : Drawer<BackBuffer>
    {
        private readonly ElementReference image;

        public BitmapDrawer(Canvas2DContext context, ElementReference image) : base(context)
        {
            this.image = image;
        }

        public override async void Draw(BackBuffer obj)
        {
            await Context.DrawImageAsync(image, 0, 0);
        }
    }
}