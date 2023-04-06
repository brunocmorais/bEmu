
using bEmu.Core.GUI;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace bEmu.Web.Drawers
{
    public abstract class Drawer<T> : IDrawer<T> where T : Core.GUI.IDrawable
    {
        protected Canvas2DContext Context;
        protected readonly string Regular;
        protected readonly int RegularSize;
        protected readonly string Title;
        protected readonly int TitleSize;
        protected readonly string Transparent;
        protected readonly string Black;
        protected readonly string White;
        protected readonly string LightGreen;

        protected Drawer(Canvas2DContext context)
        {
            Context = context;
            RegularSize = 24;
            Regular = RegularSize + "pt MEGAMAN10";
            TitleSize = 28;
            Title = TitleSize + "pt MEGAMAN10";
            Transparent = "#00000080";
            Black = "#000000FF";
            White = "#FFFFFFFF";
            LightGreen = "#9ACD32FF";
        }

        public abstract void Draw(T obj);
    }
}