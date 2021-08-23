namespace bEmu.Core.Memory
{
    public interface IROM
    {
        byte[] Bytes { get; }
        string FileName { get; }
        string FileNameWithoutExtension { get; }
        string FilePath { get; }
        string SaveFileName { get; }
    }
}