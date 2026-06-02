using kartlib.Imaging;
using System.ComponentModel;
using System.Drawing;
using static kartlib.Serial.TPL._PaletteHeader;

namespace kartlib.Serial
{
    public class BTI
    {
        [Browsable(false)]
        public string Filename { get; set; }

        [Browsable(false)]
        private ImageFormatEnum _formatEnum;

        [Category("Image Format")]
        public ImageFormatEnum FormatEnum
        {
            get => _formatEnum;
            set
            {
                if (_formatEnum != value)
                {
                    if (_image != null)
                    {
                        ReplaceImage(_image, value, _paletteFormat);
                    }
                    else
                    {
                        _formatEnum = value;
                    }
                }
            }
        }

        [Category("Image Format")]
        [Description("0x00 means alpha is disabled, anything higher means alpha is enabled")]
        public byte EnableAlpha { get; set; }

        [Category("Dimensions")] public ushort Width { get; set; }
        [Category("Dimensions")] public ushort Height { get; set; }

        [Category("Wrap")]
        [Description("0x01 for posteffect.bti, 0x00 for others")]
        public byte WrapS { get; set; }

        [Category("Wrap")]
        [Description("0x01 for posteffect.bti, 0x00 for others")]
        public byte WrapT { get; set; }

        [Browsable(false)]
        private TPL._PaletteHeader.PaletteFormat _paletteFormat;

        [Category("Palette Format")]
        public TPL._PaletteHeader.PaletteFormat PaletteFormat
        {
            get => _paletteFormat;
            set
            {
                if (_paletteFormat != value)
                {
                    if (_image != null)
                    {
                        ReplaceImage(_image, _formatEnum, value);
                    }
                    else
                    {
                        _paletteFormat = value;
                    }
                }
            }
        }
        [Category("Palette Format")] public ushort PaletteEntryCount { get; set; }
        [Browsable(false)] public uint PaletteOffset { get; set; }

        [Category("Unknown")] public uint Unknown0 { get; set; }

        [Category("Filters")]
        [Description("0x00 means nearest, 0x01 means linear")]
        public byte MagFilter { get; set; }

        [Category("Filters")] public byte MinFilter { get; set; }

        [Category("Unknown")] public ushort Unknown1 { get; set; }

        [Category("Image Format")]
        [Description("Total number of images (mipmaps + 1)")]
        public byte TotalImages { get; set; }

        [Category("Unknown")] public byte Unknown2 { get; set; }
        [Category("Unknown")] public ushort Unknown3 { get; set; }

        [Browsable(false)] public uint ImageOffset { get; set; }

        [Browsable(false)] public ImageFormat Format { get; set; }
        [Browsable(false)] public byte[] ImageData { get; set; }
        [Browsable(false)] public byte[]? PaletteData { get; set; }
        [Browsable(false)]
        private Bitmap? _image;

        [Category("Preview")]
        public Bitmap? Image
        {
            get => _image;
            set
            {
                // Only trigger the replacement if a new, valid image is provided
                if (value != null && value != _image)
                {
                    ReplaceImage(value, this.FormatEnum, this.PaletteFormat);
                }
                else if (value == null)
                {
                    _image = null;
                    ImageData = new byte[0];
                    PaletteData = null;
                    PaletteEntryCount = 0;
                    PaletteOffset = 0;
                }
            }
        }

        public BTI()
        {
            Filename = "Untitled.bti";
            FormatEnum = ImageFormatEnum.CMPR;
            EnableAlpha = 0;
            Width = 0;
            Height = 0;
            WrapS = 0;
            WrapT = 0;
            PaletteFormat = 0;
            PaletteEntryCount = 0;
            PaletteOffset = 0;
            Unknown0 = 0;
            MagFilter = 0;
            MinFilter = 0;
            Unknown1 = 0;
            TotalImages = 1;
            Unknown2 = 0;
            Unknown3 = 0;
            ImageOffset = 0x20;
            ImageData = new byte[0];
            PaletteData = null;
        }

        public BTI(byte[] buffer, string filename)
        {
            Filename = filename;
            EndianReader reader = new EndianReader(buffer, Endianness.BigEndian);
            try
            {
                FormatEnum = (ImageFormatEnum)reader.ReadByte();
                EnableAlpha = reader.ReadByte();
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                WrapS = reader.ReadByte();
                WrapT = reader.ReadByte();
                PaletteFormat = (TPL._PaletteHeader.PaletteFormat)reader.ReadUInt16();
                PaletteEntryCount = reader.ReadUInt16();
                PaletteOffset = reader.ReadUInt32();
                Unknown0 = reader.ReadUInt32();
                MagFilter = reader.ReadByte();
                MinFilter = reader.ReadByte();
                Unknown1 = reader.ReadUInt16();
                TotalImages = reader.ReadByte();
                Unknown2 = reader.ReadByte();
                Unknown3 = reader.ReadUInt16();
                ImageOffset = reader.ReadUInt32();

                if (PaletteOffset > 0 && PaletteEntryCount > 0)
                {
                    reader.Position = (int)PaletteOffset;
                    PaletteData = reader.ReadBytes(PaletteEntryCount * 2);
                }

                if (ImageOffset > 0)
                {
                    reader.Position = (int)ImageOffset;
                    int endOfImage = PaletteOffset > ImageOffset ? (int)PaletteOffset : buffer.Length;
                    ImageData = reader.ReadBytes(endOfImage - (int)ImageOffset);
                }

                ImageFormat? format = ImageFactory.GetFormat(FormatEnum);
                if (format != null && ImageData != null && ImageData.Length > 0)
                {
                    Format = format;
                    TPL._PaletteHeader.PaletteFormat? palFmt = PaletteData != null ? PaletteFormat : null;
                    Image = Format.ToBitmap(ImageData, Width, Height, PaletteData, palFmt);
                }
            }
            finally
            {
                reader.Close();
            }
        }

        public byte[] Write()
        {
            MemoryStream stream = new MemoryStream();
            EndianWriter writer = new EndianWriter(stream, Endianness.BigEndian);
            try
            {
                ImageOffset = 0x20;
                PaletteOffset = (PaletteData != null && PaletteData.Length > 0) ? (uint)(0x20 + ImageData.Length) : 0;

                writer.WriteByte((byte)FormatEnum);
                writer.WriteByte(EnableAlpha);
                writer.WriteUInt16(Width);
                writer.WriteUInt16(Height);
                writer.WriteByte(WrapS);
                writer.WriteByte(WrapT);
                writer.WriteUInt16((ushort)PaletteFormat);
                writer.WriteUInt16(PaletteEntryCount);
                writer.WriteUInt32(PaletteOffset);
                writer.WriteUInt32(Unknown0);
                writer.WriteByte(MagFilter);
                writer.WriteByte(MinFilter);
                writer.WriteUInt16(Unknown1);
                writer.WriteByte(TotalImages);
                writer.WriteByte(Unknown2);
                writer.WriteUInt16(Unknown3);
                writer.WriteUInt32(ImageOffset);

                writer.Position = (int)ImageOffset;
                if (ImageData != null)
                {
                    writer.WriteBytes(ImageData);
                }

                if (PaletteData != null && PaletteOffset > 0)
                {
                    writer.Position = (int)PaletteOffset;
                    writer.WriteBytes(PaletteData);
                }
            }
            finally
            {
                stream.Close();
                writer.Close();
            }

            return stream.ToArray();
        }

        public void ReplaceImage(Bitmap newImage, ImageFormatEnum targetFormat, TPL._PaletteHeader.PaletteFormat? targetPaletteFormat = null)
        {
            ImageFormat format = ImageFactory.GetFormat(targetFormat);
            if (format == null)
                throw new NotSupportedException($"ImageFormat {targetFormat} is not supported.");

            TPL._PaletteHeader.PaletteFormat palFormatToUse = targetPaletteFormat ?? this._paletteFormat;

            byte[] newImageData = format.FromBitmap(newImage, palFormatToUse);
            byte[] newPaletteData = format.LastGeneratedPalette;

            this.Width = (ushort)newImage.Width;
            this.Height = (ushort)newImage.Height;
            this._formatEnum = targetFormat;
            this.Format = format;
            this._image = newImage;

            this.ImageData = newImageData;

            if (newPaletteData != null && newPaletteData.Length > 0)
            {
                this.PaletteData = newPaletteData;
                this.PaletteEntryCount = (ushort)(newPaletteData.Length / 2);
                this._paletteFormat = palFormatToUse;
            }
            else
            {
                this.PaletteData = null;
                this.PaletteEntryCount = 0;
                this.PaletteOffset = 0;
            }
        }
    }
}