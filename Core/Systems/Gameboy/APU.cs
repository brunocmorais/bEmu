using System;

namespace bEmu.Core.Systems.Gameboy
{
    public class APU
    {
        private Sound sound;

        public APU(MMU mmu)
        {
            sound = new Sound(mmu);
        }
    }

    public class Sound
    {
        private readonly MMU mmu;

        public Sound(MMU mmu)
        {
            this.mmu = mmu;
        }

        public byte NR10
        {
            get { return mmu.IO[0xFF10]; }
            set { mmu.IO[0xFF10] = value; }
        }

        public byte NR11
        {
            get { return mmu.IO[0xFF11]; }
            set { mmu.IO[0xFF11] = value; }
        }

        public byte NR12
        {
            get { return mmu.IO[0xFF12]; }
            set { mmu.IO[0xFF12] = value; }
        }

        public byte NR13
        {
            get { return mmu.IO[0xFF13]; }
            set { mmu.IO[0xFF13] = value; }
        }

        public byte NR14
        {
            get { return mmu.IO[0xFF14]; }
            set { mmu.IO[0xFF14] = value; }
        }

        public double SweepTime => ((NR10 & 0x70) >> 4) / 128;
        public bool SweepDecrease => ((NR10 & 0x8) == 0x8);
        public int SweepTimes => (NR10 & 0x7);
        public double WaveDuty
        {
            get
            {
                int number = (NR11 & 0xC0) >> 6;

                switch (number)
                {
                    case 0: return 10.0 / 8.0;
                    case 1: return 10.0 / 4.0;
                    case 2: return 10.0 / 2.0;
                    case 3: return 10.0 / (4.0 / 3.0);
                    default: throw new Exception();
                }
            }
        }
        public double SoundLength => NR11 & 0x3F;
        public int Volume => (NR12 & 0xF0) >> 4;
        public bool VolumeDecrease => (NR12 & 0x8) == 0x8;
        public int NumberVolumeSweep => (NR12 & 0x7);
        public double Frequency => 131072.0 / (2048 - (((NR14 & 0x3) << 8) | NR13));
        public bool StopOutputWhenLengthExpires => (NR14 & 0x40) == 0x40;
        public bool RestartSound => (NR14 & 0x80) == 0x80;
    }
}