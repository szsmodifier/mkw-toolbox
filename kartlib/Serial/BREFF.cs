namespace kartlib.Serial
{
    public class BREFF
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
                Magic = 0x52454646;
                ByteOrder = 0xFEFF;
                Version = 9;
                FileLength = 0;
                HeaderLength = 0x10;
                BlocksAmount = 1;
            }

            public _Header(EndianReader reader)
            {
                Magic = reader.ReadUInt32();
                if (Magic != 0x52454646)
                    throw new InvalidDataException();

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
                Magic = 0x52454646;
                SectionLength = 0;
            }

            public _BlockHeader(EndianReader reader)
            {
                Magic = reader.ReadUInt32();
                if (Magic != 0x52454646)
                    throw new InvalidDataException();

                SectionLength = reader.ReadUInt32();
            }

            //////////////////////////////////////
            //////////////////////////////////////

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
                Name = "Untitled.breff";
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

            /////////////////////////////
            /////////////////////////////

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

            //////////////////////////////////////
            //////////////////////////////////////

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
            public Emitter Emitter;
            public Particle Particle;

            public _TableItem(EndianReader reader)
            {
                NameLength = reader.ReadUInt16();
                Name = reader.ReadStringNT();
                DataOffset = reader.ReadUInt32();
                DataSize = reader.ReadUInt32();

                int tableStartPos = reader.PeekPosition();
                reader.PushPosition();
                reader.Position = tableStartPos + (int)DataOffset;

                Emitter = new Emitter(reader);
                Particle = new Particle(reader);

                reader.Position = reader.PopPosition();
            }

            //////////////////////////////////////
            //////////////////////////////////////

            public void Write(EndianWriter writer, int tableStart)
            {
                writer.WriteUInt16(NameLength);
                writer.WriteStringNT(Name);
                writer.WriteUInt32(DataOffset);
                writer.WriteUInt32(DataSize);

                // Jump to DataOffset, write subfile bytes, jump back to previous position
                writer.PushPosition();
                writer.Position = (int)DataOffset + tableStart;

                Emitter.Write(writer);
                Particle.Write(writer);

                writer.Position = writer.PopPosition();
            }

            public int SectionSize()
            {
                return 0x02 + NameLength + 0x04 + 0x04;
            }
        }

        public class _AnimationTable
        {
            public ushort ParticleAnimationCount;
            public ushort ParticleInitTrackCount;
            public uint[] ParticleAnimationPointers;
            public uint[] ParticleAnimationSizes;

            public ushort EmitterAnimationCount;
            public ushort EmitterInitTrackCount;
            public uint[] EmitterAnimationPointers;
            public uint[] EmitterAnimationSizes;

            public List<_Animation> ParticleAnimations;
            public List<_Animation> EmitterAnimations;

            public _AnimationTable()
            {
                ParticleAnimations = new List<_Animation>();
                EmitterAnimations = new List<_Animation>();
            }

            public _AnimationTable(EndianReader reader)
            {
                long tableStart = reader.Position;

                ParticleAnimationCount = reader.ReadUInt16();
                ParticleInitTrackCount = reader.ReadUInt16();

                ParticleAnimationPointers = new uint[ParticleAnimationCount];
                for (int i = 0; i < ParticleAnimationCount; i++)
                    ParticleAnimationPointers[i] = reader.ReadUInt32();

                ParticleAnimationSizes = new uint[ParticleAnimationCount];
                for (int i = 0; i < ParticleAnimationCount; i++)
                    ParticleAnimationSizes[i] = reader.ReadUInt32();

                EmitterAnimationCount = reader.ReadUInt16();
                EmitterInitTrackCount = reader.ReadUInt16();

                EmitterAnimationPointers = new uint[EmitterAnimationCount];
                for (int i = 0; i < EmitterAnimationCount; i++)
                    EmitterAnimationPointers[i] = reader.ReadUInt32();

                EmitterAnimationSizes = new uint[EmitterAnimationCount];
                for (int i = 0; i < EmitterAnimationCount; i++)
                    EmitterAnimationSizes[i] = reader.ReadUInt32();

                ParticleAnimations = new List<_Animation>();
                for (int i = 0; i < ParticleAnimationCount; i++)
                {
                    reader.PushPosition();
                    reader.Position = (int)(tableStart + ParticleAnimationPointers[i]);
                    ParticleAnimations.Add(new _Animation(reader));
                    reader.Position = reader.PopPosition();
                }

                EmitterAnimations = new List<_Animation>();
                for (int i = 0; i < EmitterAnimationCount; i++)
                {
                    reader.PushPosition();
                    reader.Position = (int)(tableStart + EmitterAnimationPointers[i]);
                    EmitterAnimations.Add(new _Animation(reader));
                    reader.Position = reader.PopPosition();
                }
            }

            public void Write(EndianWriter writer)
            {
                ParticleAnimationCount = (ushort)ParticleAnimations.Count;
                EmitterAnimationCount = (ushort)EmitterAnimations.Count;

                int headerBlockSize = 2 + 2 + (ParticleAnimationCount * 4) + (ParticleAnimationCount * 4) +
                                      2 + 2 + (EmitterAnimationCount * 4) + (EmitterAnimationCount * 4);

                ParticleAnimationPointers = new uint[ParticleAnimationCount];
                ParticleAnimationSizes = new uint[ParticleAnimationCount];
                EmitterAnimationPointers = new uint[EmitterAnimationCount];
                EmitterAnimationSizes = new uint[EmitterAnimationCount];

                int currentDataOffset = headerBlockSize;

                for (int i = 0; i < ParticleAnimationCount; i++)
                {
                    ParticleAnimationPointers[i] = (uint)currentDataOffset;
                    uint animSize = (uint)ParticleAnimations[i].SectionSize();
                    ParticleAnimationSizes[i] = animSize;
                    currentDataOffset += (int)animSize;
                }

                for (int i = 0; i < EmitterAnimationCount; i++)
                {
                    EmitterAnimationPointers[i] = (uint)currentDataOffset;
                    uint animSize = (uint)EmitterAnimations[i].SectionSize();
                    EmitterAnimationSizes[i] = animSize;
                    currentDataOffset += (int)animSize;
                }

                writer.WriteUInt16(ParticleAnimationCount);
                writer.WriteUInt16(ParticleInitTrackCount);

                foreach (uint ptr in ParticleAnimationPointers)
                    writer.WriteUInt32(ptr);
                foreach (uint size in ParticleAnimationSizes)
                    writer.WriteUInt32(size);

                writer.WriteUInt16(EmitterAnimationCount);
                writer.WriteUInt16(EmitterInitTrackCount);

                foreach (uint ptr in EmitterAnimationPointers)
                    writer.WriteUInt32(ptr);
                foreach (uint size in EmitterAnimationSizes)
                    writer.WriteUInt32(size);

                foreach (_Animation anim in ParticleAnimations)
                    anim.Write(writer);

                foreach (_Animation anim in EmitterAnimations)
                    anim.Write(writer);
            }
        }

        public class _Animation
        {
            public byte Identifier;
            public byte KindType;
            public byte CurveFlag;
            public byte KindEnable;
            public byte ProcessFlag;
            public byte LoopCount;
            public ushort RandomSeed;
            public ushort FrameCount;
            public ushort Padding;

            public uint KeyTableSize;
            public uint RangeTableSize;
            public uint RandomTableSize;
            public uint NameTableSize;
            public uint InfoTableSize;

            public byte[] KeyTableData;
            public byte[] RangeTableData;
            public byte[] RandomTableData;
            public byte[] NameTableData;
            public byte[] InfoTableData;

            public _Animation(EndianReader reader)
            {
                Identifier = reader.ReadByte();
                KindType = reader.ReadByte();
                CurveFlag = reader.ReadByte();
                KindEnable = reader.ReadByte();
                ProcessFlag = reader.ReadByte();
                LoopCount = reader.ReadByte();
                RandomSeed = reader.ReadUInt16();
                FrameCount = reader.ReadUInt16();
                Padding = reader.ReadUInt16();

                KeyTableSize = reader.ReadUInt32();
                RangeTableSize = reader.ReadUInt32();
                RandomTableSize = reader.ReadUInt32();
                NameTableSize = reader.ReadUInt32();
                InfoTableSize = reader.ReadUInt32();

                KeyTableData = reader.ReadBytes((int)KeyTableSize);
                RangeTableData = reader.ReadBytes((int)RangeTableSize);
                RandomTableData = reader.ReadBytes((int)RandomTableSize);
                NameTableData = reader.ReadBytes((int)NameTableSize);
                InfoTableData = reader.ReadBytes((int)InfoTableSize);
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteByte(Identifier);
                writer.WriteByte(KindType);
                writer.WriteByte(CurveFlag);
                writer.WriteByte(KindEnable);
                writer.WriteByte(ProcessFlag);
                writer.WriteByte(LoopCount);
                writer.WriteUInt16(RandomSeed);
                writer.WriteUInt16(FrameCount);
                writer.WriteUInt16(Padding);

                KeyTableSize = (uint)(KeyTableData?.Length ?? 0);
                RangeTableSize = (uint)(RangeTableData?.Length ?? 0);
                RandomTableSize = (uint)(RandomTableData?.Length ?? 0);
                NameTableSize = (uint)(NameTableData?.Length ?? 0);
                InfoTableSize = (uint)(InfoTableData?.Length ?? 0);

                writer.WriteUInt32(KeyTableSize);
                writer.WriteUInt32(RangeTableSize);
                writer.WriteUInt32(RandomTableSize);
                writer.WriteUInt32(NameTableSize);
                writer.WriteUInt32(InfoTableSize);

                if (KeyTableSize > 0) writer.WriteBytes(KeyTableData);
                if (RangeTableSize > 0) writer.WriteBytes(RangeTableData);
                if (RandomTableSize > 0) writer.WriteBytes(RandomTableData);
                if (NameTableSize > 0) writer.WriteBytes(NameTableData);
                if (InfoTableSize > 0) writer.WriteBytes(InfoTableData);
            }

            public int SectionSize()
            {
                int payloadSize = (int)(KeyTableSize + RangeTableSize + RandomTableSize + NameTableSize + InfoTableSize);
                return 28 + payloadSize;
            }
        }

        /////////////////////////////
        /////////////////////////////

        public _Header Header { get; set; }
        public _BlockHeader BlockHeader { get; set; }
        public _ProjectHeader ProjectHeader { get; set; }
        public _Table Table { get; set; }
        public _AnimationTable AnimationTable { get; set; }
        public string Filename { get; set; }

        /// <summary>
        /// Create new empty BREFF
        /// </summary>
        public BREFF()
        {
            Filename = "Untitled.breff";
            Header = new _Header();
            BlockHeader = new _BlockHeader();
            ProjectHeader = new _ProjectHeader();
            Table = new _Table();
            AnimationTable = new _AnimationTable();
        }

        /// <summary>
        /// Create new BREFF from byte array and filename
        /// </summary>
        /// <param name="buffer">File bytes</param>
        /// <param name="filename">File path or name</param>
        public BREFF(byte[] buffer, string filename)
        {
            Filename = filename;
            EndianReader reader = new EndianReader(buffer, Endianness.BigEndian);
            try
            {
                Header = new _Header(reader);
                BlockHeader = new _BlockHeader(reader);
                ProjectHeader = new _ProjectHeader(reader);
                Table = new _Table(reader);
                AnimationTable = new _AnimationTable(reader);
            }
            finally
            {
                reader.Close();
            }
        }

        /////////////////////////////
        /////////////////////////////

        /// <summary>
        /// Calculates the internal file data (e.g. subfile offsets)
        /// </summary>
        private void CalculateVariables()
        {
            // Header Vars
            int size = Header.SectionSize() + BlockHeader.SectionSize() + ProjectHeader.SectionSize() + Table.SectionSize();
            foreach (_TableItem item in Table.Entries)
                size += (int)item.DataSize;

            Header.FileLength = (uint)size;

            // BlockHeader Vars
            size = ProjectHeader.SectionSize() + Table.SectionSize();
            foreach (_TableItem item in Table.Entries)
                size += (int)item.DataSize;

            BlockHeader.SectionLength = (uint)size;

            // ProjectHeader Vars
            ProjectHeader.Length = (uint)ProjectHeader.SectionSize();

            // Table Vars
            Table.Length = (uint)Table.SectionSize();
            Table.EntryAmount = (ushort)Table.Entries.Count;

            // Table Items
            size = Table.SectionSize();
            foreach (_TableItem item in Table.Entries)
            {
                item.DataOffset = (uint)size;
                size += (int)item.DataSize;
            }
        }

        /////////////////////////////
        /////////////////////////////

        /// <summary>
        /// Serializes BREFF instance to a new byte array
        /// </summary>
        /// <returns>BREFF file bytes</returns>
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
                AnimationTable.Write(writer);
            }
            finally
            {
                stream.Close();
                writer.Close();
            }

            return stream.ToArray();
        }
    }

    public class Emitter
    {
        public class _Header
        {
            public uint EffectNamePointer;
            public uint Size;

            public _Header()
            {
                EffectNamePointer = 0;
                Size = 0;
            }

            public _Header(EndianReader reader)
            {
                EffectNamePointer = reader.ReadUInt32();
                Size = reader.ReadUInt32();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(EffectNamePointer);
                writer.WriteUInt32(Size);
            }
        }

        public class _EmitData
        {
            public uint Unknown0;
            public uint EmitFlags;
            public byte EmitShape;
            public ushort EmitterLife;
            public ushort ParticleLife;
            public byte ParticleLifeRandom;
            public bool InheritChildParticleTranslation;
            public byte EmitIntervalRandom;
            public byte EmitRandom;
            public float EmissionRate;
            public ushort EmitStart;
            public ushort EmitEnd;
            public ushort EmitInterval;
            public bool InheritParticleTranslation;
            public bool InheritChildEmitterTranslation;
            public float[] EmitterDimensions;
            public ushort EmitDiversion;
            public byte VelocityRandom;
            public byte MomentumRandom;
            public float PowerRadiation;
            public float PowerYAxisValue;
            public float PowerRandom;
            public float PowerNormal;
            public float DiffusionEmitterNormal;
            public float PowerSpec;
            public float DiffusionSpec;
            public Vector3f EmissionAngle;
            public Vector3f Scale;
            public Vector3f Rotation;
            public Vector3f Translation;
            public byte LODNearestDistance;
            public byte LODFarthestDistance;
            public byte LODMinimalEmission;
            public byte LODAlpha;
            public uint RandomSeed;
            public ulong Unknown1;
            public ushort DrawFlagsBitfield;
            public byte AlphaComparison0;
            public byte AlphaComparison1;
            public byte AlphaCompareOperation;
            public byte TEVStageAmount;
            public byte Unknown2;
            public byte EnableIndirectTEV;

            public _EmitData(EndianReader reader)
            {
                Unknown0 = reader.ReadUInt32();
                EmitFlags = reader.ReadUInt24();
                EmitShape = reader.ReadByte();
                EmitterLife = reader.ReadUInt16();
                ParticleLife = reader.ReadUInt16();
                ParticleLifeRandom = reader.ReadByte();
                InheritChildParticleTranslation = reader.ReadByte() == 0;
                EmitIntervalRandom = reader.ReadByte();
                EmitRandom = reader.ReadByte();
                EmissionRate = reader.ReadSingle();
                EmitStart = reader.ReadUInt16();
                EmitEnd = reader.ReadUInt16();
                EmitInterval = reader.ReadUInt16();
                InheritParticleTranslation = reader.ReadByte() == 0;
                InheritChildEmitterTranslation = reader.ReadByte() == 0;
                EmitterDimensions = new float[6]
                {
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                };
                EmitDiversion = reader.ReadUInt16();
                VelocityRandom = reader.ReadByte();
                MomentumRandom = reader.ReadByte();
                PowerRadiation = reader.ReadSingle();
                PowerYAxisValue = reader.ReadSingle();
                PowerRandom = reader.ReadSingle();
                PowerNormal = reader.ReadSingle();
                DiffusionEmitterNormal = reader.ReadSingle();
                PowerSpec = reader.ReadSingle();
                DiffusionSpec = reader.ReadSingle();
                EmissionAngle = new Vector3f
                (
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
                Scale = new Vector3f
                (
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
                Rotation = new Vector3f
                (
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
                Translation = new Vector3f
                (
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
                LODNearestDistance = reader.ReadByte();
                LODFarthestDistance = reader.ReadByte();
                LODMinimalEmission = reader.ReadByte();
                LODAlpha = reader.ReadByte();
                RandomSeed = reader.ReadUInt32();
                Unknown1 = reader.ReadUInt64();
                DrawFlagsBitfield = reader.ReadUInt16();
                AlphaComparison0 = reader.ReadByte();
                AlphaComparison1 = reader.ReadByte();
                AlphaCompareOperation = reader.ReadByte();
                TEVStageAmount = reader.ReadByte();
                Unknown2 = reader.ReadByte();
                EnableIndirectTEV = reader.ReadByte();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(Unknown0);
                writer.WriteUInt24(EmitFlags);
                writer.WriteByte(EmitShape);
                writer.WriteUInt16(EmitterLife);
                writer.WriteUInt16(ParticleLife);
                writer.WriteByte(ParticleLifeRandom);
                writer.WriteByte(Convert.ToByte(InheritChildParticleTranslation));
                writer.WriteByte(EmitIntervalRandom);
                writer.WriteByte(EmitRandom);
                writer.WriteSingle(EmissionRate);
                writer.WriteUInt16(EmitStart);
                writer.WriteUInt16(EmitEnd);
                writer.WriteUInt16(EmitInterval);
                writer.WriteByte(Convert.ToByte(InheritParticleTranslation));
                writer.WriteByte(Convert.ToByte(InheritChildEmitterTranslation));
                foreach (float f in EmitterDimensions)
                    writer.WriteSingle(f);
                writer.WriteUInt16(EmitDiversion);
                writer.WriteByte(VelocityRandom);
                writer.WriteByte(MomentumRandom);
                writer.WriteSingle(PowerRadiation);
                writer.WriteSingle(PowerYAxisValue);
                writer.WriteSingle(PowerRandom);
                writer.WriteSingle(PowerNormal);
                writer.WriteSingle(DiffusionEmitterNormal);
                writer.WriteSingle(PowerSpec);
                writer.WriteSingle(DiffusionSpec);

                writer.WriteSingle(EmissionAngle.X);
                writer.WriteSingle(EmissionAngle.Y);
                writer.WriteSingle(EmissionAngle.Z);

                writer.WriteSingle(Scale.X);
                writer.WriteSingle(Scale.Y);
                writer.WriteSingle(Scale.Z);

                writer.WriteSingle(Rotation.X);
                writer.WriteSingle(Rotation.Y);
                writer.WriteSingle(Rotation.Z);

                writer.WriteSingle(Translation.X);
                writer.WriteSingle(Translation.Y);
                writer.WriteSingle(Translation.Z);
                writer.WriteByte(LODNearestDistance);
                writer.WriteByte(LODFarthestDistance);
                writer.WriteByte(LODMinimalEmission);
                writer.WriteByte(LODAlpha);
                writer.WriteUInt32(RandomSeed);
                writer.WriteUInt64(Unknown1);
                writer.WriteUInt16(DrawFlagsBitfield);
                writer.WriteByte(AlphaComparison0);
                writer.WriteByte(AlphaComparison1);
                writer.WriteByte(AlphaCompareOperation);
                writer.WriteByte(TEVStageAmount);
                writer.WriteByte(Unknown2);
                writer.WriteByte(EnableIndirectTEV);
            }
        }

        public class _Shader
        {
            public List<_ShaderStage> ShaderStages;
            public byte[] Textures;
            public byte[][] ColorInputs;
            public byte[][] ColorOperations;
            public byte[][] AlphaInputs;
            public byte[][] AlphaOperations;

            public _Shader(EndianReader reader, int stageAmount)
            {
                Textures = reader.ReadBytes(4);
                ColorInputs = new byte[4][];
                for (int i = 0; i < 4; i++)
                    ColorInputs[i] = reader.ReadBytes(4);

                ColorOperations = new byte[4][];
                for (int i = 0; i < 4; i++)
                    ColorOperations[i] = reader.ReadBytes(5);

                AlphaInputs = new byte[4][];
                for (int i = 0; i < 4; i++)
                    AlphaInputs[i] = reader.ReadBytes(4);

                AlphaOperations = new byte[4][];
                for (int i = 0; i < 4; i++)
                    AlphaOperations[i] = reader.ReadBytes(5);

                ShaderStages = new List<_ShaderStage>();
                for (int i = 0; i < stageAmount; i++)
                {
                    ShaderStages.Add(new _ShaderStage(
                        Textures[i],
                        ColorInputs[i],
                        ColorOperations[i],
                        AlphaInputs[i],
                        AlphaOperations[i]
                    ));
                }
            }

            public void Write(EndianWriter writer)
            {
                for (int i = 0; i < ShaderStages.Count; i++)
                {
                    Textures[i] = ShaderStages[i].Texture;
                    ColorInputs[i] = ShaderStages[i].ColorInputs;
                    ColorOperations[i] = ShaderStages[i].ColorOperations;
                    AlphaInputs[i] = ShaderStages[i].AlphaInputs;
                    AlphaOperations[i] = ShaderStages[i].AlphaOperations;
                }

                writer.WriteBytes(Textures);
                for (int i = 0; i < 4; i++)
                    writer.WriteBytes(ColorInputs[i]);
                for (int i = 0; i < 4; i++)
                    writer.WriteBytes(ColorOperations[i]);
                for (int i = 0; i < 4; i++)
                    writer.WriteBytes(AlphaInputs[i]);
                for (int i = 0; i < 4; i++)
                    writer.WriteBytes(AlphaOperations[i]);
            }
        }

        public class _ShaderStage
        {
            public byte Texture;
            public byte[] ColorInputs;
            public byte[] ColorOperations;
            public byte[] AlphaInputs;
            public byte[] AlphaOperations;

            public _ShaderStage()
            {
                ColorInputs = new byte[4];
                ColorOperations = new byte[5];
                AlphaInputs = new byte[4];
                AlphaOperations = new byte[5];
            }

            public _ShaderStage(
                byte texture,
                byte[] colorInputs,
                byte[] colorOperations,
                byte[] alphaInputs,
                byte[] alphaOperations)
            {
                Texture = texture;
                ColorInputs = colorInputs;
                ColorOperations = colorOperations;
                AlphaInputs = alphaInputs;
                AlphaOperations = alphaOperations;
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteByte(Texture);
                writer.WriteBytes(ColorInputs);
                writer.WriteBytes(ColorOperations);
                writer.WriteBytes(AlphaInputs);
                writer.WriteBytes(AlphaOperations);
            }
        }

        public class _Color
        {
            public byte[] ConstColor;
            public byte[] ConstAlpha;
            public byte BlendMode;
            public byte BlendSourceFactor;
            public byte BlendDestFactor;
            public byte BlendOperation;
            public ulong TEVColor; // will be a struct; info hasn't yet been added to wiki
            public ulong TEVAlpha; // 
            public byte ZCompareFunction;
            public byte AlphaFlickType;
            public ushort AlphaFlickCycleLength;
            public byte AlphaFlickMax;
            public byte AlphaFlickAmplitude;

            public _Color(EndianReader reader)
            {
                ConstColor = reader.ReadBytes(4);
                ConstAlpha = reader.ReadBytes(4);
                BlendMode = reader.ReadByte();
                BlendSourceFactor = reader.ReadByte();
                BlendDestFactor = reader.ReadByte();
                BlendOperation = reader.ReadByte();
                TEVColor = reader.ReadUInt64();
                TEVAlpha = reader.ReadUInt64();
                ZCompareFunction = reader.ReadByte();
                AlphaFlickType = reader.ReadByte();
                AlphaFlickCycleLength = reader.ReadUInt16();
                AlphaFlickMax = reader.ReadByte();
                AlphaFlickAmplitude = reader.ReadByte();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteBytes(ConstColor);
                writer.WriteBytes(ConstAlpha);
                writer.WriteByte(BlendMode);
                writer.WriteByte(BlendSourceFactor);
                writer.WriteByte(BlendDestFactor);
                writer.WriteByte(BlendOperation);
                writer.WriteUInt64(TEVColor);
                writer.WriteUInt64(TEVAlpha);
                writer.WriteByte(ZCompareFunction);
                writer.WriteByte(AlphaFlickType);
                writer.WriteUInt16(AlphaFlickCycleLength);
                writer.WriteByte(AlphaFlickMax);
                writer.WriteByte(AlphaFlickAmplitude);
            }
        }

        public class _Lighting
        {
            public byte LightingMode;
            public byte LightingType;
            public byte[] LightingAmbientColor;
            public byte[] LightingDiffuseColor;
            public float LightingRadius;
            public Vector3f LightingPosition;
            public float[,] IndirectTextureMatrix;
            public sbyte IndirectTextureMatrixScale;
            public sbyte PivotX;
            public sbyte PivotY;
            public byte Padding;

            public _Lighting(EndianReader reader)
            {
                LightingMode = reader.ReadByte();
                LightingType = reader.ReadByte();
                LightingAmbientColor = reader.ReadBytes(4);
                LightingDiffuseColor = reader.ReadBytes(4);
                LightingRadius = reader.ReadSingle();
                LightingPosition = reader.ReadSingles(3);
                IndirectTextureMatrix = new float[2, 3];
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 3; j++)
                        IndirectTextureMatrix[i, j] = reader.ReadSingle();
                }
                IndirectTextureMatrixScale = reader.ReadSByte();
                PivotX = reader.ReadSByte();
                PivotY = reader.ReadSByte();
                Padding = reader.ReadByte();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteByte(LightingMode);
                writer.WriteByte(LightingType);
                writer.WriteBytes(LightingAmbientColor);
                writer.WriteBytes(LightingDiffuseColor);
                writer.WriteSingle(LightingRadius);
                writer.WriteSingles(LightingPosition);
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 3; j++)
                        writer.WriteSingle(IndirectTextureMatrix[i, j]);
                }
                writer.WriteSByte(IndirectTextureMatrixScale);
                writer.WriteSByte(PivotX);
                writer.WriteSByte(PivotY);
                writer.WriteByte(Padding);
            }
        }

        public class _Movement
        {
            public byte ParticleType;
            public byte ParticleVariant;
            public byte MovementDirection;
            public byte RotationAxis;
            public byte Setting1;
            public byte Setting2;
            public byte Setting3;
            public byte Padding;
            public float ZOffset;

            public _Movement(EndianReader reader)
            {
                ParticleType = reader.ReadByte();
                ParticleVariant = reader.ReadByte();
                MovementDirection = reader.ReadByte();
                RotationAxis = reader.ReadByte();
                Setting1 = reader.ReadByte();
                Setting2 = reader.ReadByte();
                Setting3 = reader.ReadByte();
                Padding = reader.ReadByte();
                ZOffset = reader.ReadSingle();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteByte(ParticleType);
                writer.WriteByte(ParticleVariant);
                writer.WriteByte(MovementDirection);
                writer.WriteByte(RotationAxis);
                writer.WriteByte(Setting1);
                writer.WriteByte(Setting2);
                writer.WriteByte(Setting3);
                writer.WriteByte(Padding);
                writer.WriteSingle(ZOffset);
            }
        }

        /////////////////////////
        /////////////////////////

        public _Header Header;
        public _EmitData EmitData;
        public _Shader Shader;
        public _Color Color;
        public _Lighting Lighting;
        public _Movement Movement;

        public Emitter(EndianReader reader)
        {
            Header = new _Header(reader);
            EmitData = new _EmitData(reader);
            Shader = new _Shader(reader, EmitData.TEVStageAmount);
            Color = new _Color(reader);
            Lighting = new _Lighting(reader);
            Movement = new _Movement(reader);
        }

        public void Write(EndianWriter writer)
        {
            Header.Write(writer);
            EmitData.Write(writer);
            Shader.Write(writer);
            Color.Write(writer);
            Lighting.Write(writer);
            Movement.Write(writer);
        }
    }

    public class Particle
    {
        public class _Header
        {
            public uint Size;

            public _Header(EndianReader reader)
            {
                Size = reader.ReadUInt32();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(Size);
            }
        }

        public class _ParticleData
        {
            public byte[] Color1A;
            public byte[] Color1B;
            public byte[] Color2A;
            public byte[] Color2B;
            public Vector2f Size;
            public Vector2f Scale;
            public Vector3f Rotation;
            public Vector2f TextureScale1;
            public Vector2f TextureScale2;
            public Vector2f TextureScale3;
            public Vector3f TextureRotation;
            public Vector2f TextureTranslate1;
            public Vector2f TextureTranslate2;
            public Vector2f TextureTranslate3;
            public uint mTexture1;
            public uint mTexture2;
            public uint mTexture3;
            public ushort TextureWrap;
            public byte TextureReverse;
            public byte AlphaCompareRef0;
            public byte AlphaCompareRef1;
            public byte RotateOffsetRandom1;
            public byte RotateOffsetRandom2;
            public byte RotateOffsetRandom3;
            public float[] RotateOffset;
            public ushort TexRef1Length;
            public string TexRef1;
            public ushort TexRef2Length;
            public string TexRef2;
            public ushort TexRef3Length;
            public string TexRef3;

            public _ParticleData(EndianReader reader)
            {
                Color1A = reader.ReadBytes(4);
                Color1B = reader.ReadBytes(4);
                Color2A = reader.ReadBytes(4);
                Color2B = reader.ReadBytes(4);
                Size = reader.ReadSingles(2);
                Scale = reader.ReadSingles(2);
                Rotation = reader.ReadSingles(3);
                TextureScale1 = reader.ReadSingles(2);
                TextureScale2 = reader.ReadSingles(2);
                TextureScale3 = reader.ReadSingles(2);
                TextureRotation = reader.ReadSingles(3);
                TextureTranslate1 = reader.ReadSingles(2);
                TextureTranslate2 = reader.ReadSingles(2);
                TextureTranslate3 = reader.ReadSingles(2);
                mTexture1 = reader.ReadUInt32();
                mTexture2 = reader.ReadUInt32();
                mTexture3 = reader.ReadUInt32();
                TextureWrap = reader.ReadUInt16();
                TextureReverse = reader.ReadByte();
                AlphaCompareRef0 = reader.ReadByte();
                AlphaCompareRef1 = reader.ReadByte();
                RotateOffsetRandom1 = reader.ReadByte();
                RotateOffsetRandom2 = reader.ReadByte();
                RotateOffsetRandom3 = reader.ReadByte();
                RotateOffset = reader.ReadSingles(3);
                TexRef1Length = reader.ReadUInt16();
                TexRef1 = reader.ReadStringNT();
                TexRef2Length = reader.ReadUInt16();
                TexRef2 = reader.ReadStringNT();
                TexRef3Length = reader.ReadUInt16();
                TexRef3 = reader.ReadStringNT();
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteBytes(Color1A);
                writer.WriteBytes(Color1B);
                writer.WriteBytes(Color2A);
                writer.WriteBytes(Color2B);
                writer.WriteSingles(Size);
                writer.WriteSingles(Scale);
                writer.WriteSingles(Rotation);
                writer.WriteSingles(TextureScale1);
                writer.WriteSingles(TextureScale2);
                writer.WriteSingles(TextureScale3);
                writer.WriteSingles(TextureRotation);
                writer.WriteSingles(TextureTranslate1);
                writer.WriteSingles(TextureTranslate2);
                writer.WriteSingles(TextureTranslate3);
                writer.WriteUInt32(mTexture1);
                writer.WriteUInt32(mTexture2);
                writer.WriteUInt32(mTexture3);
                writer.WriteUInt16(TextureWrap);
                writer.WriteByte(TextureReverse);
                writer.WriteByte(AlphaCompareRef0);
                writer.WriteByte(AlphaCompareRef1);
                writer.WriteByte(RotateOffsetRandom1);
                writer.WriteByte(RotateOffsetRandom2);
                writer.WriteByte(RotateOffsetRandom3);
                writer.WriteSingles(RotateOffset);
                writer.WriteUInt16(TexRef1Length);
                writer.WriteStringNT(TexRef1);
                writer.WriteUInt16(TexRef2Length);
                writer.WriteStringNT(TexRef2);
                writer.WriteUInt16(TexRef3Length);
                writer.WriteStringNT(TexRef3);
            }
        }

        /////////////////////
        /////////////////////

        public _Header Header;
        public _ParticleData ParticleData;

        public Particle(EndianReader reader)
        {
            Header = new _Header(reader);
            ParticleData = new _ParticleData(reader);
        }

        public void Write(EndianWriter writer)
        {
            Header.Write(writer);
            ParticleData.Write(writer);
        }
    }
}
