using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public class _2xSaIScaler : Scaler
    {
        public _2xSaIScaler(IFrameBuffer frameBuffer) : base(frameBuffer, 2)
        {
        }

        public override void Update(int x, int y)
        {
            var pixel2x = new ScaledPixel(2);

            uint colorA, colorB, colorC, colorD,
                 colorE, colorF, colorG, colorH,
                 colorI, colorJ, colorK, colorL,
                 colorM, colorN, colorO, colorP;

            uint product, product1, product2;

            colorI = this[x - 1, y - 1];
            colorE = this[x, y - 1];
            colorF = this[x + 1, y - 1];
            colorJ = this[x + 2, y - 1];

            colorG = this[x - 1, y];
            colorA = this[x, y];
            colorB = this[x + 1, y];
            colorK = this[x + 2, y];

            colorH = this[x - 1, y + 1];
            colorC = this[x, y + 1];
            colorD = this[x + 1, y + 1];
            colorL = this[x + 2, y + 1];

            colorM = this[x - 1, y + 2];
            colorN = this[x, y + 2];
            colorO = this[x + 1, y + 2];
            colorP = this[x + 2, y + 2];

            if ((colorA == colorD) && (colorB != colorC))
            {
                if ( ((colorA == colorE) && (colorB == colorL)) ||
                    ((colorA == colorC) && (colorA == colorF) && (colorB != colorE) && (colorB == colorJ)) )
                    product = colorA;
                else
                    product = ScalerHelpers.Interpolate(colorA, colorB);

                if (((colorA == colorG) && (colorC == colorO)) ||
                    ((colorA == colorB) && (colorA == colorH) && (colorG != colorC) && (colorC == colorM)) )
                    product1 = colorA;
                else
                    product1 = ScalerHelpers.Interpolate(colorA, colorC);

                product2 = colorA;
            }
            else
            {
                if ((colorB == colorC) && (colorA != colorD))
                {
                    if (((colorB == colorF) && (colorA == colorH)) ||
                        ((colorB == colorE) && (colorB == colorD) && (colorA != colorF) && (colorA == colorI)) )
                        product = colorB;
                    else
                        product = ScalerHelpers.Interpolate(colorA, colorB);

                    if (((colorC == colorH) && (colorA == colorF)) ||
                        ((colorC == colorG) && (colorC == colorD) && (colorA != colorH) && (colorA == colorI)) )
                        product1 = colorC;
                    else
                        product1 = ScalerHelpers.Interpolate(colorA, colorC);
                    product2 = colorB;
                }
                else
                {
                    if ((colorA == colorD) && (colorB == colorC))
                    {
                        if (colorA == colorB)
                        {
                            product = colorA;
                            product1 = colorA;
                            product2 = colorA;
                        }
                        else
                        {
                            int r = 0;
                            product1 = ScalerHelpers.Interpolate(colorA, colorC);
                            product = ScalerHelpers.Interpolate(colorA, colorB);

                            r += ScalerHelpers.GetResult1 (colorA, colorB, colorG, colorE/*, colorI*/);
                            r += ScalerHelpers.GetResult2 (colorB, colorA, colorK, colorF/*, colorJ*/);
                            r += ScalerHelpers.GetResult2 (colorB, colorA, colorH, colorN/*, colorM*/);
                            r += ScalerHelpers.GetResult1 (colorA, colorB, colorL, colorO/*, colorP*/);

                            if (r > 0)
                                product2 = colorA;
                            else
                            {
                                if (r < 0)
                                    product2 = colorB;
                                else
                                    product2 = ScalerHelpers.QInterpolate(colorA, colorB, colorC, colorD);
                            }
                        }
                    }
                    else
                    {
                        product2 = ScalerHelpers.QInterpolate(colorA, colorB, colorC, colorD);

                        if ((colorA == colorC) && (colorA == colorF) && (colorB != colorE) && (colorB == colorJ))
                            product = product1 = colorA;
                        else
                        {
                            if ((colorB == colorE) && (colorB == colorD) && (colorA != colorF) && (colorA == colorI))
                                product = colorB;
                            else
                                product = ScalerHelpers.Interpolate(colorA, colorB);

                            if ((colorA == colorB) && (colorA == colorH) && (colorG != colorC) && (colorC == colorM))
                                product1 = colorA;
                            else
                            {
                                if ((colorC == colorG) && (colorC == colorD) && (colorA != colorH) && (colorA == colorI))
                                    product1 = colorC;
                                else
                                    product1 = ScalerHelpers.Interpolate(colorA, colorC);
                            }
                        }
                    }
                }
            }

            pixel2x[0, 0] = colorA;
            pixel2x[1, 0] = product;
            pixel2x[0, 1] = product1;
            pixel2x[1, 1] = product2;

            ScaledFramebuffer.SetScaledPixel(pixel2x, x, y);
        }
    }
}