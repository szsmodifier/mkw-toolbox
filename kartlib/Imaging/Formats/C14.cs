using System;
using System.Collections.Generic;
using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class C14 : C8
    {
        public override int BitsPerPixel => 16;
        public override int BlockWidth => 4;
        public override int BlockHeight => 4;

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            if (paletteData == null) return Array.Empty<uint>();

            uint[] palette = DecodePalette(paletteData, paletteFormat);
            List<uint> pixels = new List<uint>(buffer.Length / 2);

            for (int i = 0; i < buffer.Length; i += 2)
            {
                ushort indexData = (ushort)((buffer[i] << 8) | buffer[i + 1]);

                int index = indexData & 0x3FFF;

                pixels.Add(index < palette.Length ? palette[index] : 0x00000000);
            }
            return pixels.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            TPL._PaletteHeader.PaletteFormat format = paletteFormat ?? TPL._PaletteHeader.PaletteFormat.RGB5A3;

            var (paletteBytes, colorToIndex) = BuildPalette(pixels, 16384, format);

            typeof(C8).GetProperty("LastGeneratedPalette").SetValue(this, paletteBytes);

            List<byte> buffer = new List<byte>(pixels.Length * 2);

            for (int i = 0; i < pixels.Length; i++)
            {
                int index = colorToIndex[pixels[i]];

                ushort indexData = (ushort)(index & 0x3FFF);

                buffer.Add((byte)(indexData >> 8));
                buffer.Add((byte)(indexData & 0xFF));
            }

            return buffer.ToArray();
        }
    }
}