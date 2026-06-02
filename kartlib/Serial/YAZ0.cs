namespace kartlib.Serial
{
    public class YAZ0
    {
        public class _Header
        {
            public UInt32 Magic;
            public UInt32 UncompressedSize;
            public UInt32[] Reserved;

            public _Header(EndianReader reader)
            {
                Magic               = reader.ReadUInt32();
                UncompressedSize    = reader.ReadUInt32();
                Reserved            = reader.ReadUInt32s(2);
            }

            public static void Write(EndianWriter writer, uint size)
            {
                writer.WriteUInt32(0x59617A30);
                writer.WriteUInt32(size);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
            }
        }

        public enum CompressionAlgorithm
        {
            Fast = 0,
            Optimal = 1
        }

        public static byte[] Compress(byte[] data, CompressionAlgorithm algorithm = CompressionAlgorithm.Optimal)
        {
            switch (algorithm)
            {
                case CompressionAlgorithm.Fast:
                    return FastEncode(data);
                case CompressionAlgorithm.Optimal:
                default:
                    return OptimalEncode(data);
            }
        }

        private static byte[] OptimalEncode(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            EndianWriter writer = new EndianWriter(stream, Endianness.BigEndian);

            try
            {
                _Header.Write(writer, (uint)data.Length);

                int position = 0;
                byte chunkCode = 0;
                int bitsLeft = 8;

                // Max size of 8 commands is 8 * 3 bytes = 24 bytes
                byte[] chunkData = new byte[24];
                int chunkDataIdx = 0;

                while (position < data.Length)
                {
                    Search(data, position, out int matchOffset, out int matchLength);
                    if (matchLength >= 3)
                    {
                        Search(data, position + 1, out int nextMatchOffset, out int nextMatchLength);
                        if (nextMatchLength >= matchLength + 2)
                        {
                            matchLength = 1;
                        }
                    }

                    if (matchLength < 3)
                    {
                        chunkCode |= (byte)(1 << (bitsLeft - 1));
                        chunkData[chunkDataIdx++] = data[position];
                        position++;
                    }
                    else
                    {
                        if (matchLength > 17)
                        {
                            chunkData[chunkDataIdx++] = (byte)((matchOffset - 1) >> 8);
                            chunkData[chunkDataIdx++] = (byte)((matchOffset - 1) & 0xFF);
                            chunkData[chunkDataIdx++] = (byte)(matchLength - 18);
                        }
                        else
                        {
                            chunkData[chunkDataIdx++] = (byte)(((matchLength - 2) << 4) | ((matchOffset - 1) >> 8));
                            chunkData[chunkDataIdx++] = (byte)((matchOffset - 1) & 0xFF);
                        }
                        position += matchLength;
                    }

                    bitsLeft--;

                    if (bitsLeft == 0 || position >= data.Length)
                    {
                        writer.WriteByte(chunkCode);
                        for (int i = 0; i < chunkDataIdx; i++)
                        {
                            writer.WriteByte(chunkData[i]);
                        }

                        chunkCode = 0;
                        chunkDataIdx = 0;
                        bitsLeft = 8;
                    }
                }
            }
            finally
            {
                stream.Close();
                writer.Close();
            }

            return stream.ToArray();
        }

        private static void Search(byte[] data, int position, out int matchOffset, out int matchLength)
        {
            matchLength = 0;
            matchOffset = 0;

            int startPos = position - 0x1000;
            if (startPos < 0) startPos = 0;

            int maxMatchLen = data.Length - position;
            if (maxMatchLen > 0x111) maxMatchLen = 0x111;

            for (int i = startPos; i < position; i++)
            {
                int currentMatchLen = 0;
                while (currentMatchLen < maxMatchLen && data[i + currentMatchLen] == data[position + currentMatchLen])
                {
                    currentMatchLen++;
                }

                if (currentMatchLen > matchLength)
                {
                    matchLength = currentMatchLen;
                    matchOffset = position - i;
                }
            }
        }

        public static byte[] Decompress(byte[] buffer)
        {
            EndianReader reader = new EndianReader(buffer, Endianness.BigEndian);
            try
            {
                _Header header = new _Header(reader);
                byte[]  result = new byte[header.UncompressedSize]; 
                int     offset = 0;

                Func<bool, int> readGroup = (x) =>
                {
                    if(x)
                    {
                        result[offset++] = reader.ReadByte();
                        return 0;
                    }

                    int group   = (reader.ReadByte() << 8) | reader.ReadByte();
                    int reverse = (group & 0xFFF) + 1;
                    int gSize   = group >> 12;

                    int size = gSize > 0 ? gSize + 2 : reader.ReadByte() + 18;

                    for(int i = 0; i < size; i++) 
                        result[offset] = result[offset++ - reverse];
                    return 0;
                };

                Func<int> readChunk = () =>
                {
                    byte chunkHeader = reader.ReadByte();
                    for(int i = 0; i < 8; i++)
                    {
                        if(reader.Position >= reader.StreamLength)  return 1;
                        if(offset >= header.UncompressedSize)       return 1;
                        readGroup((chunkHeader & (1 << (7 - i))) > 0);
                    }
                    return 0;
                };

                while (reader.Position < reader.StreamLength && offset < header.UncompressedSize)
                    readChunk();

                return result;
            }
            finally
            {
                reader.Close();
            }
        }

        private static byte[] FastEncode(byte[] buffer)
        {
            MemoryStream stream = new MemoryStream();
            EndianWriter writer = new EndianWriter(stream, Endianness.BigEndian);
            try
            {
                _Header.Write(writer, (uint)buffer.Length);

                int counter = 0;
                for (int i = 0; i < buffer.Length;)
                {
                    if (counter == 0)
                    {
                        writer.WriteByte(0xFF);
                        counter = 8;
                    }
                    writer.WriteByte(buffer[i++]);
                    counter--;
                }
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
