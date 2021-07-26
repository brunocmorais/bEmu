using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public class Super2xSaIScaler : Scaler
    {
        public Super2xSaIScaler(IFrameBuffer frameBuffer) : base(frameBuffer, 2)
        {
        }

        public override void Update(int x, int y)
        {
            var pixel2x = new ScaledPixel(2);
            
            uint color4, color5, color6,
                 color1, color2, color3,
                 colorA0, colorA1, colorA2, colorA3,
                 colorB0, colorB1, colorB2, colorB3,
                 colorS1, colorS2,
                 product1a, product1b,
                 product2a, product2b;

            colorB0 = this[x - 1, y - 1];
            colorB1 = this[x, y - 1];
            colorB2 = this[x + 1, y - 1];
            colorB3 = this[x + 2, y - 1];

            color4  = this[x - 1, y];
            color5  = this[x, y];
            color6  = this[x + 1, y];
            colorS2 = this[x + 2, y];

            color1  = this[x - 1, y + 1];
            color2  = this[x, y + 1];
            color3  = this[x + 1, y + 1];
            colorS1 = this[x + 2, y + 1];

            colorA0 = this[x - 1, y + 2];
            colorA1 = this[x, y + 2];
            colorA2 = this[x + 1, y + 2];
            colorA3 = this[x + 2, y + 2];

            if (color2 == color6 && color5 != color3)
                product2b = product1b = color2;
            else
            if (color5 == color3 && color2 != color6)
                product2b = product1b = color5;
            else
            if (color5 == color3 && color2 == color6 && color5 != color6)
            {
                int	r = 0;

                r += ScalerHelpers.GetResult1(color6, color5, color1,  colorA1);
                r += ScalerHelpers.GetResult1(color6, color5, color4,  colorB1);
                r += ScalerHelpers.GetResult1(color6, color5, colorA2, colorS1);
                r += ScalerHelpers.GetResult1(color6, color5, colorB2, colorS2);

                if (r > 0)
                    product2b = product1b = color6;
                else
                    if (r < 0)
                        product2b = product1b = color5;
                    else
                        product2b = product1b = ScalerHelpers.Interpolate(color5, color6);
            }
            else
            {
                if (color6 == color3 && color3 == colorA1 && color2 != colorA2 && color3 != colorA0)
                    product2b = ScalerHelpers.QInterpolate(color3, color3, color3, color2);
                else
                    if (color5 == color2 && color2 == colorA2 && colorA1 != color3 && color2 != colorA3)
                        product2b = ScalerHelpers.QInterpolate(color2, color2, color2, color3);
                    else
                        product2b = ScalerHelpers.Interpolate(color2, color3);

                if (color6 == color3 && color6 == colorB1 && color5 != colorB2 && color6 != colorB0)
                    product1b = ScalerHelpers.QInterpolate(color6, color6, color6, color5);
                else
                    if (color5 == color2 && color5 == colorB2 && colorB1 != color6 && color5 != colorB3)
                        product1b = ScalerHelpers.QInterpolate(color6, color5, color5, color5);
                    else
                        product1b = ScalerHelpers.Interpolate (color5, color6);
            }

            if (color5 == color3 && color2 != color6 && color4 == color5 && color5 != colorA2)
                product2a = ScalerHelpers.Interpolate(color2, color5);
            else
                if (color5 == color1 && color6 == color5 && color4 != color2 && color5 != colorA0)
                    product2a = ScalerHelpers.Interpolate(color2, color5);
                else
                    product2a = color2;

            if (color2 == color6 && color5 != color3 && color1 == color2 && color2 != colorB2)
                product1a = ScalerHelpers.Interpolate(color2, color5);
            else
                if (color4 == color2 && color3 == color2 && color1 != color5 && color2 != colorB0)
                    product1a = ScalerHelpers.Interpolate(color2, color5);
                else
                    product1a = color5;


            pixel2x[0, 0] = product1a;
            pixel2x[1, 0] = product1b;
            pixel2x[0, 1] = product2a;
            pixel2x[1, 1] = product2b;

            ScaledFramebuffer.SetScaledPixel(pixel2x, x, y);
        }
    }
}