namespace bEmu.Core.Model
{
    public interface IState
    {
        byte[] Memory { get; set; }
        ushort PC { get; set; }
        ushort SP { get; set; }
        int Cycles { get; set; }
        bool Halted { get; set; }
        int Instructions { get; set; }

        void LoadProgram(byte[] bytes, int position);
        void LoadProgram(string fileName, int position);
    }
}