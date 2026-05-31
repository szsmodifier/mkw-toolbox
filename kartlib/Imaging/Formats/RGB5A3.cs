using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class RGB5A3 : ImageFormat
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
                bool alphaEnabled = ((uint)buffer[i] >> 7) == 0;
                uint pixel;
                if (alphaEnabled)
                {
                    uint A = (((uint)buffer[i] >> 4) & 0x7) * 0x20;
                    uint R = ((uint)buffer[i] & 0xF) * 0x11;
                    uint G = ((uint)buffer[i + 1] >> 4) * 0x11;
                    uint B = ((uint)buffer[i + 1] & 0xF) * 0x11;

                    pixel = (A << 24) | (R << 16) | (G << 8) | B;
                }
                else
                {
                    uint R = (((uint)buffer[i] >> 2) & 0x1F) * 0x8;
                    uint G = ((((uint)buffer[i] & 0x3) << 3) | ((uint)buffer[i + 1] >> 5)) * 0x8;
                    uint B = ((uint)buffer[i + 1] & 0x1F) * 0x8;

                    pixel = (uint)((0xFF << 24) | (R << 16) | (G << 8) | B);
                }
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

                byte a = (byte)((p >> 24) & 0xFF);
                byte r = (byte)((p >> 16) & 0xFF);
                byte g = (byte)((p >> 8) & 0xFF);
                byte b = (byte)(p & 0xFF);

                ushort rgb5a3 = 0;

                if (a >= 224)
                {
                    int r5 = r >> 3;
                    int g5 = g >> 3;
                    int b5 = b >> 3;
                    rgb5a3 = (ushort)(0x8000 | (r5 << 10) | (g5 << 5) | b5);
                }
                else
                {
                    int a3 = a >> 5;
                    int r4 = r >> 4;
                    int g4 = g >> 4;
                    int b4 = b >> 4;
                    rgb5a3 = (ushort)((a3 << 12) | (r4 << 8) | (g4 << 4) | b4);
                }

                buffer.Add((byte)(rgb5a3 >> 8));
                buffer.Add((byte)(rgb5a3 & 0xFF));
            }

            return buffer.ToArray();
        }
    }
}
