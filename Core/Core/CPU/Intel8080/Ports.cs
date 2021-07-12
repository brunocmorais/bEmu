namespace bEmu.Core.CPU.Intel8080
{
    public struct Ports
    {
        public byte Read1 { get; set; }
        public byte Read2 { get; set; }
        public byte Shift1 { get; set; }
        public byte Shift0 { get; set; }
        public byte Write2 { get; set; }
        public byte Write3 { get; set; }
        public byte Write5 { get; set; }
    }
}