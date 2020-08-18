namespace bEmu.Core
{
    public interface IState
    {
        ISystem System { get; }
        ushort PC { get; set; }
        ushort SP { get; set; }
        int Cycles { get; set; }
        bool Halted { get; set; }
        int Instructions { get; set; }
        void Reset();
    }
}