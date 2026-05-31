using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class IA4 : ImageFormat
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
                uint input = buffer[i];

                uint pixelA = (input >> 4) * 0x11;
                uint pixelI = (input & 0xF) * 0x11;

                uint pixel = (pixelA << 24) | (pixelI << 16) | (pixelI << 8) | pixelI;
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

                byte alpha = (byte)((p >> 24) & 0xFF);
                byte intensity = (byte)((((p >> 16) & 0xFF) * 77 + ((p >> 8) & 0xFF) * 150 + (p & 0xFF) * 29) >> 8);

                byte a4 = (byte)(alpha >> 4);
                byte i4 = (byte)(intensity >> 4);

                buffer.Add((byte)((a4 << 4) | i4));
            }

            return buffer.ToArray();
        }
    }
}
