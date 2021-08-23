using bEmu.Core.Audio;
using bEmu.Core.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using bEmu.Core.Extensions;

namespace bEmu.MonoGame
{
    public class MusicPlayer : Game
    {
        DynamicSoundEffectInstance sound;
        private GraphicsDeviceManager graphics;

        public MusicPlayer()
        {
            graphics = new GraphicsDeviceManager(this); 
        }

        protected override void Initialize()
        {
            sound = new DynamicSoundEffectInstance(APU.SampleRate, AudioChannels.Stereo);

            var bytes = FileManager.Read("/home/bruno/Projetos/NET/bEmu/Systems/Assets/Generic8080/shot.wav").ToBytes();
            var buffer = new byte[APU.BufferSize];

            sound.Play();

            for (int i = 0; i < bytes.Length / APU.BufferSize; i++)
            {
                for (int j = 0; j < APU.BufferSize; j++)
                buffer[j] = bytes[(i * APU.BufferSize) + j]; 

                sound.SubmitBuffer(buffer);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // if (sound.PendingBufferCount == 0)
            //     Exit();
        }
    }
}