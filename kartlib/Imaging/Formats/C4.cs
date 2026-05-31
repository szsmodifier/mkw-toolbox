using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class C4 : C8
    {
        public override int BitsPerPixel => 4;
        public override int BlockWidth => 8;
        public override int BlockHeight => 8;

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            if (paletteData == null) return Array.Empty<uint>();

            uint[] palette = DecodePalette(paletteData, paletteFormat);
            List<uint> pixels = new List<uint>(buffer.Length * 2);

            for (int i = 0; i < buffer.Length; i++)
            {
                int index1 = (buffer[i] >> 4) & 0x0F;
                int index2 = buffer[i] & 0x0F;

                pixels.Add(index1 < palette.Length ? palette[index1] : 0x00000000);
                pixels.Add(index2 < palette.Length ? palette[index2] : 0x00000000);
            }
            return pixels.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            TPL._PaletteHeader.PaletteFormat format = paletteFormat ?? TPL._PaletteHeader.PaletteFormat.RGB5A3;

            var (paletteBytes, colorToIndex) = BuildPalette(pixels, 16, format);

            typeof(C8).GetProperty("LastGeneratedPalette").SetValue(this, paletteBytes);

            List<byte> buffer = new List<byte>(pixels.Length / 2);

            for (int i = 0; i < pixels.Length; i += 2)
            {
                int index1 = colorToIndex[pixels[i]];
                int index2 = colorToIndex[pixels[i + 1]];

                buffer.Add((byte)((index1 << 4) | index2));
            }

            return buffer.ToArray();
        }
    }
}