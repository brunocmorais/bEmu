using bEmu.Core.Memory;

namespace bEmu.Systems.Gameboy
{
    public class WRAM
    {
        public IMMU MMU { get; }
        private byte[][] wramBanks;
        
        public WRAM(IMMU mmu)
        {
            wramBanks = new byte[8][];

            for (int i = 0; i < wramBanks.Length; i++)
                wramBanks[i] = new byte[4096];

            MMU = mmu;
        }

        public byte this[int index]
        {
            get 
            {
                int bank = (SVBK == 0 ? 1 : SVBK) & 0x7;

                if (index < 0x1000)
                    return wramBanks[0][index]; 
                else
                    return wramBanks[bank][index - 0x1000];
            }
            set 
            {
                int bank = (SVBK == 0 ? 1 : SVBK) & 0x7;

                if (index < 0x1000)
                    wramBanks[0][index] = value; 
                else
                    wramBanks[bank][index - 0x1000] = value;
            }
        }

        public byte SVBK 
        {
            get { return MMU[0xFF70]; }
            set { MMU[0xFF70] = value; }
        }
    }
}