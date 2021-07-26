namespace bEmu.Core.GUI
{
    public interface IDrawer<T> where T : IDrawable
    {
        void Draw(T obj);
    }
}