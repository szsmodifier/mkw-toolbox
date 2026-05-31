using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class RGB565 : ImageFormat
    {
        public override int BitsPerPixel => 16;
        public override int BlockWidth   => 4;
        public override int BlockHeight  => 4;
        public override byte[]? LastGeneratedPalette { get; protected set; }

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<uint> pixels = new List<uint>();
            for (int i = 0; i < buffer.Length; i += 2)
            {
                ushort pixelData = (ushort)((buffer[i] << 8) | buffer[i + 1]);

                uint r5 = (uint)((pixelData >> 11) & 0x1F);
                uint g6 = (uint)((pixelData >> 5) & 0x3F);
                uint b5 = (uint)(pixelData & 0x1F);

                uint R = (r5 << 3) | (r5 >> 2);
                uint G = (g6 << 2) | (g6 >> 4);
                uint B = (b5 << 3) | (b5 >> 2);

                uint pixel = ((uint)0xFF << 24) | (R << 16) | (G << 8) | B;
                pixels.Add(pixel);
            }
            return pixels.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<byte> buffer = new List<byte>(pixels.Length * 2);

            for (int i = 0; i < pixels.Length; i++)
            {
                uint pixel = pixels[i];

                byte r = (byte)((pixel >> 16) & 0xFF);
                byte g = (byte)((pixel >> 8) & 0xFF);
                byte b = (byte)(pixel & 0xFF);

                int r5 = r >> 3;
                int g6 = g >> 2;
                int b5 = b >> 3;

                ushort rgb565 = (ushort)((r5 << 11) | (g6 << 5) | b5);

                buffer.Add((byte)(rgb565 >> 8));
                buffer.Add((byte)(rgb565 & 0xFF));
            }

            return buffer.ToArray();
        }
    }
}
