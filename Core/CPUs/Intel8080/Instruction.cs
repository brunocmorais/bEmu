namespace bEmu.Core.CPUs.Intel8080
{
    public struct Instruction
    {
        public string Mnemonic { get; set; }
        public int Length { get; set; }
        public int Position { get; set; }

        public Instruction(string mnemonic, int length, int position)
        {
            Mnemonic = mnemonic;
            Length = length;
            Position = position;
        }

        public override string ToString()
        {
            string pos = Position.ToString("X").PadLeft(4, '0');
            return $"{pos}: {Mnemonic}";
        }
    }
}