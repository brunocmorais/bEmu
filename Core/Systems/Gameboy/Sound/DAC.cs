namespace bEmu.Core.Systems.Gameboy.Sound
{
    public class DAC
    {
        public byte[] LeftBuffer = new byte[APU.AudioBufferFrames];
        public byte[] RightBuffer = new byte[APU.AudioBufferFrames];

        public void SetSamples(SampleData sampleData)
        {
            for (int i = 0; i < APU.AudioBufferFrames; i++)
            {
                float left = (float) sampleData.Square1Left[i];
                float right = (float) sampleData.Square1Right[i];

                //LeftBuffer.
            }
        }
    }
}