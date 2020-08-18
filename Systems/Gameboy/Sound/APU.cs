using System;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class APU : IAPU
    {
        public const int LengthTiming = 16384;
        public const int EnvelopeTiming = 65536;
        public const int AudioBufferFrames = 1024;
        public const int CyclesAudioSample = 4194304 / (AudioSampleRate + 250);
        public const int AudioSampleRate = 44100;
        public const int BytesPerSample = 2;
        private readonly MMU mmu;
        private readonly SquareChannel squareChannel1;
        private readonly SquareChannel squareChannel2;
        private readonly Channel3 channel3;
        private readonly Channel4 channel4;
        private readonly SoundOscillator oscillator;
        private bool DataObtained { get; set; }
        private SampleData sampleData;
        private int sampleCounter;
        private DAC dac;
        // public byte[] WorkingBuffer 
        // { 
        //     get 
        //     {
        //         if (DataObtained)
        //             return null;

        //         var workingBuffer = new byte[squareChannel1.WorkingBuffer.Length * 2];

        //         for (int i = 0; i < squareChannel1.WorkingBuffer.Length; i++)
        //         {
        //             byte value = squareChannel1.WorkingBuffer[i];
        //             float floatSample = RestrictValue(value, -1.0f, 1.0f);	
        //             short shortSample = (short) (floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);

        //             // workingBuffer[(i * 2)] = (byte) (value == 1 ? 0xFF : 0x00);
        //             // workingBuffer[(i * 2) + 1] = (byte) (value == 1 ? 0xFF : 192);

        //             workingBuffer[(i * 2)] = (byte)shortSample;
        //             workingBuffer[(i * 2) + 1] = (byte)(shortSample >> 8);
        //         }

        //         return workingBuffer;

        //         // var ret = new byte[64];

        //         // for (int i = 0; i < 32; i += 2)
        //         // {
        //         //     if (!squareChannel1.IsRunning)
        //         //         continue;

        //         //     var sample = oscillator.GenerateSquareWave(time, (262144.0 / (2048 - squareChannel1.Frequency)), 1);
        //         //     ret[(i * 2)] = (byte) (sample > 0 ? 255 : 0);
        //         //     ret[(i * 2) + 1] = (byte) (sample > 0 ? 255 : 192);
        //         //     time += 1.0 / AudioSampleRate;
        //         // }

        //         // return ret;
        //     }
        // }
        public double time = 0;
        private int cycleCountLength = 0;
        private int cycleCountEnvelope = 0;
        private int cycleCount = 0;

        public APU(ISystem system)
        {
            var mmu = system.MMU as MMU;

            squareChannel1 = new SquareChannel(mmu, 0x10, 0x11, 0x12, 0x13, 0x14);
            squareChannel2 = new SquareChannel(mmu, 0x20, 0x21, 0x22, 0x23, 0x24);
            channel3 = new Channel3(mmu);
            channel4 = new Channel4(mmu);
            oscillator = new SoundOscillator();
            sampleData = new SampleData();
            dac = new DAC();

            this.mmu = mmu;
        }

        //public bool OutputSO1 => (NR50 & 0x8) == 0x8;
        public int SO1OutputLevel => 8 - (NR50 & 0x7);
        //public bool OutputSO2 => (NR50 & 0x80) == 0x80;
        public int SO2OutputLevel => 8 - ((NR50 & 0x70) >> 4);
        public bool Enabled => (NR52 & 0x80) == 0x80; 

        public bool GetSelectionOutputTerminal(int terminal, int channelNumber)
        {
            int mask = 1 << (channelNumber - 1);

            if (terminal == 2)
                mask <<= 4;

            return (NR51 & mask) == mask;
        }

        public byte NR50
        {
            get { return mmu.IO[0x24]; }
            set { mmu.IO[0x24] = value; }
        }

        public byte NR51
        {
            get { return mmu.IO[0x25]; }
            set { mmu.IO[0x25] = value; }
        }

        public byte NR52
        {
            get { return mmu.IO[0x26]; }
            set { mmu.IO[0x26] = value; }
        }

        public void Cycle(int lastCycleCount)
        {
            CycleLength(lastCycleCount);
            CycleEnvelope(lastCycleCount);
            CycleChannels(lastCycleCount);
            CycleSamples(lastCycleCount);
        }

        private void CycleSamples(int lastCycleCount)
        {
            cycleCount += lastCycleCount;

            if (cycleCount >= CyclesAudioSample)
            {
                sampleData.Square1Left[sampleCounter] = (byte) (GetSelectionOutputTerminal(1, 1) ? (squareChannel1.CurrentSample / SO1OutputLevel) : 0);
                sampleData.Square1Left[sampleCounter] = (byte) (GetSelectionOutputTerminal(2, 1) ? (squareChannel1.CurrentSample / SO2OutputLevel) : 0);

                sampleCounter++;

                if (sampleCounter == AudioBufferFrames)
                {
                    sampleCounter = 0;
                    dac.SetSamples(sampleData);
                }

                cycleCount -= CyclesAudioSample;
            }
        }

        private void CycleChannels(int lastCycleCount)
        {
            squareChannel1.Tick(lastCycleCount);
        }

        private void CycleEnvelope(int lastCycleCount)
        {
            cycleCountEnvelope += lastCycleCount;

            if (cycleCountEnvelope >= EnvelopeTiming)
            {
                squareChannel1.LengthTick();
                cycleCountEnvelope -= EnvelopeTiming;
            }
        }

        private void CycleLength(int lastCycleCount)
        {
            cycleCountLength += lastCycleCount;

            if (cycleCountLength >= LengthTiming)
            {
                squareChannel1.LengthTick();
                cycleCountLength -= LengthTiming;
            }
        }

        public float RestrictValue(float value, float min, float max)
        {
            if (value < min)
                return min;
            
            if (value > max)
                return max;
            
            return value;
        }
    }
}