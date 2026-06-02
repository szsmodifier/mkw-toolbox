using System;
using System.ComponentModel;
using System.IO;

namespace kartlib.Serial
{
    public enum FogTypeEnum : int
    {
        [Description("No fog")]
        None = 0x0,
        [Description("Perspective projection linear")]
        PerspectiveLinear = 0x1,
        [Description("Perspective projection exponential")]
        PerspectiveExponential = 0x2,
        [Description("Perspective projection exponent squared")]
        PerspectiveExponentSquared = 0x3,
        [Description("Perspective projection inverse exponent")]
        PerspectiveInverseExponent = 0x4,
        [Description("Perspective projection inverse of the square exponent")]
        PerspectiveInverseSquareExponent = 0x5,
        [Description("Orthographic projection linear")]
        OrthographicLinear = 0x6,
        [Description("Orthographic projection exponential")]
        OrthographicExponential = 0x7,
        [Description("Orthographic projection exponent squared")]
        OrthographicExponentSquared = 0x8,
        [Description("Orthographic projection inverse exponent")]
        OrthographicInverseExponent = 0x9,
        [Description("Orthographic projection inverse of the square exponent")]
        OrthographicInverseSquareExponent = 0xA
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FogEntry
    {
        [Category("Fog")] public FogTypeEnum FogType { get; set; }
        [Category("View Space")] public float StartZ { get; set; }
        [Category("View Space")] public float EndZ { get; set; }
        [Category("Appearance")] public byte[] FogColor { get; set; }

        [Category("Settings")]
        [Description("0 = Disabled, 1 = Enabled")]
        public UInt16 RangeCorrection { get; set; }

        [Category("Settings")]
        [Description("Center to which range correction is applied")]
        public UInt16 Center { get; set; }

        [Category("Settings")]
        [Description("Affects the transition speed when swapping fog entries. 1.0 is instant.")]
        public float FadeSpeed { get; set; }

        [Category("Unknown")]
        [Description("0 in most Nintendo tracks, 5544 in N64 DK's Jungle Parkway.")]
        public UInt16 Unknown1 { get; set; }

        [Category("Unknown")] public UInt16 Unknown2 { get; set; }

        public FogEntry()
        {
            FogType = FogTypeEnum.None;
            StartZ = 0;
            EndZ = 0;
            FogColor = new byte[4];
            RangeCorrection = 0;
            Center = 0;
            FadeSpeed = 1.0f;
            Unknown1 = 0;
            Unknown2 = 0;
        }

        public void Read(EndianReader reader)
        {
            FogType = (FogTypeEnum)reader.ReadInt32();
            StartZ = reader.ReadSingle();
            EndZ = reader.ReadSingle();
            FogColor = reader.ReadBytes(4);
            RangeCorrection = reader.ReadUInt16();
            Center = reader.ReadUInt16();
            FadeSpeed = reader.ReadSingle();
            Unknown1 = reader.ReadUInt16();
            Unknown2 = reader.ReadUInt16();
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32((int)FogType);
            writer.WriteSingle(StartZ);
            writer.WriteSingle(EndZ);
            writer.WriteBytes(FogColor);
            writer.WriteUInt16(RangeCorrection);
            writer.WriteUInt16(Center);
            writer.WriteSingle(FadeSpeed);
            writer.WriteUInt16(Unknown1);
            writer.WriteUInt16(Unknown2);
        }
        public override string ToString()
        {
            return $"{FogType} (Z: {StartZ} to {EndZ})";
        }
    }

    public class BFG
    {
        [Browsable(false)]
        public string Filename;

        [Category("Fog Entries")] public FogEntry Entry0 { get; set; }
        [Category("Fog Entries")] public FogEntry Entry1 { get; set; }
        [Category("Fog Entries")] public FogEntry Entry2 { get; set; }
        [Category("Fog Entries")] public FogEntry Entry3 { get; set; }

        public BFG()
        {
            Filename = "Untitled.bfg";
            Entry0 = new FogEntry();
            Entry1 = new FogEntry();
            Entry2 = new FogEntry();
            Entry3 = new FogEntry();
        }

        public BFG(byte[] buffer, string filename)
        {
            Filename = filename;
            Entry0 = new FogEntry();
            Entry1 = new FogEntry();
            Entry2 = new FogEntry();
            Entry3 = new FogEntry();

            EndianReader reader = new EndianReader(buffer, Endianness.BigEndian);
            try
            {
                Entry0.Read(reader);
                Entry1.Read(reader);
                Entry2.Read(reader);
                Entry3.Read(reader);
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
                Entry0.Write(writer);
                Entry1.Write(writer);
                Entry2.Write(writer);
                Entry3.Write(writer);
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