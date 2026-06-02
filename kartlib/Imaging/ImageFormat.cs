using kartlib.Serial;
using System.Drawing;
using System.Drawing.Imaging;

namespace kartlib.Imaging
{
    public abstract class ImageFormat
    {
        public abstract int BitsPerPixel { get; }
        public abstract int BlockWidth { get; }
        public abstract int BlockHeight { get; }
        public abstract byte[]? LastGeneratedPalette { get; protected set; }


        protected virtual uint[]? DecodePixels(byte[] buffer, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null) { return null; }

        private byte[]? SortBlocks(byte[] buffer, int width, int height, byte[]? paletteData, TPL._PaletteHeader.PaletteFormat? paletteFormat)
        {
            uint[]? formattedBuffer = DecodePixels(buffer, paletteData, paletteFormat);
            if (formattedBuffer == null)
                return null;

            int blocksAcross = (int)Math.Ceiling((double)width / BlockWidth);

            List<byte> result = new List<byte>(width * height * 4);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int blockX = x / BlockWidth;
                    int blockY = y / BlockHeight;
                    int blockIndex = (blockY * blocksAcross) + blockX;

                    int blockStart = blockIndex * (BlockWidth * BlockHeight);

                    int inBlockX = x % BlockWidth;
                    int inBlockY = y % BlockHeight;
                    int inBlockOffset = (inBlockY * BlockWidth) + inBlockX;

                    int c = blockStart + inBlockOffset;

                    result.AddRange(BitConverter.GetBytes(formattedBuffer[c]));
                }
            }

            return result.ToArray();
        }

        protected uint[] DecodePalette(byte[]? paletteData, TPL._PaletteHeader.PaletteFormat? format)
        {
            if (paletteData == null || format == null)
                return Array.Empty<uint>();

            List<uint> palette = new List<uint>();

            for (int i = 0; i < paletteData.Length; i += 2)
            {
                ushort colorData = (ushort)((paletteData[i] << 8) | paletteData[i + 1]);
                uint pixel = 0;

                switch (format)
                {
                    case TPL._PaletteHeader.PaletteFormat.IA8:
                        uint alpha = paletteData[i];
                        uint intensity = paletteData[i + 1];
                        pixel = (alpha << 24) | (intensity << 16) | (intensity << 8) | intensity;
                        break;

                    case TPL._PaletteHeader.PaletteFormat.RGB565:
                        uint r5 = (uint)((colorData >> 11) & 0x1F);
                        uint g6 = (uint)((colorData >> 5) & 0x3F);
                        uint b5 = (uint)(colorData & 0x1F);

                        uint R = (r5 << 3) | (r5 >> 2);
                        uint G = (g6 << 2) | (g6 >> 4);
                        uint B = (b5 << 3) | (b5 >> 2);

                        pixel = ((uint)0xFF << 24) | (R << 16) | (G << 8) | B;
                        break;

                    case TPL._PaletteHeader.PaletteFormat.RGB5A3:
                        bool alphaEnabled = (colorData & 0x8000) == 0;
                        if (alphaEnabled)
                        {
                            uint A = (uint)((colorData >> 12) & 0x7) * 0x20;
                            uint R3 = (uint)((colorData >> 8) & 0xF) * 0x11;
                            uint G3 = (uint)((colorData >> 4) & 0xF) * 0x11;
                            uint B3 = (uint)(colorData & 0xF) * 0x11;
                            pixel = (A << 24) | (R3 << 16) | (G3 << 8) | B3;
                        }
                        else
                        {
                            uint R4 = (uint)((colorData >> 10) & 0x1F);
                            uint G4 = (uint)((colorData >> 5) & 0x1F);
                            uint B4 = (uint)(colorData & 0x1F);

                            uint Rf = (R4 << 3) | (R4 >> 2);
                            uint Gf = (G4 << 3) | (G4 >> 2);
                            uint Bf = (B4 << 3) | (B4 >> 2);

                            pixel = ((uint)0xFF << 24) | (Rf << 16) | (Gf << 8) | Bf;
                        }
                        break;
                }
                palette.Add(pixel);
            }

            return palette.ToArray();
        }

        public Bitmap? ToBitmap(byte[] buffer, int width, int height, byte[]? paletteData = null, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            byte[]? formattedBuffer = SortBlocks(buffer, width, height, paletteData, paletteFormat);
            if(formattedBuffer == null) 
                return null;

            PixelFormat format = PixelFormat.Format32bppArgb;
            Bitmap result = new Bitmap(width, height, format);
            BitmapData bitmapData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);
            System.Runtime.InteropServices.Marshal.Copy(formattedBuffer, 0, bitmapData.Scan0, formattedBuffer.Length);
            result.UnlockBits(bitmapData);
            return result;
        }

        public byte[]? FromBitmap(Bitmap bitmap, TPL._PaletteHeader.PaletteFormat? paletteFormat = null)
        {
            int width = (int)Math.Ceiling((double)bitmap.Width / BlockWidth) * BlockWidth;
            int height = (int)Math.Ceiling((double)bitmap.Height / BlockHeight) * BlockHeight;

            Bitmap paddedBitmap = bitmap;
            if (bitmap.Width != width || bitmap.Height != height)
            {
                paddedBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(paddedBitmap))
                {
                    g.DrawImage(bitmap, 0, 0);
                }
            }

            uint[] linearPixels = new uint[width * height];
            BitmapData bitmapData = paddedBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int[] tempPixels = new int[width * height];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, tempPixels, 0, tempPixels.Length);
            paddedBitmap.UnlockBits(bitmapData);

            for (int i = 0; i < tempPixels.Length; i++)
                linearPixels[i] = (uint)tempPixels[i];

            if (paddedBitmap != bitmap) paddedBitmap.Dispose();

            uint[] swizzledPixels = new uint[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int a = (int)Math.Floor((double)x / BlockWidth) * BlockWidth * BlockHeight + (x % BlockWidth);
                    int b = y * BlockWidth + a;
                    int c = (int)Math.Floor((double)y / BlockHeight) * BlockWidth * BlockHeight * (width / BlockWidth - 1) + b;

                    swizzledPixels[c] = linearPixels[y * width + x];
                }
            }

            return EncodePixels(swizzledPixels, paletteFormat);
        }

        protected virtual byte[] EncodePixels(uint[] pixels, TPL._PaletteHeader.PaletteFormat? paletteFormat = null) { return Array.Empty<byte>(); }
    }

    public enum ImageFormatEnum : uint
    {
        I4 = 0x00,
        I8 = 0x01,
        IA4 = 0x02,
        IA8 = 0x03,
        RGB565 = 0x04,
        RGB5A3 = 0x05,
        RGBA8 = 0x06,
        C4 = 0x08,
        C8 = 0x09,
        C14 = 0x0A,
        CMPR = 0x0E
    }
}
