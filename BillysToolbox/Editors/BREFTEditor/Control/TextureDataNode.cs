using kartlib.Serial;
using System.ComponentModel;
using kartlib.Imaging;

namespace ParticleEditor.Control
{
    public class TextureDataNode : DataNode
    {
        public BREFT._TableItem Item;

        [Category("Info")]
        public string Name
        {
            get { return Item.Name; }
        }

        [Category("Info")]
        public uint DataOffset
        {
            get { return Item.DataOffset; }
        }

        [Category("Info")]
        public uint DataLength
        {
            get { return Item.DataSize; }
        }

        [Category("Texture Properties")]
        public ushort Width
        {
            get { return Item.Texture.Width; }
        }

        [Category("Texture Properties")]
        public ushort Height
        {
            get { return Item.Texture.Height; }
        }

        [Category("Texture Properties")]
        public ImageFormatEnum Format
        {
            get { return Item.Texture.FormatEnum; }
            set
            {
                if (Item.Texture.FormatEnum != value && Item.Texture.Image != null)
                {
                    Item.Texture.ReplaceImage(Item.Texture.Image, value, Item.Texture.PaletteFormat);
                }
            }
        }

        [Category("Texture Properties")]
        public TPL._PaletteHeader.PaletteFormat PaletteFormat
        {
            get { return Item.Texture.PaletteFormat; }
            set
            {
                if (Item.Texture.PaletteFormat != value && Item.Texture.Image != null)
                {
                    Item.Texture.ReplaceImage(Item.Texture.Image, Item.Texture.FormatEnum, value);
                }
            }
        }

        [Category("Advanced Settings")]
        public byte MipmapCount
        {
            get { return Item.Texture.MipmapCount; }
            set { Item.Texture.MipmapCount = value; }
        }

        [Category("Advanced Settings")]
        public byte MinFilter
        {
            get { return Item.Texture.MinFilter; }
            set { Item.Texture.MinFilter = value; }
        }

        [Category("Advanced Settings")]
        public byte MagFilter
        {
            get { return Item.Texture.MagFilter; }
            set { Item.Texture.MagFilter = value; }
        }

        [Category("Advanced Settings")]
        public float LODBias
        {
            get { return Item.Texture.LODBias; }
            set { Item.Texture.LODBias = value; }
        }

        public TextureDataNode(BREFT._TableItem item) : base(item.Name)
        {
            this.Item = item;
            SetImage("decal");
        }
    }
}