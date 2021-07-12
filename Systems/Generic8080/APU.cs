using System;
using System.IO;
using System.Reflection;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Systems.Generic8080
{
    public class APU : Core.Audio.APU
    {
        private readonly float[] waveBuffer;
        private readonly Sound shot;
        private readonly Sound invaderDie;
        private readonly Sound explosion;
        private readonly Sound ufoLowPitch;
        private readonly Sound ufoHighPitch;
        private readonly Sound fastInvader1;
        private readonly Sound fastInvader2;
        private readonly Sound fastInvader3;
        private readonly Sound fastInvader4;
        private SoundInfo lastInfo;

        public APU(ISystem system) : base(system)
        {
            string assetFolder = Path.Combine(Infrastructure.GetProgramLocation(), Generic8080.System.AssetFolder);
            shot = new Sound(Path.Combine(assetFolder, "shot.wav"));
            invaderDie = new Sound(Path.Combine(assetFolder, "invader_die.wav"));
            explosion = new Sound(Path.Combine(assetFolder, "explosion.wav"));
            ufoLowPitch = new Sound(Path.Combine(assetFolder, "ufo_lowpitch.wav"));
            fastInvader1 = new Sound(Path.Combine(assetFolder, "fastinvader1.wav"));
            fastInvader2 = new Sound(Path.Combine(assetFolder, "fastinvader2.wav"));
            fastInvader3 = new Sound(Path.Combine(assetFolder, "fastinvader3.wav"));
            fastInvader4 = new Sound(Path.Combine(assetFolder, "fastinvader4.wav"));
            ufoHighPitch = new Sound(Path.Combine(assetFolder, "ufo_highpitch.wav"), true);
            waveBuffer = new float[BufferSize];
        }

        public override void UpdateBuffer()
        {
            for (int i = 0; i < waveBuffer.Length; i++)
                waveBuffer[i] = 0;

            if (explosion.IsPlaying)
                AddToBuffer(explosion);
            if (ufoHighPitch.IsPlaying)
                AddToBuffer(ufoHighPitch);
            if (ufoLowPitch.IsPlaying)
                AddToBuffer(ufoLowPitch);
            if (invaderDie.IsPlaying)
                AddToBuffer(invaderDie);
            if (shot.IsPlaying)
                AddToBuffer(shot);
            if (fastInvader1.IsPlaying)
                AddToBuffer(fastInvader1);
            if (fastInvader2.IsPlaying)
                AddToBuffer(fastInvader2);
            if (fastInvader3.IsPlaying)
                AddToBuffer(fastInvader3);
            if (fastInvader4.IsPlaying)
                AddToBuffer(fastInvader4);            

            for (int i = 0; i < Buffer.Length; i++)
            {
                float wave = Math.Clamp(waveBuffer[i], -1.0f, 1.0f);
                sbyte value = (sbyte)(wave * sbyte.MaxValue);
                Buffer[i] = (byte) (value);
            }
        }

        public override void Update(int cycles)
        {
            var state = System.State as Systems.Generic8080.State;
            var info = new SoundInfo(state.Ports.Write3, state.Ports.Write5);

            if (info.PlayShot && !lastInfo.PlayShot)
                shot.Play();
            else if (!info.PlayShot)
                shot.Stop();

            if (info.PlayExplosion && !lastInfo.PlayExplosion)
                explosion.Play();
            else if (!info.PlayExplosion)
                explosion.Stop();

            if (info.PlayInvaderDie && !lastInfo.PlayInvaderDie)
                invaderDie.Play();
            else if (!info.PlayInvaderDie)
                invaderDie.Stop();

            if (info.PlayFastInvader1 && !lastInfo.PlayFastInvader1)
                fastInvader1.Play();
            else if (!info.PlayFastInvader1)
                fastInvader1.Stop();

            if (info.PlayFastInvader2 && !lastInfo.PlayFastInvader2)
                fastInvader2.Play();
            else if (!info.PlayFastInvader2)
                fastInvader2.Stop();

            if (info.PlayFastInvader3 && !lastInfo.PlayFastInvader3)
                fastInvader3.Play();
            else if (!info.PlayFastInvader3)
                fastInvader3.Stop();

            if (info.PlayFastInvader4 && !lastInfo.PlayFastInvader4)
                fastInvader4.Play();
            else if (!info.PlayFastInvader4)
                fastInvader4.Stop();

            if (info.PlayUfoLowPitch && !lastInfo.PlayUfoLowPitch)
                ufoLowPitch.Play();
            else if (!info.PlayUfoLowPitch)
                ufoLowPitch.Stop();

            if (info.PlayUfoHighPitch && !lastInfo.PlayUfoHighPitch)
                ufoHighPitch.Play();
            else if (!info.PlayUfoHighPitch)
                ufoHighPitch.Stop();

            lastInfo = info;
        }

        private void AddToBuffer(Sound sound)
        {
            for (int i = 0; i < waveBuffer.Length; i++)
                waveBuffer[i] += ((sound.Next * 0.5f) / (sbyte.MaxValue));
        }
    }
}