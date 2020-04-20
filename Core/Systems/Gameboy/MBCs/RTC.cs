using System;

namespace bEmu.Core.Systems.Gameboy.MBCs
{
    public class RTC
    {
        int cycles;
        int counter;
        public byte Mode { get; private set; }
        bool halt;
        public bool Latched { get; set; }

        public RTC()
        {
            cycles = 0;
            counter = 0;
            Mode = 0;
            halt = true;
            Latched = false;
        }

        TimeSpan ts => new TimeSpan(counter * TimeSpan.TicksPerSecond);
        byte Seconds
        {
            get => (byte) ts.Seconds;
            set => counter = (int) new TimeSpan(Days, Hours, Minutes, value).TotalSeconds;
        }
        byte Minutes
        {
            get => (byte) ts.Minutes;
            set => counter = (int) new TimeSpan(Days, Hours, value, Seconds).TotalSeconds;
        } 
        byte Hours
        {
            get => (byte) ts.Hours;
            set => counter = (int) new TimeSpan(Days, value, Minutes, Seconds).TotalSeconds;
        }
        int Days
        {
            get => ts.Days;
            set => counter = (int) new TimeSpan(value, Hours, Minutes, Seconds).TotalSeconds;
        }

        public void SetMode(byte mode)
        {
            Mode = mode;
        }

        public byte Read()
        {
            switch (Mode)
            {
                case 0x08: return Seconds;
                case 0x09: return Minutes;
                case 0x0A: return Hours;
                case 0x0B: return (byte) (Days & 0xFF);
                case 0x0C: return (byte) ((halt ? 0 : 1) << 6 | ((Days & 0x100) >> 9));
                default: throw new Exception();
            }
        }

        public void Write(byte value)
        {
            switch (Mode)
            {
                case 0x08: Seconds = value; break;
                case 0x09: Minutes = value; break;
                case 0x0A: Hours = value; break;
                case 0x0B: Days = (byte) (value & 0xFF); break;
                case 0x0C: 
                    halt = (value & 0x40) == 0x40;
                    Days = ((value & 0x1) << 9) | Days;
                break;
                default: throw new Exception();
            }
        }

        public void Tick(int lastCycleCount)
        {
            if (!halt && !Latched)
            {
                cycles += lastCycleCount;

                while (cycles >= 128)
                {
                    counter++;
                    cycles -= 128;
                }
            }
        }
    }
}