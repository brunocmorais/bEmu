namespace bEmu.Core.Systems.Gameboy.Sound
{
    public class SampleData
    {
        public byte[] Square1Left { get; }
        public byte[] Square1Right { get; }

        public SampleData()
        {
            Square1Left = new byte[APU.AudioBufferFrames];
            Square1Right = new byte[APU.AudioBufferFrames];
        }
    }
}