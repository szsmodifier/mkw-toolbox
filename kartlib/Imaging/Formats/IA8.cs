using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class IA8 : ImageFormat
    {
        public override int BitsPerPixel => 16;
        public override int BlockWidth   => 4;
        public override int BlockHeight  => 4;
        public override byte[]? LastGeneratedPalette { get; protected set; }

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<uint> pixels = new List<uint>();
            for(int i = 0; i < buffer.Length; i += 2)
            {
                uint alpha = buffer[i];
                uint value = buffer[i + 1];
                uint pixel = (alpha << 24) | (value << 16) | (value << 8) | value;
                pixels.Add(pixel);
            }
            return pixels.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<byte> buffer = new List<byte>(pixels.Length * 2);

            for (int i = 0; i < pixels.Length; i++)
            {
                uint p = pixels[i];

                byte alpha = (byte)((p >> 24) & 0xFF);
                byte intensity = (byte)((((p >> 16) & 0xFF) * 77 + ((p >> 8) & 0xFF) * 150 + (p & 0xFF) * 29) >> 8);

                buffer.Add(alpha);
                buffer.Add(intensity);
            }

            return buffer.ToArray();
        }
    }
}
