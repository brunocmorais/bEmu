using bEmu.Core;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;

namespace bEmu.Systems.Gameboy
{

    public class Joypad
    {
        private byte activeColumn;
        public byte Column1 { get; set; }
        public byte Column2 { get; set; }

        public Joypad()
        {
            Column1 = 0xF;
            Column2 = 0xF;
        }

        public void SetJoypadColumn(byte column)
        {
            activeColumn = (byte)(column & 0x30);
        }

        public byte GetJoypadInfo()
        {
            if (activeColumn == 0x10)
                return Column1;
            if (activeColumn == 0x20)
                return Column2;

            return 0;
        }

        public void Update(IGamePad gamePad)
        {
            if (gamePad.IsKeyDown(GamePadKey.Z))
                Column1 &= 0xE;
            if (gamePad.IsKeyDown(GamePadKey.X))
                Column1 &= 0xD;
            if (gamePad.IsKeyDown(GamePadKey.RightShift))
                Column1 &= 0xB;
            if (gamePad.IsKeyDown(GamePadKey.Enter))
                Column1 &= 0x7;
            if (gamePad.IsKeyDown(GamePadKey.Right))
                Column2 &= 0xE;
            if (gamePad.IsKeyDown(GamePadKey.Left))
                Column2 &= 0xD;
            if (gamePad.IsKeyDown(GamePadKey.Up))
                Column2 &= 0xB;
            if (gamePad.IsKeyDown(GamePadKey.Down))
                Column2 &= 0x7;

            if (gamePad.IsKeyUp(GamePadKey.Z))
                Column1 |= 0x1;
            if (gamePad.IsKeyUp(GamePadKey.X))
                Column1 |= 0x2;
            if (gamePad.IsKeyUp(GamePadKey.RightShift))
                Column1 |= 0x4;
            if (gamePad.IsKeyUp(GamePadKey.Enter))
                Column1 |= 0x8;
            if (gamePad.IsKeyUp(GamePadKey.Right))
                Column2 |= 0x1;
            if (gamePad.IsKeyUp(GamePadKey.Left))
                Column2 |= 0x2;
            if (gamePad.IsKeyUp(GamePadKey.Up))
                Column2 |= 0x4;
            if (gamePad.IsKeyUp(GamePadKey.Down))
                Column2 |= 0x8;
        }
    }
}