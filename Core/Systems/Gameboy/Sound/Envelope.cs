namespace bEmu.Core.Systems.Gameboy.Sound
{
    public class Envelope
    {
        private byte envelopeTicks;
        public byte EnvelopeVolume { get; private set; }
        private bool isEnvelopeIncreasing;
        private byte ticks;

        public void SetEnvelope(byte ticks, byte startVolume, bool increasing)
        {
            envelopeTicks = ticks;
            EnvelopeVolume = startVolume;
            isEnvelopeIncreasing = increasing;
            this.ticks = 0;
        }

        public void Tick()
        {
            if (envelopeTicks == 0)
                return;

            ticks++;
            ticks %= envelopeTicks;

            if (ticks == 0)
            {
                if (isEnvelopeIncreasing && EnvelopeVolume != 0xF)
                    EnvelopeVolume++;
                else if (!isEnvelopeIncreasing && EnvelopeVolume != 0x0)
                    EnvelopeVolume--;
            }
        }
    }
}
