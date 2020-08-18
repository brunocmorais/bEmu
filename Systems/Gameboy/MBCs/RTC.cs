using System;

namespace bEmu.Systems.Gameboy.MBCs
{
    public class RTC
    {
        private int cycles;
        private int counter;
        private bool halt;
        private RTC latchedData;
        private byte latchRegister;
        public byte Mode { get; private set; }
        private TimeSpan ts => new TimeSpan(counter * TimeSpan.TicksPerSecond);
        private byte Seconds
        {
            get => latchedData != null ? latchedData.Seconds : (byte) ts.Seconds;
            set => counter = (int) new TimeSpan(Days, Hours, Minutes, value).TotalSeconds;
        }
        private byte Minutes
        {
            get => latchedData != null ? latchedData.Minutes : (byte) ts.Minutes;
            set => counter = (int) new TimeSpan(Days, Hours, value, Seconds).TotalSeconds;
        } 
        private byte Hours
        {
            get => latchedData != null ? latchedData.Hours : (byte) ts.Hours;
            set => counter = (int) new TimeSpan(Days, value, Minutes, Seconds).TotalSeconds;
        }
        private int Days
        {
            get => latchedData != null ? latchedData.Days : ts.Days;
            set => counter = (int) new TimeSpan(value, Hours, Minutes, Seconds).TotalSeconds;
        }

        public RTC()
        {
            cycles = 0;
            counter = 0;
            Mode = 0;
            halt = true;
            latchRegister = 0;
        }

        RTC(int days, int hours, int minutes, int seconds)
        {
            Days = days;
            Hours = (byte) hours;
            Minutes = (byte) minutes;
            Seconds = (byte) seconds;
        }

        public void SetMode(byte mode)
        {
            Mode = mode;
        }

        public byte Read()
        {
            return GetValue(Mode);
        }

        private byte GetValue(byte mode)
        {
            switch (Mode)
            {
                case 0x08: return Seconds;
                case 0x09: return Minutes;
                case 0x0A: return Hours;
                case 0x0B: return (byte) (Days & 0xFF);
                case 0x0C: return (byte) ((halt ? 0 : 1) << 6 | ((Days & 0x100) >> 9));
                default: return 0;
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
            if (!halt)
            {
                cycles += lastCycleCount;

                while (cycles >= 128)
                {
                    counter++;
                    cycles -= 128;
                }
            }
        }

        public void Latch(byte value)
        {
            if (value == 1 && latchRegister == 0)
                latchedData = new RTC(Days, Hours, Minutes, Seconds);
            else
                latchedData = null;

            latchRegister = value;
        }

        public byte[] Export()
        {
            byte[] bytes = new byte[10];
            Latch(0);

            for (int i = 0x08; i < 0x0C; i++)
                bytes[i - 0x08] = GetValue((byte) i);

            Latch(1);

            for (int i = 0x08; i < 0x0C; i++)
                bytes[i - 0x03] = latchedData.GetValue((byte) i);

            return bytes;
        }
    }
}