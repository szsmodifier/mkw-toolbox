using kartlib.Imaging;
using System.Drawing;

namespace kartlib.Serial
{
    public class BREFT
    {
        public class _Header
        {
            public uint Magic;
            public ushort ByteOrder;
            public ushort Version;
            public uint FileLength;
            public ushort HeaderLength;
            public ushort BlocksAmount;

            public _Header()
            {
                Magic = 0x52454654;
                ByteOrder = 0xFEFF;
                Version = 9;
                FileLength = 0;
                HeaderLength = 0x10;
                BlocksAmount = 1;
            }

            public _Header(EndianReader reader)
            {
                Magic = reader.ReadUInt32();
                if (Magic != 0x52454654)
                    throw new InvalidDataException("Invalid BREFT magic.");

                ByteOrder = reader.ReadUInt16();
                Version = reader.ReadUInt16();
                FileLength = reader.ReadUInt32();
                HeaderLength = reader.ReadUInt16();
                BlocksAmount = reader.ReadUInt16();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(Magic);
                writer.WriteUInt16(ByteOrder);
                writer.WriteUInt16(Version);
                writer.WriteUInt32(FileLength);
                writer.WriteUInt16(HeaderLength);
                writer.WriteUInt16(BlocksAmount);
            }

            public int SectionSize()
            {
                return 0x10;
            }
        }

        public class _BlockHeader
        {
            public uint Magic;
            public uint SectionLength;

            public _BlockHeader()
            {
                Magic = 0x52454654;
                SectionLength = 0;
            }

            public _BlockHeader(EndianReader reader)
            {
                Magic = reader.ReadUInt32();
                if (Magic != 0x52454654)
                    throw new InvalidDataException("Invalid BREFT block magic.");

                SectionLength = reader.ReadUInt32();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(Magic);
                writer.WriteUInt32(SectionLength);
            }

            public int SectionSize()
            {
                return 0x04 + 0x04;
            }
        }

        public class _ProjectHeader
        {
            public uint Length;
            public uint PreviousProject;
            public uint NextProject;
            public ushort NameLength;
            public ushort Padding;
            public string Name;

            public _ProjectHeader()
            {
                PreviousProject = 0;
                NextProject = 0;
                Padding = 0;
                Name = "Untitled.breft";
                NameLength = (ushort)(Name.Length + 1);
                Length = (uint)SectionSize();
            }

            public _ProjectHeader(EndianReader reader)
            {
                reader.PushPosition();

                Length = reader.ReadUInt32();
                PreviousProject = reader.ReadUInt32();
                NextProject = reader.ReadUInt32();
                NameLength = reader.ReadUInt16();
                Padding = reader.ReadUInt16();
                Name = reader.ReadStringNT();

                reader.Position = reader.PopPosition() + (int)Length;
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(Length);
                writer.WriteUInt32(PreviousProject);
                writer.WriteUInt32(NextProject);
                writer.WriteUInt16(NameLength);
                writer.WriteUInt16(Padding);
                writer.WriteStringNT(Name);
            }

            public int SectionSize()
            {
                return 0x04 + 0x04 + 0x04 + 0x02 + 0x02 + NameLength;
            }
        }

        public class _Table
        {
            public uint Length;
            public ushort EntryAmount;
            public ushort Padding;
            public List<_TableItem> Entries;

            public _Table()
            {
                Length = 0x04 + 0x02 + 0x02;
                EntryAmount = 0;
                Padding = 0;
                Entries = new List<_TableItem>();
            }

            public _Table(EndianReader reader)
            {
                reader.PushPosition();
                Length = reader.ReadUInt32();
                EntryAmount = reader.ReadUInt16();
                Padding = reader.ReadUInt16();
                Entries = new List<_TableItem>();

                for (int i = 0; i < EntryAmount; i++)
                    Entries.Add(new _TableItem(reader));
            }

            public void Write(EndianWriter writer)
            {
                int tableStart = writer.Position;

                writer.WriteUInt32(Length);
                writer.WriteUInt16(EntryAmount);
                writer.WriteUInt16(Padding);
                foreach (_TableItem item in Entries)
                    item.Write(writer, tableStart);
            }

            public int SectionSize()
            {
                int size = 0x04 + 0x02 + 0x02;
                foreach (_TableItem item in this.Entries)
                    size += item.SectionSize();

                return size;
            }
        }

        public class _TableItem
        {
            public ushort NameLength;
            public string Name;
            public uint DataOffset;
            public uint DataSize;
            public Texture Texture;

            public _TableItem(EndianReader reader)
            {
                NameLength = reader.ReadUInt16();
                Name = reader.ReadStringNT();
                DataOffset = reader.ReadUInt32();
                DataSize = reader.ReadUInt32();

                int tableStartPos = reader.PeekPosition();
                reader.PushPosition();
                reader.Position = tableStartPos + (int)DataOffset;

                Texture = new Texture(reader);

                reader.Position = reader.PopPosition();
            }

            public void Write(EndianWriter writer, int tableStart)
            {
                writer.WriteUInt16(NameLength);
                writer.WriteStringNT(Name);
                writer.WriteUInt32(DataOffset);
                writer.WriteUInt32(DataSize);

                writer.PushPosition();
                writer.Position = (int)DataOffset + tableStart;

                Texture.Write(writer);

                writer.Position = writer.PopPosition();
            }

            public int SectionSize()
            {
                return 0x02 + NameLength + 0x04 + 0x04;
            }
        }

        public class Texture
        {
            public uint Unknown0;
            public ushort Width;
            public ushort Height;
            public uint ImageDataSize;
            public ImageFormatEnum FormatEnum;
            public TPL._PaletteHeader.PaletteFormat PaletteFormat;
            public ushort PaletteEntryCount;
            public uint PaletteDataSize;
            public byte MipmapCount;
            public byte MinFilter;
            public byte MagFilter;
            public byte Unknown1;
            public float LODBias;
            public uint Unknown2;

            public ImageFormat Format;
            public byte[] ImageData;
            public byte[]? PaletteData;
            public Bitmap? Image;

            public Texture(EndianReader reader)
            {
                Unknown0 = reader.ReadUInt32();
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                ImageDataSize = reader.ReadUInt32();
                FormatEnum = (ImageFormatEnum)reader.ReadByte();
                PaletteFormat = (TPL._PaletteHeader.PaletteFormat)reader.ReadByte();
                PaletteEntryCount = reader.ReadUInt16();
                PaletteDataSize = reader.ReadUInt32();
                MipmapCount = reader.ReadByte();
                MinFilter = reader.ReadByte();
                MagFilter = reader.ReadByte();
                Unknown1 = reader.ReadByte();
                LODBias = reader.ReadSingle();
                Unknown2 = reader.ReadUInt32();

                ImageData = reader.ReadBytes((int)ImageDataSize);

                if (PaletteDataSize > 0)
                {
                    PaletteData = reader.ReadBytes((int)PaletteDataSize);
                }

                ImageFormat? format = ImageFactory.GetFormat(FormatEnum);
                if (format != null)
                {
                    Format = format;
                    TPL._PaletteHeader.PaletteFormat? palFmt = PaletteDataSize > 0 ? PaletteFormat : null;
                    Image = Format.ToBitmap(ImageData, Width, Height, PaletteData, palFmt);
                }
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(Unknown0);
                writer.WriteUInt16(Width);
                writer.WriteUInt16(Height);
                writer.WriteUInt32(ImageDataSize);
                writer.WriteByte((byte)FormatEnum);
                writer.WriteByte((byte)PaletteFormat);
                writer.WriteUInt16(PaletteEntryCount);
                writer.WriteUInt32(PaletteDataSize);
                writer.WriteByte(MipmapCount);
                writer.WriteByte(MinFilter);
                writer.WriteByte(MagFilter);
                writer.WriteByte(Unknown1);
                writer.WriteSingle(LODBias);
                writer.WriteUInt32(Unknown2);

                writer.WriteBytes(ImageData);

                if (PaletteDataSize > 0 && PaletteData != null)
                {
                    writer.WriteBytes(PaletteData);
                }
            }

            public int SectionSize()
            {
                return 32 + (int)ImageDataSize + (int)PaletteDataSize;
            }

            public void ReplaceImage(Bitmap newImage, ImageFormatEnum targetFormat, TPL._PaletteHeader.PaletteFormat? targetPaletteFormat = null)
            {
                ImageFormat format = ImageFactory.GetFormat(targetFormat);
                if (format == null)
                    throw new NotSupportedException($"ImageFormat {targetFormat} is not supported.");

                TPL._PaletteHeader.PaletteFormat palFormatToUse = targetPaletteFormat ?? this.PaletteFormat;

                byte[] newImageData = format.FromBitmap(newImage, palFormatToUse);
                byte[] newPaletteData = format.LastGeneratedPalette;

                this.Width = (ushort)newImage.Width;
                this.Height = (ushort)newImage.Height;
                this.FormatEnum = targetFormat;
                this.Format = format;
                this.Image = newImage;
                this.ImageData = newImageData;
                this.ImageDataSize = (uint)newImageData.Length;

                if (newPaletteData != null && newPaletteData.Length > 0)
                {
                    this.PaletteData = newPaletteData;
                    this.PaletteDataSize = (uint)newPaletteData.Length;
                    this.PaletteEntryCount = (ushort)(newPaletteData.Length / 2);
                    this.PaletteFormat = palFormatToUse;
                }
                else
                {
                    this.PaletteData = null;
                    this.PaletteDataSize = 0;
                    this.PaletteEntryCount = 0;
                }
            }
        }

        public _Header Header { get; set; }
        public _BlockHeader BlockHeader { get; set; }
        public _ProjectHeader ProjectHeader { get; set; }
        public _Table Table { get; set; }
        public string Filename { get; set; }

        public BREFT()
        {
            Filename = "Untitled.breft";
            Header = new _Header();
            BlockHeader = new _BlockHeader();
            ProjectHeader = new _ProjectHeader();
            Table = new _Table();
        }

        public BREFT(byte[] buffer, string filename)
        {
            Filename = filename;
            EndianReader reader = new EndianReader(buffer, Endianness.BigEndian);
            try
            {
                Header = new _Header(reader);
                BlockHeader = new _BlockHeader(reader);
                ProjectHeader = new _ProjectHeader(reader);
                Table = new _Table(reader);
            }
            finally
            {
                reader.Close();
            }
        }

        private void CalculateVariables()
        {
            int size = Header.SectionSize() + BlockHeader.SectionSize() + ProjectHeader.SectionSize() + Table.SectionSize();
            foreach (_TableItem item in Table.Entries)
                size += (int)item.DataSize;

            Header.FileLength = (uint)size;

            size = ProjectHeader.SectionSize() + Table.SectionSize();
            foreach (_TableItem item in Table.Entries)
                size += (int)item.DataSize;

            BlockHeader.SectionLength = (uint)size;
            ProjectHeader.Length = (uint)ProjectHeader.SectionSize();

            Table.Length = (uint)Table.SectionSize();
            Table.EntryAmount = (ushort)Table.Entries.Count;

            size = Table.SectionSize();
            foreach (_TableItem item in Table.Entries)
            {
                item.DataSize = (uint)item.Texture.SectionSize();
                item.DataOffset = (uint)size;
                size += (int)item.DataSize;
            }
        }

        public byte[] Write()
        {
            MemoryStream stream = new MemoryStream();
            EndianWriter writer = new EndianWriter(stream, Endianness.BigEndian);
            try
            {
                CalculateVariables();
                Header.Write(writer);
                BlockHeader.Write(writer);
                ProjectHeader.Write(writer);
                Table.Write(writer);
            }
            finally
            {
                stream.Close();
                writer.Close();
            }

            return stream.ToArray();
        }
    }
}