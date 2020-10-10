namespace bEmu.Components
{
    public interface IMenu : IDrawable
    {
        string Title { get; }
        void Update();
    }
}