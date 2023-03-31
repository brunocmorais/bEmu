
using bEmu.Core.GUI;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace bEmu.Web.Drawers
{
    public abstract class Drawer<T> : IDrawer<T> where T : Core.GUI.IDrawable
    {
        protected Canvas2DContext Context;
        protected BECanvasComponent Canvas;
        protected readonly string Regular;
        protected readonly int RegularSize;
        protected readonly string Title;
        protected readonly int TitleSize;
        protected readonly string Transparent;
        protected readonly string Black;
        protected readonly string White;

        protected Drawer(Canvas2DContext context, BECanvasComponent canvas)
        {
            Context = context;
            Canvas = canvas;
            RegularSize = 24;
            Regular = RegularSize + "pt MEGAMAN10";
            TitleSize = 28;
            Title = TitleSize + "pt MEGAMAN10";
            Transparent = "#FFFFFF00";
            Black = "#000000FF";
            White = "#FFFFFFFF";
        }

        public abstract void Draw(T obj);
    }
}