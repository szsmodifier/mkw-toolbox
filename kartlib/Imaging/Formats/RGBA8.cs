using kartlib.Serial;

namespace kartlib.Imaging.Formats
{
    public class RGBA8 : ImageFormat
    {
        public override int BitsPerPixel => 32;
        public override int BlockWidth   => 4;
        public override int BlockHeight  => 4;
        public override byte[]? LastGeneratedPalette { get; protected set; }

        protected override uint[] DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<uint> pixels = new();

            List<uint> A = new();
            List<uint> R = new();
            List<uint> G = new();
            List<uint> B = new();

            for(int i = 0; i < buffer.Length;)
            {
                for(int j = 0; j < 16; j++)
                {
                    A.Add( buffer[i++] );
                    R.Add( buffer[i++] );
                }

                for(int j = 0; j < 16; j++)
                {
                    G.Add( buffer[i++] );
                    B.Add( buffer[i++] );
                }
            }

            for(int i = 0; i < A.Count; i++)
            {
                uint pixel = (A[i] << 24) | (R[i] << 16) | (G[i] << 8) | B[i];
                pixels.Add(pixel);
            }

            return pixels.ToArray();
        }

        protected override byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            List<byte> buffer = new List<byte>(pixels.Length * 4);

            for (int i = 0; i < pixels.Length; i += 16)
            {
                for (int j = 0; j < 16; j++)
                {
                    uint p = pixels[i + j];
                    buffer.Add((byte)((p >> 24) & 0xFF));
                    buffer.Add((byte)((p >> 16) & 0xFF));
                }

                for (int j = 0; j < 16; j++)
                {
                    uint p = pixels[i + j];
                    buffer.Add((byte)((p >> 8) & 0xFF));
                    buffer.Add((byte)(p & 0xFF));
                }
            }

            return buffer.ToArray();
        }
    }
}
