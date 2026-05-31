using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class I8 : ImageFormat
    {
        public override int BitsPerPixel => 8;
        public override int BlockWidth   => 8;
        public override int BlockHeight  => 4;
        public override byte[]? LastGeneratedPalette { get; protected set; }

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<uint> result = new List<uint>();
            for(int i = 0; i < buffer.Length; i++)
            {
                uint pixel = buffer[i];    
                pixel = (uint)((0xFF << 24) | (pixel << 16) | (pixel << 8) | pixel);
                result.Add(pixel);
            }
            return result.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<byte> buffer = new List<byte>(pixels.Length);

            for (int i = 0; i < pixels.Length; i++)
            {
                uint p = pixels[i];

                byte intensity = (byte)((((p >> 16) & 0xFF) * 77 + ((p >> 8) & 0xFF) * 150 + (p & 0xFF) * 29) >> 8);
                buffer.Add(intensity);
            }

            return buffer.ToArray();
        }
    }
}
