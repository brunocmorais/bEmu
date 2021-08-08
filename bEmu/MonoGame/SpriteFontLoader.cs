using bEmu.Core.IO;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace bEmu.MonoGame
{
    public static class SpriteFontLoader
    {
        private static readonly CharacterRange[] characterRanges = new[]
        {
            CharacterRange.BasicLatin,
            CharacterRange.Latin1Supplement,
            CharacterRange.LatinExtendedA
        };

        public static SpriteFont Get(string fontName, GraphicsDevice graphicsDevice, int size)
        {
            return TtfFontBaker.Bake(AssetLoader.Load(fontName), size, 512, 512, characterRanges)
                .CreateSpriteFont(graphicsDevice);
        }
    }
}