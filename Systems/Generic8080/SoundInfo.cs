namespace bEmu.Systems.Generic8080
{
    public struct SoundInfo
    {
        public SoundInfo(byte write3, byte write5)
        {
            PlayShot = ((write3 & (1 << 1)) >> 1) == 1;
            PlayExplosion = ((write3 & (1 << 2)) >> 2) == 1;
            PlayInvaderDie = ((write3 & (1 << 3)) >> 3) == 1;
            PlayFastInvader1 = ((write5 & (1 << 0)) >> 0) == 1;
            PlayFastInvader2 = ((write5 & (1 << 1)) >> 1) == 1;
            PlayFastInvader3 = ((write5 & (1 << 2)) >> 2) == 1;
            PlayFastInvader4 = ((write5 & (1 << 3)) >> 3) == 1;
            PlayUfoLowPitch = ((write5 & (1 << 4)) >> 4) == 1;
            PlayUfoHighPitch = (write3 & 1) == 1;
        }

        public bool PlayShot { get; }
        public bool PlayExplosion { get; }
        public bool PlayInvaderDie { get; }
        public bool PlayFastInvader1 { get; }
        public bool PlayFastInvader2 { get; }
        public bool PlayFastInvader3 { get; }
        public bool PlayFastInvader4 { get; }
        public bool PlayUfoLowPitch { get; }
        public bool PlayUfoHighPitch { get; }
    }
}