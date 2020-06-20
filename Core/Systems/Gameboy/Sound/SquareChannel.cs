using System;

namespace bEmu.Core.Systems.Gameboy.Sound
{
    public class SquareChannel
    {
        private readonly MMU mmu;
        private readonly int sweep;
        private readonly int soundLengthAndWavePattern;
        private readonly int volumeEnvelope;
        private readonly int frequencyLo;
        private readonly int frequencyHi;
        private int cycles;
        private int sampleIndex;
        private int cycleSampleUpdate;
        //private int soundLengthCounter;
        private LengthCounter lengthCounter;
        private Envelope envelope;
        private ushort currentFrequency;
        public bool IsRunning { get; set; }
        public byte CurrentSample { get; private set; }

        private readonly byte[,] dutyCycles = 
        {
            { 1, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 0, 0 }
        };

        public SquareChannel(MMU mmu, int sweep, int soundLengthAndWavePattern, int volumeEnvelope, int frequencyLo, int frequencyHi)
        {
            this.mmu = mmu;
            this.sweep = sweep;
            this.soundLengthAndWavePattern = soundLengthAndWavePattern;
            this.volumeEnvelope = volumeEnvelope;
            this.frequencyLo = frequencyLo;
            this.frequencyHi = frequencyHi;
            this.lengthCounter = new LengthCounter();
            this.envelope = new Envelope();
            Restart();
        }

        public double SweepTime => ((Sweep & 0x70) >> 4) / 128;
        public bool SweepDecrease => ((Sweep & 0x8) == 0x8);
        public int SweepTimes => (Sweep & 0x7);
        public int WaveDuty => (SoundLengthAndWavePattern & 0xC0) >> 6;
        public int SoundLength => SoundLengthAndWavePattern & 0x3F;
        public byte Volume => (byte) ((VolumeEnvelope & 0xF0) >> 4);
        public bool VolumeDecrease => (VolumeEnvelope & 0x8) == 0x8;
        public byte NumberVolumeSweep => (byte) (VolumeEnvelope & 0x7);
        public ushort Frequency => (ushort) (((FrequencyHi & 0x3) << 8) | FrequencyLo);
        public bool StopOutputWhenLengthExpires => (FrequencyHi & 0x40) == 0x40;
        public bool RestartSound => (FrequencyHi & 0x80) == 0x80;

        public byte Sweep
        {
            get { return mmu.IO[sweep]; }
            set { mmu.IO[sweep] = value; }
        }

        public byte SoundLengthAndWavePattern
        {
            get { return mmu.IO[soundLengthAndWavePattern]; }
            set { mmu.IO[soundLengthAndWavePattern] = value; }
        }

        public byte VolumeEnvelope
        {
            get { return mmu.IO[volumeEnvelope]; }
            set { mmu.IO[volumeEnvelope] = value; }
        }

        public byte FrequencyLo
        {
            get { return mmu.IO[frequencyLo]; }
            set { mmu.IO[frequencyLo] = value; }
        }

        public byte FrequencyHi
        {
            get { return mmu.IO[frequencyHi]; }
            set { mmu.IO[frequencyHi] = value; }
        }

        public void LengthTick()
        {
            IsRunning = lengthCounter.Tick();

            if (!IsRunning)
                mmu.IO[0x26] &= 0xFE;
        }

        public void EnvelopeTick()
        {
            envelope.Tick();
        }

        public void Tick(int lastCycleCount)
        {
            cycles += lastCycleCount;

            if (cycles >= cycleSampleUpdate)
            {
                sampleIndex++;

                if (sampleIndex > 7)
                    sampleIndex = 0;

                UpdateSound();

                cycles -= cycleSampleUpdate;
            }
        }

        private void UpdateSound()
        {
            if (!IsRunning)
                CurrentSample = 0;
            else
            {
                byte dutyValue = dutyCycles[WaveDuty, sampleIndex];
                CurrentSample = (byte) (dutyValue * envelope.EnvelopeVolume);
            }
        }

        public void Restart()
        {
            IsRunning = true;
            mmu.IO[0x26] = (byte) (mmu.IO[0x26] | 0x1);
            lengthCounter.SetLength((ushort) (64 - SoundLength), StopOutputWhenLengthExpires);
            envelope.SetEnvelope(NumberVolumeSweep, VolumeEnvelope, !VolumeDecrease);

            currentFrequency = Frequency;
            cycleSampleUpdate = (2048 - currentFrequency) * 4;
            cycles = 0;
            sampleIndex = 0;
        }
    }
}