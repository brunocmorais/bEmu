using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu.Components
{
    public interface IGraphicsDevice 
    {

    }

    public class GraphicsDeviceAdapter : GraphicsDevice, IGraphicsDevice
    {
        public GraphicsDeviceAdapter(GraphicsDevice graphicsDevice) : 
            base(graphicsDevice.Adapter, graphicsDevice.GraphicsProfile, graphicsDevice.PresentationParameters) { }
    }

    public interface ISpriteBatch
    {
        
    }

    public class SpriteBatchAdapter : SpriteBatch, ISpriteBatch
    {
        public SpriteBatchAdapter(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }
    }

    public struct Rect
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    public struct Vec2
    {
        public int X { get; }
        public int Y { get; }

        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Drawer
    {
        public IGraphicsDevice GraphicsDevice { get; }
        public ISpriteBatch SpriteBatch { get; }

        public Drawer(IGraphicsDevice graphicsDevice, ISpriteBatch spriteBatch)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;
        }

        public void Begin()
        {
            (SpriteBatch as SpriteBatch).Begin();
        }

        public void End()
        {
            (SpriteBatch as SpriteBatch).End();
        }

        public void Draw(Texture texture, Rect destinationRectangle, TextureColor color)
        {
            //(SpriteBatch as SpriteBatch).Draw(texture.)
        }
    }

    public class Texture
    {
        private Texture2D texture;
        public int Width { get; }
        public int Height { get; }

        public Texture(Drawer drawer, byte[] data, int width, int height)
        {
            texture = new Texture2D(drawer.GraphicsDevice as GraphicsDevice, width, height);
            Width = width;
            Height = height;
        }

        public void SetData(byte[] data)
        {
            texture.SetData(data);
        }

        public void SetData(TextureColor[] colors)
        {
            Color[] color = new Color[colors.Length];

            for (int i = 0; i < color.Length; i++)
                color[i] = Color.FromNonPremultiplied(colors[i].R, colors[i].G, colors[i].B, colors[i].A);

            texture.SetData(color);
        }
    }

    public struct TextureColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public TextureColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}