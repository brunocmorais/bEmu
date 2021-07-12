using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public class SuperEagleScaler : BaseScaler
    {
        public SuperEagleScaler() : base(2)
        {
        }

        public override void Update(int frame)
        {
            var pixel2x = new Pixel2x();
            int xScale = 0;
            int yScale = 0;

            for (int x = 0; x < Framebuffer.Width; x++)
            {
                xScale = 0;

                for (int y = 0; y < Framebuffer.Height; y++)
                {
                    uint color4, color5, color6;
                    uint color1, color2, color3;
                    uint colorA0, colorA1, colorA2, colorA3,
                        colorB0, colorB1, colorB2, colorB3,
                        colorS1, colorS2;
                    uint product1a, product1b,
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
                    {
                        product1b = product2a = color2;
                        if ((color1 == color2 && color6 == colorS2) ||
                            (color2 == colorA1 && color6 == colorB2))
                        {
                            product1a = ScalerHelpers.Interpolate (color2, color5);
                            product1a = ScalerHelpers.Interpolate (color2, product1a);
                            product2b = ScalerHelpers.Interpolate (color2, color3);
                            product2b = ScalerHelpers.Interpolate (color2, product2b);
                        }
                        else
                        {
                            product1a = ScalerHelpers.Interpolate (color5, color6);
                            product2b = ScalerHelpers.Interpolate (color2, color3);
                        }
                    }
                    else
                    {
                        if (color5 == color3 && color2 != color6)
                        {
                            product2b = product1a = color5;
                            if ((colorB1 == color5 && color3 == colorA2) ||
                                (color4 == color5 && color3 == colorS1))
                            {
                                product1b = ScalerHelpers.Interpolate (color5, color6);
                                product1b = ScalerHelpers.Interpolate (color5, product1b);
                                product2a = ScalerHelpers.Interpolate (color5, color2);
                                product2a = ScalerHelpers.Interpolate (color5, product2a);
                            }
                            else
                            {
                                product1b = ScalerHelpers.Interpolate (color5, color6);
                                product2a = ScalerHelpers.Interpolate (color2, color3);
                            }
                        }
                        else
                        {
                            if (color5 == color3 && color2 == color6 && color5 != color6)
                            {
                                int r = 0;

                                r += ScalerHelpers.GetResult1 (color6, color5, color1, colorA1);
                                r += ScalerHelpers.GetResult1 (color6, color5, color4, colorB1);
                                r += ScalerHelpers.GetResult1 (color6, color5, colorA2, colorS1);
                                r += ScalerHelpers.GetResult1 (color6, color5, colorB2, colorS2);

                                if (r > 0)
                                {
                                    product1b = product2a = color2;
                                    product1a = product2b = ScalerHelpers.Interpolate (color5, color6);
                                }
                                else
                                    if (r < 0)
                                    {
                                        product2b = product1a = color5;
                                        product1b = product2a = ScalerHelpers.Interpolate (color5, color6);
                                    }
                                    else
                                    {
                                        product2b = product1a = color5;
                                        product1b = product2a = color2;
                                    }
                            }
                            else
                            {

                                if ((color2 == color5) || (color3 == color6))
                                {
                                    product1a = color5;
                                    product2a = color2;
                                    product1b = color6;
                                    product2b = color3;

                                }
                                else
                                {
                                    product1b = product1a = ScalerHelpers.Interpolate (color5, color6);
                                    product1a = ScalerHelpers.Interpolate (color5, product1a);
                                    product1b = ScalerHelpers.Interpolate (color6, product1b);

                                    product2a = product2b = ScalerHelpers.Interpolate (color2, color3);
                                    product2a = ScalerHelpers.Interpolate (color2, product2a);
                                    product2b = ScalerHelpers.Interpolate (color3, product2b);
                                }
                            }
                        }
                    }

                    pixel2x.P1 = product1a;
                    pixel2x.P2 = product1b;
                    pixel2x.P3 = product2a;
                    pixel2x.P4 = product2b;

                    ScaledFramebuffer.SetPixel2x(pixel2x, xScale, yScale);

                    xScale += ScaleFactor;
                }

                yScale += ScaleFactor;
            };

            base.Update(frame);
        }
    }
}