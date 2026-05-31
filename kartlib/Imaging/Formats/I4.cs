using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class I4 : ImageFormat
    {
        public override int BitsPerPixel => 4;
        public override int BlockWidth   => 8;
        public override int BlockHeight  => 8;
        public override byte[]? LastGeneratedPalette { get; protected set; }

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<uint> formattedData = new List<uint>();
            for(int i = 0; i < buffer.Length; i++)
            {
                uint pixel = buffer[i];

                uint pixel1 = (pixel >> 4) * 0x11;
                pixel1 = (uint)((0xFF << 24) | (pixel1 << 16) | (pixel1 << 8) | pixel1);
                formattedData.Add(pixel1);

                uint pixel2 = (pixel & 0xF) * 0x11;
                pixel2 = (uint)((0xFF << 24) | (pixel2 << 16) | (pixel2 << 8) | pixel2);
                formattedData.Add(pixel2);
            }
            return formattedData.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<byte> buffer = new List<byte>(pixels.Length / 2);

            for (int i = 0; i < pixels.Length; i += 2)
            {
                uint p1 = pixels[i];
                uint p2 = pixels[i + 1];

                byte i1 = (byte)((((p1 >> 16) & 0xFF) * 77 + ((p1 >> 8) & 0xFF) * 150 + (p1 & 0xFF) * 29) >> 8);
                byte i2 = (byte)((((p2 >> 16) & 0xFF) * 77 + ((p2 >> 8) & 0xFF) * 150 + (p2 & 0xFF) * 29) >> 8);

                byte compressedI1 = (byte)(i1 >> 4);
                byte compressedI2 = (byte)(i2 >> 4);

                buffer.Add((byte)((compressedI1 << 4) | compressedI2));
            }

            return buffer.ToArray();
        }
    }
}
