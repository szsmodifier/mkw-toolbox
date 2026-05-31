using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class C8 : ImageFormat
    {
        public override int BitsPerPixel => 8;
        public override int BlockWidth => 8;
        public override int BlockHeight => 4;
        public override byte[]? LastGeneratedPalette { get; protected set; }

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            if (paletteData == null) return Array.Empty<uint>();

            uint[] palette = DecodePalette(paletteData, paletteFormat);
            List<uint> pixels = new List<uint>(buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
            {
                int index = buffer[i];
                pixels.Add(index < palette.Length ? palette[index] : 0x00000000);
            }
            return pixels.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            TPL._PaletteHeader.PaletteFormat format = paletteFormat ?? TPL._PaletteHeader.PaletteFormat.RGB5A3;

            var (paletteBytes, colorToIndex) = BuildPalette(pixels, 256, format);
            LastGeneratedPalette = paletteBytes;

            List<byte> buffer = new List<byte>(pixels.Length);

            for (int i = 0; i < pixels.Length; i++)
            {
                buffer.Add((byte)colorToIndex[pixels[i]]);
            }

            return buffer.ToArray();
        }

        protected (byte[], Dictionary<uint, int>) BuildPalette(uint[] pixels, int maxColors, TPL._PaletteHeader.PaletteFormat format)
        {
            List<uint> uniqueColors = pixels.Distinct().ToList();
            List<uint> paletteColors;

            if (uniqueColors.Count > maxColors)
            {
                paletteColors = QuantizeColors(uniqueColors, maxColors);
            }
            else
            {
                paletteColors = uniqueColors;
            }

            List<byte> paletteBytes = new List<byte>();
            for (int i = 0; i < paletteColors.Count; i++)
            {
                ushort encodedColor = EncodeColorToPaletteFormat(paletteColors[i], format);
                paletteBytes.Add((byte)(encodedColor >> 8));
                paletteBytes.Add((byte)(encodedColor & 0xFF));
            }

            Dictionary<uint, int> colorToIndex = new Dictionary<uint, int>();
            foreach (uint c in uniqueColors)
            {
                colorToIndex[c] = FindClosestPaletteIndex(c, paletteColors);
            }

            return (paletteBytes.ToArray(), colorToIndex);
        }

        private List<uint> QuantizeColors(List<uint> colors, int maxColors)
        {
            List<List<uint>> buckets = new List<List<uint>> { colors };

            while (buckets.Count < maxColors)
            {
                int maxRange = -1;
                List<uint>? targetBucket = null;
                int targetChannel = 0;

                foreach (var bucket in buckets)
                {
                    if (bucket.Count > 1)
                    {
                        var (channel, range) = GetWidestChannelAndRange(bucket);
                        if (range > maxRange)
                        {
                            maxRange = range;
                            targetBucket = bucket;
                            targetChannel = channel;
                        }
                    }
                }

                if (targetBucket == null) break;

                buckets.Remove(targetBucket);

                if (targetChannel == 0) targetBucket.Sort((a, b) => ((a >> 24) & 0xFF).CompareTo((b >> 24) & 0xFF)); // A
                else if (targetChannel == 1) targetBucket.Sort((a, b) => ((a >> 16) & 0xFF).CompareTo((b >> 16) & 0xFF)); // R
                else if (targetChannel == 2) targetBucket.Sort((a, b) => ((a >> 8) & 0xFF).CompareTo((b >> 8) & 0xFF));  // G
                else targetBucket.Sort((a, b) => (a & 0xFF).CompareTo(b & 0xFF));                                     // B

                int mid = targetBucket.Count / 2;
                buckets.Add(targetBucket.GetRange(0, mid));
                buckets.Add(targetBucket.GetRange(mid, targetBucket.Count - mid));
            }

            List<uint> finalPalette = new List<uint>();
            foreach (var bucket in buckets)
            {
                finalPalette.Add(AverageColor(bucket));
            }

            return finalPalette;
        }

        private (int channel, int range) GetWidestChannelAndRange(List<uint> bucket)
        {
            int minA = 255, maxA = 0, minR = 255, maxR = 0, minG = 255, maxG = 0, minB = 255, maxB = 0;

            foreach (uint c in bucket)
            {
                int a = (int)((c >> 24) & 0xFF);
                int r = (int)((c >> 16) & 0xFF);
                int g = (int)((c >> 8) & 0xFF);
                int b = (int)(c & 0xFF);

                if (a < minA) minA = a; if (a > maxA) maxA = a;
                if (r < minR) minR = r; if (r > maxR) maxR = r;
                if (g < minG) minG = g; if (g > maxG) maxG = g;
                if (b < minB) minB = b; if (b > maxB) maxB = b;
            }

            int rangeA = maxA - minA;
            int rangeR = maxR - minR;
            int rangeG = maxG - minG;
            int rangeB = maxB - minB;

            int maxRange = Math.Max(Math.Max(rangeA, rangeR), Math.Max(rangeG, rangeB));

            if (maxRange == rangeR) return (1, rangeR);
            if (maxRange == rangeG) return (2, rangeG);
            if (maxRange == rangeB) return (3, rangeB);
            return (0, rangeA);
        }

        private uint AverageColor(List<uint> bucket)
        {
            long sumA = 0, sumR = 0, sumG = 0, sumB = 0;
            foreach (uint c in bucket)
            {
                sumA += (c >> 24) & 0xFF;
                sumR += (c >> 16) & 0xFF;
                sumG += (c >> 8) & 0xFF;
                sumB += c & 0xFF;
            }
            int count = bucket.Count;
            return (uint)(((sumA / count) << 24) | ((sumR / count) << 16) | ((sumG / count) << 8) | (sumB / count));
        }

        private int FindClosestPaletteIndex(uint color, List<uint> palette)
        {
            int bestIndex = 0;
            int minDistance = int.MaxValue;

            int a1 = (int)((color >> 24) & 0xFF);
            int r1 = (int)((color >> 16) & 0xFF);
            int g1 = (int)((color >> 8) & 0xFF);
            int b1 = (int)(color & 0xFF);

            for (int i = 0; i < palette.Count; i++)
            {
                uint p = palette[i];
                int a2 = (int)((p >> 24) & 0xFF);
                int r2 = (int)((p >> 16) & 0xFF);
                int g2 = (int)((p >> 8) & 0xFF);
                int b2 = (int)(p & 0xFF);

                int dist = ((a1 - a2) * (a1 - a2)) +
                           ((r1 - r2) * (r1 - r2)) +
                           ((g1 - g2) * (g1 - g2)) +
                           ((b1 - b2) * (b1 - b2));

                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }

        protected ushort EncodeColorToPaletteFormat(uint p, TPL._PaletteHeader.PaletteFormat format)
        {
            byte a = (byte)((p >> 24) & 0xFF);
            byte r = (byte)((p >> 16) & 0xFF);
            byte g = (byte)((p >> 8) & 0xFF);
            byte b = (byte)(p & 0xFF);

            switch (format)
            {
                case TPL._PaletteHeader.PaletteFormat.IA8:
                    byte intensity = (byte)((r * 77 + g * 150 + b * 29) >> 8);
                    return (ushort)((a << 8) | intensity);

                case TPL._PaletteHeader.PaletteFormat.RGB565:
                    return (ushort)(((r >> 3) << 11) | ((g >> 2) << 5) | (b >> 3));

                case TPL._PaletteHeader.PaletteFormat.RGB5A3:
                default:
                    if (a >= 224)
                        return (ushort)(0x8000 | ((r >> 3) << 10) | ((g >> 3) << 5) | (b >> 3));
                    else
                        return (ushort)(((a >> 5) << 12) | ((r >> 4) << 8) | ((g >> 4) << 4) | (b >> 4));
            }
        }
    }
}
