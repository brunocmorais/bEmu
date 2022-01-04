namespace bEmu.Core.Mappers
{
    public interface IMapper
    {
        void LoadProgram(byte[] bytes);
        
        byte ReadROM(int addr);
        void Shutdown();
    }
}