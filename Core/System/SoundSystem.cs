namespace bEmu.Core.System
{
    public abstract class SoundSystem : System
    {
        protected SoundSystem(string fileName) : base(fileName)
        {
        }

        public abstract void PlaySound();
        public abstract void StopSound();
    }
}