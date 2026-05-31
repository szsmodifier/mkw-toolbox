using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class CMPR : ImageFormat
    {
        public override int BitsPerPixel => 4;
        public override int BlockWidth => 8;
        public override int BlockHeight => 8;
        public override byte[]? LastGeneratedPalette { get; protected set; }

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<uint> pixels = new List<uint>();

            for (int i = 0; i < buffer.Length; i += 32)
            {
                uint[] blockPixels = new uint[64];

                for (int subBlock = 0; subBlock < 4; subBlock++)
                {
                    int subOffset = i + (subBlock * 8);

                    int xOffset = (subBlock % 2) * 4;
                    int yOffset = (subBlock / 2) * 4;

                    ushort color0 = (ushort)((buffer[subOffset] << 8) | buffer[subOffset + 1]);
                    ushort color1 = (ushort)((buffer[subOffset + 2] << 8) | buffer[subOffset + 3]);

                    uint indices = (uint)((buffer[subOffset + 4] << 24) |
                                          (buffer[subOffset + 5] << 16) |
                                          (buffer[subOffset + 6] << 8) |
                                           buffer[subOffset + 7]);

                    uint[] palette = new uint[4];
                    palette[0] = RGB565ToARGB8(color0);
                    palette[1] = RGB565ToARGB8(color1);

                    if (color0 > color1)
                    {
                        palette[2] = MixColors(palette[0], palette[1], 2, 1);
                        palette[3] = MixColors(palette[0], palette[1], 1, 2);
                    }
                    else
                    {
                        palette[2] = MixColors(palette[0], palette[1], 1, 1);
                        palette[3] = 0x00000000;
                    }

                    for (int py = 0; py < 4; py++)
                    {
                        for (int px = 0; px < 4; px++)
                        {
                            int shift = 30 - ((py * 4 + px) * 2);
                            uint index = (indices >> shift) & 0x03;

                            int finalX = xOffset + px;
                            int finalY = yOffset + py;

                            blockPixels[finalY * 8 + finalX] = palette[index];
                        }
                    }
                }

                pixels.AddRange(blockPixels);
            }

            return pixels.ToArray();
        }

        private uint RGB565ToARGB8(ushort color)
        {
            uint r = (uint)((color >> 11) & 0x1F);
            uint g = (uint)((color >> 5) & 0x3F);
            uint b = (uint)(color & 0x1F);

            r = (r << 3) | (r >> 2);
            g = (g << 2) | (g >> 4);
            b = (b << 3) | (b >> 2);

            return ((uint)0xFF << 24) | (r << 16) | (g << 8) | b;
        }

        private uint MixColors(uint colorA, uint colorB, uint weightA, uint weightB)
        {
            uint rA = (colorA >> 16) & 0xFF;
            uint gA = (colorA >> 8) & 0xFF;
            uint bA = colorA & 0xFF;

            uint rB = (colorB >> 16) & 0xFF;
            uint gB = (colorB >> 8) & 0xFF;
            uint bB = colorB & 0xFF;

            uint wSum = weightA + weightB;

            uint r = (rA * weightA + rB * weightB) / wSum;
            uint g = (gA * weightA + gB * weightB) / wSum;
            uint b = (bA * weightA + bB * weightB) / wSum;

            return ((uint)0xFF << 24) | (r << 16) | (g << 8) | b;
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<byte> buffer = new List<byte>(pixels.Length / 2);

            for (int i = 0; i < pixels.Length; i += 64)
            {
                for (int subBlock = 0; subBlock < 4; subBlock++)
                {
                    int startX = (subBlock % 2) * 4;
                    int startY = (subBlock / 2) * 4;

                    uint[] subPixels = new uint[16];
                    for (int py = 0; py < 4; py++)
                    {
                        for (int px = 0; px < 4; px++)
                        {
                            subPixels[py * 4 + px] = pixels[i + ((startY + py) * 8) + (startX + px)];
                        }
                    }
                    buffer.AddRange(CompressDXT1Block(subPixels));
                }
            }
            return buffer.ToArray();
        }

        private byte[] CompressDXT1Block(uint[] pixels)
        {
            uint minColor = 0xFFFFFFFF, maxColor = 0;
            int minLum = 256, maxLum = -1;
            bool hasTransparent = false;

            foreach (uint p in pixels)
            {
                byte a = (byte)((p >> 24) & 0xFF);
                if (a < 128)
                {
                    hasTransparent = true;
                    continue;
                }

                int r = (int)((p >> 16) & 0xFF);
                int g = (int)((p >> 8) & 0xFF);
                int b = (int)(p & 0xFF);
                int lum = (r * 77 + g * 150 + b * 29) >> 8;

                if (lum > maxLum) { maxLum = lum; maxColor = p; }
                if (lum < minLum) { minLum = lum; minColor = p; }
            }

            if (maxLum == -1)
            {
                return new byte[] { 0, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0xFF };
            }

            ushort max565 = RGBTo565(maxColor);
            ushort min565 = RGBTo565(minColor);

            if (hasTransparent)
            {
                if (max565 > min565)
                {
                    ushort temp = max565; max565 = min565; min565 = temp;
                    uint tempC = maxColor; maxColor = minColor; minColor = tempC;
                }
            }
            else
            {
                if (max565 < min565)
                {
                    ushort temp = max565; max565 = min565; min565 = temp;
                    uint tempC = maxColor; maxColor = minColor; minColor = tempC;
                }
                else if (max565 == min565 && max565 != 0xFFFF)
                {
                    max565++;
                }
            }

            uint[] palette = new uint[4];
            palette[0] = maxColor;
            palette[1] = minColor;

            if (!hasTransparent)
            {
                palette[2] = MixColors(maxColor, minColor, 2, 1);
                palette[3] = MixColors(maxColor, minColor, 1, 2);
            }
            else
            {
                palette[2] = MixColors(maxColor, minColor, 1, 1);
                palette[3] = 0x00000000;
            }

            uint indices = 0;
            for (int i = 0; i < 16; i++)
            {
                uint bestIndex = 0;

                byte a = (byte)((pixels[i] >> 24) & 0xFF);
                if (hasTransparent && a < 128)
                {
                    bestIndex = 3;
                }
                else
                {
                    int minDistance = int.MaxValue;
                    int colorsToCheck = hasTransparent ? 3 : 4;

                    for (uint j = 0; j < colorsToCheck; j++)
                    {
                        int dist = ColorDistance(pixels[i], palette[j]);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            bestIndex = j;
                        }
                    }
                }

                indices |= (bestIndex << (30 - (i * 2)));
            }

            return new byte[]
            {
                (byte)(max565 >> 8), (byte)(max565 & 0xFF),
                (byte)(min565 >> 8), (byte)(min565 & 0xFF),
                (byte)((indices >> 24) & 0xFF), (byte)((indices >> 16) & 0xFF),
                (byte)((indices >> 8) & 0xFF), (byte)(indices & 0xFF)
            };
        }

        private ushort RGBTo565(uint color)
        {
            int r = (int)((color >> 16) & 0xFF) >> 3;
            int g = (int)((color >> 8) & 0xFF) >> 2;
            int b = (int)(color & 0xFF) >> 3;
            return (ushort)((r << 11) | (g << 5) | b);
        }

        private int ColorDistance(uint c1, uint c2)
        {
            int r = (int)(((c1 >> 16) & 0xFF) - ((c2 >> 16) & 0xFF));
            int g = (int)(((c1 >> 8) & 0xFF) - ((c2 >> 8) & 0xFF));
            int b = (int)((c1 & 0xFF) - (c2 & 0xFF));
            return (r * r) + (g * g) + (b * b);
        }
    }
}
