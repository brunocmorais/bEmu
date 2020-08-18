namespace bEmu.Systems.Gameboy.Sound
{
    public class LengthCounter
    {
        private ushort length;
        private bool lengthStop;

        public void SetLength(ushort value, bool stopAfterLength)
        {
            length = length == 0 ? value : length;       
            lengthStop = stopAfterLength;
        }

        public bool Tick()
        {
            if (length != 0)
                length--;

            return !lengthStop || (length != 0 && lengthStop);
        }
    }
}