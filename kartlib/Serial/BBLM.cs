using System;
using System.ComponentModel;
using System.IO;

namespace kartlib.Serial
{
    public class BBLM
    {
        public enum BlendModeEnum : byte
        {
            [Description("No modifications")]
            None = 0,
            [Description("Multiply Complement")]
            MultiplyComplement = 1,
            [Description("Tint Alpha (Source)")]
            TintAlphaSource = 2,
            [Description("Tint Alpha (Weighted)")]
            TintAlphaWeighted = 3,
            [Description("Multiply Self")]
            MultiplySelf = 4
        }

        public string Filename;
        [Browsable(false)] public UInt32 Magic;
        [Browsable(false)] public UInt32 Size;

        [Category("Header")] public Byte Version { get; set; }
        [Browsable(false)] public Byte[] Reserved0;
        [Category("Header")] public UInt32 Unknown0 { get; set; }

        [Category("Threshold")] public float ThresholdIntensity { get; set; }
        [Category("Threshold")] public byte[] ThresholdColor { get; set; }

        [Category("Tint")] public byte[] TintColor { get; set; }

        [Category("Settings")] public UInt16 Bitfield { get; set; }
        [Browsable(false)] public Byte[] Reserved1;

        [Category("First Blur")] public float FirstBlurStrength { get; set; }
        [Category("First Blur")] public float FirstBlurColorIntensity { get; set; }
        [Category("First Blur")] public UInt32[] FirstBlurUnknown { get; set; }

        [Category("Second Blur (Stage 1)")] public float SecondBlurStage1Strength { get; set; }
        [Category("Second Blur (Stage 1)")] public float SecondBlurStage1ColorIntensity { get; set; }
        [Category("Second Blur (Stage 1)")] public UInt32[] SecondBlurStage1Unknown { get; set; }

        [Category("Second Blur (Stage 2)")]
        [Description("Unused in-game; value from stage 1 is used instead.")]
        public float SecondBlurStage2Strength { get; set; }
        [Category("Second Blur (Stage 2)")] public float SecondBlurStage2ColorIntensity { get; set; }
        [Category("Second Blur (Stage 2)")] public UInt32[] SecondBlurStage2Unknown { get; set; }

        [Category("Bloom")] public BlendModeEnum BlendMode { get; set; }
        [Category("Bloom")] public Byte BlurStages { get; set; }
        [Browsable(false)] public Byte[] Reserved2;

        [Category("Bloom")] public float[] BloomColorIntensity { get; set; }

        public BBLM()
        {
            Filename = "Untitled.bblm";
            Magic = 0x50424C4D;
            Size = 0xA4;
            Version = 0x01;
            Reserved0 = new byte[3];
            Unknown0 = 0;
            ThresholdIntensity = 0;
            ThresholdColor = new byte[4];
            TintColor = new byte[4];
            Bitfield = 0;
            Reserved1 = new byte[2];
            FirstBlurStrength = 0;
            FirstBlurColorIntensity = 0;
            FirstBlurUnknown = new uint[6];
            SecondBlurStage1Strength = 0;
            SecondBlurStage1ColorIntensity = 0;
            SecondBlurStage1Unknown = new uint[6];
            SecondBlurStage2Strength = 0;
            SecondBlurStage2ColorIntensity = 0;
            SecondBlurStage2Unknown = new uint[6];
            BlendMode = BlendModeEnum.None;
            BlurStages = 0;
            Reserved2 = new byte[26];
            BloomColorIntensity = new float[2];
        }

        public BBLM(byte[] buffer, string filename)
        {
            Filename = filename;
            EndianReader reader = new EndianReader(buffer, Endianness.BigEndian);
            try
            {
                Magic = reader.ReadUInt32();
                Size = reader.ReadUInt32();
                Version = reader.ReadByte();
                Reserved0 = reader.ReadBytes(3);
                Unknown0 = reader.ReadUInt32();

                ThresholdIntensity = reader.ReadSingle();
                ThresholdColor = reader.ReadBytes(4);
                TintColor = reader.ReadBytes(4);

                Bitfield = reader.ReadUInt16();
                Reserved1 = reader.ReadBytes(2);

                FirstBlurStrength = reader.ReadSingle();
                FirstBlurColorIntensity = reader.ReadSingle();

                FirstBlurUnknown = new uint[6];
                for (int i = 0; i < 6; i++) FirstBlurUnknown[i] = reader.ReadUInt32();

                SecondBlurStage1Strength = reader.ReadSingle();
                SecondBlurStage1ColorIntensity = reader.ReadSingle();
                SecondBlurStage1Unknown = new uint[6];
                for (int i = 0; i < 6; i++) SecondBlurStage1Unknown[i] = reader.ReadUInt32();

                SecondBlurStage2Strength = reader.ReadSingle();
                SecondBlurStage2ColorIntensity = reader.ReadSingle();
                SecondBlurStage2Unknown = new uint[6];
                for (int i = 0; i < 6; i++) SecondBlurStage2Unknown[i] = reader.ReadUInt32();

                BlendMode = (BlendModeEnum)reader.ReadByte();
                BlurStages = reader.ReadByte();
                Reserved2 = reader.ReadBytes(26);

                BloomColorIntensity = reader.ReadSingles(2);
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
                writer.WriteUInt32(Magic);
                writer.WriteUInt32(Size);
                writer.WriteByte(Version);
                writer.WriteBytes(Reserved0);
                writer.WriteUInt32(Unknown0);

                writer.WriteSingle(ThresholdIntensity);
                writer.WriteBytes(ThresholdColor);
                writer.WriteBytes(TintColor);

                writer.WriteUInt16(Bitfield);
                writer.WriteBytes(Reserved1);

                writer.WriteSingle(FirstBlurStrength);
                writer.WriteSingle(FirstBlurColorIntensity);
                for (int i = 0; i < 6; i++) writer.WriteUInt32(FirstBlurUnknown[i]);

                writer.WriteSingle(SecondBlurStage1Strength);
                writer.WriteSingle(SecondBlurStage1ColorIntensity);
                for (int i = 0; i < 6; i++) writer.WriteUInt32(SecondBlurStage1Unknown[i]);

                writer.WriteSingle(SecondBlurStage2Strength);
                writer.WriteSingle(SecondBlurStage2ColorIntensity);
                for (int i = 0; i < 6; i++) writer.WriteUInt32(SecondBlurStage2Unknown[i]);

                writer.WriteByte((byte)BlendMode);
                writer.WriteByte(BlurStages);
                writer.WriteBytes(Reserved2);

                writer.WriteSingles(BloomColorIntensity);
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