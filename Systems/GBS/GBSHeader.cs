namespace bEmu.Systems.GBS
{
    public class GBSHeader
    {
        public string Identifier { get; }
        public byte Version { get; }
        public byte NumberOfSongs { get; }
        public byte FirstSong { get; }
        public ushort LoadAddress { get; }
        public ushort InitAddress { get; }
        public ushort PlayAddress { get; }
        public ushort SP { get; }
        public byte TimerModulo { get; }
        public byte TimerControl { get; }
        public string Title { get; }
        public string Author { get; }
        public string Copyright { get; }

        public GBSHeader(string identifier, byte version, byte numberOfSongs, byte firstSong, 
            ushort loadAddress, ushort initAddress, ushort playAddress, ushort sp, byte timerModulo, 
            byte timerControl, string title, string author, string copyright)
        {
            Identifier = identifier;
            Version = version;
            NumberOfSongs = numberOfSongs;
            FirstSong = firstSong;
            LoadAddress = loadAddress;
            InitAddress = initAddress;
            PlayAddress = playAddress;
            SP = sp;
            TimerModulo = timerModulo;
            TimerControl = timerControl;
            Title = title;
            Author = author;
            Copyright = copyright;
        }
    }
}