namespace bEmu.Core
{
    public interface IMMU
    {
        ISystem System { get; }
        byte this[long index] { get; set; }
        void LoadProgram(string fileName, int startAddress);
        void LoadProgram(byte[] bytes, int startAddress);
    }
}