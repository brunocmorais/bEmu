using bEmu.Core.Enums;
using bEmu.Core.GamePad;

namespace bEmu.Web.Classes
{
    public class GamePadBuilder : IGamePadBuilder<string>
    {
        public IGamePad Build(string[] pressedKeys)
        {
            var keys = new GamePadKey[pressedKeys.Length];

            for (int i = 0; i < keys.Length; i++)
                keys[i] = GetGamePadKey(pressedKeys[i]);

            return new Core.GamePad.GamePad(keys);
        }

        public GamePadKey GetGamePadKey(string key)
        {
            switch (key)
            {
                case "D0": return GamePadKey.D0;
                case "D1": return GamePadKey.D1;
                case "D2": return GamePadKey.D2;
                case "D3": return GamePadKey.D3;
                case "D4": return GamePadKey.D4;
                case "D5": return GamePadKey.D5;
                case "D6": return GamePadKey.D6;
                case "D7": return GamePadKey.D7;
                case "D8": return GamePadKey.D8;
                case "D9": return GamePadKey.D9;
                case "A": return GamePadKey.A;
                case "B": return GamePadKey.B;
                case "C": return GamePadKey.C;
                case "D": return GamePadKey.D;
                case "E": return GamePadKey.E;
                case "F": return GamePadKey.F;
                case "G": return GamePadKey.G;
                case "H": return GamePadKey.H;
                case "I": return GamePadKey.I;
                case "J": return GamePadKey.J;
                case "K": return GamePadKey.K;
                case "L": return GamePadKey.L;
                case "M": return GamePadKey.M;
                case "N": return GamePadKey.N;
                case "O": return GamePadKey.O;
                case "P": return GamePadKey.P;
                case "Q": return GamePadKey.Q;
                case "R": return GamePadKey.R;
                case "S": return GamePadKey.S;
                case "T": return GamePadKey.T;
                case "U": return GamePadKey.U;
                case "V": return GamePadKey.V;
                case "W": return GamePadKey.W;
                case "X": return GamePadKey.X;
                case "Y": return GamePadKey.Y;
                case "Z": return GamePadKey.Z;
                case "LeftControl": return GamePadKey.LeftControl;
                case "RightControl": return GamePadKey.RightControl;
                case "LeftAlt": return GamePadKey.LeftAlt;
                case "RightAlt": return GamePadKey.RightAlt;
                case "LeftShift": return GamePadKey.LeftShift;
                case "RightShift": return GamePadKey.RightShift;
                case "Space": return GamePadKey.Space;
                case "Enter": return GamePadKey.Enter;
                case "ArrowUp": return GamePadKey.Up;
                case "ArrowDown": return GamePadKey.Down;
                case "ArrowLeft": return GamePadKey.Left;
                case "ArrowRight": return GamePadKey.Right;
                case "Back": return GamePadKey.Back;
                case "Escape": return GamePadKey.Escape;
                case "F1": return GamePadKey.F1;
                case "F2": return GamePadKey.F2;
                case "F3": return GamePadKey.F3;
                case "F4": return GamePadKey.F4;
                case "F5": return GamePadKey.F5;
                case "F6": return GamePadKey.F6;
                case "F7": return GamePadKey.F7;
                case "F8": return GamePadKey.F8;
                case "F9": return GamePadKey.F9;
                case "F10": return GamePadKey.F10;
                case "F11": return GamePadKey.F11;
                case "F12": return GamePadKey.F12;
                default: return 0;
            }
        }
    }
}