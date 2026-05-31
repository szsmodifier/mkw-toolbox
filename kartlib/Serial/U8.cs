using System.Diagnostics;
using System.Text;

namespace kartlib.Serial
{
    public class U8
    {
        public const int HEADER_SIZE = 0x20;
        public const int NODE_SIZE = 0x0C;

        public class _Header
        {
            public UInt32 Magic;
            public Int32 NodeOffset;
            public Int32 NodeSize;
            public Int32 DataOffset;
            public Int32[] Reserved;

            public _Header()
            {
                Magic = 0x55AA382D;
                NodeOffset = 0x20;
                NodeSize = 0;
                DataOffset = 0;
                Reserved = new int[4];
            }

            public _Header(EndianReader reader)
            {
                Magic = reader.ReadUInt32();
                NodeOffset = reader.ReadInt32();
                NodeSize = reader.ReadInt32();
                DataOffset = reader.ReadInt32();
                Reserved = reader.ReadInt32s(4);
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteUInt32(Magic);
                writer.WriteInt32(NodeOffset);
                writer.WriteInt32(NodeSize);
                writer.WriteInt32(DataOffset);
                writer.WriteInt32s(Reserved);
            }
        }

        public class _Node
        {
            public enum NodeType : Byte
            {
                File = 0,
                Directory = 1
            }

            public NodeType Type;       // 0x00 = File, 0x01 = Directory
            public UInt32 NameOffset;   // Offset into string pool
            public UInt32 DataOffset;
            public UInt32 DataSize;

            public string Name;
            public Byte[]? Data;

            public _Node()
            {
                Type = NodeType.Directory;
                NameOffset = 0;
                DataOffset = 0;
                DataSize = 1;
                Name = "";
                Data = new byte[] { };
            }

            public _Node(EndianReader reader, int stringPoolOffset)
            {
                Type = (NodeType)reader.ReadByte();
                NameOffset = reader.ReadUInt24();
                DataOffset = reader.ReadUInt32();
                DataSize = reader.ReadUInt32();

                // Get name from string pool
                int initialPos = reader.Position;
                reader.Position = (int)(stringPoolOffset + NameOffset);
                Name = reader.ReadStringNT(Encoding.ASCII);

                // Get file data from data pool
                if(Type == NodeType.File)
                {
                    reader.Position = (int)DataOffset;
                    Data = reader.ReadBytes((int)DataSize);
                }
                reader.Position = initialPos;
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteByte((Byte)Type);
                writer.WriteUInt24(NameOffset);
                writer.WriteUInt32(DataOffset);
                writer.WriteUInt32(DataSize);
            }
        }

        public string Filename;
        public _Header Header;
        public List<_Node> Nodes;

        public U8()
        {
            Filename = "Untitled.arc";
            Header = new _Header();
            Nodes = new List<_Node>() { new _Node() };
        }

        public U8(byte[] buffer, string fileName)
        {
            Filename = fileName;

            EndianReader reader = new EndianReader(buffer, Endianness.BigEndian);
            try
            {
                Header = new _Header(reader);

                // Create head node
                reader.Position = Header.NodeOffset;
                Nodes = new List<_Node>
                {
                    new _Node(reader, 0)
                };

                // Create remaining nodes
                int stringTableOffset = Header.NodeOffset + (int)Nodes[0].DataSize * NODE_SIZE;
                for (int i = 1; i < Nodes[0].DataSize; i++)
                    Nodes.Add(new _Node(reader, stringTableOffset));
            }
            finally
            {
                reader.Close();
            }
        }

        public byte[] Write()
        {
            UpdateInternalData();
            MemoryStream stream = new MemoryStream();
            EndianWriter writer = new EndianWriter(stream, Endianness.BigEndian);
            try
            {
                // Write header and nodes
                this.Header.Write(writer);
                foreach (_Node n in Nodes)
                    n.Write(writer);

                // Write string pool
                writer.WriteByte(0);
                for (int i = 1; i < Nodes.Count; i++)
                    writer.WriteStringNT(Nodes[i].Name, Encoding.ASCII);

                // Write padding
                while (writer.Position % HEADER_SIZE != 0)
                    writer.WriteByte(0);

                // Write binary data for each file
                foreach (_Node n in Nodes)
                {
                    if (n.Type == _Node.NodeType.File)
                    {
                        writer.WriteBytes(n.Data);
                        writer.HardAlign(HEADER_SIZE);
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

        public int[] GetChildren(int index)
        {
            List<int> result = new List<int>();
            if (Nodes[index].Type != _Node.NodeType.Directory)
                return result.ToArray();

            for(int i = index + 1; i < Nodes[index].DataSize; i++)
            {
                if (Nodes[i].Type == _Node.NodeType.Directory)
                {
                    if (Nodes[i].DataOffset == index)
                        result.Add(i);
                    i = (int)Nodes[i].DataSize - 1;
                }
                else
                {
                    result.Add(i);
                }
            }

            return result.ToArray();
        }

        public int FindIndexFromName(string name)
        {
            for(int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Name == name)
                    return i;
            }
            return -1;
        }

        public int GetIndexFromNode(_Node node)
        {
            for(int i = 0; i < Nodes.Count;i++)
            {
                if(node == Nodes[i])
                    return i;
            }
            return -1;
        }

        public void AddFile(int index, byte[] buffer, string fileName)
        {
            // Create node
            _Node node = new _Node();
            node.Name = Path.GetFileName(fileName);
            node.Data = buffer;
            node.DataSize = (uint)buffer.Length;
            node.Type = _Node.NodeType.File;

            // Insert node
            Nodes.Insert(index + 1, node);
            IncrementInternalFolderData(index, 1);
        }

        public void AddFolder(int index, string name)
        {
            int newIndex = GetNodeFolder(index);

            // Create node
            _Node node = new _Node();
            node.Name = name;
            node.Type = _Node.NodeType.Directory;
            node.DataOffset = (uint)newIndex;
            node.DataSize = Nodes[newIndex].DataSize;

            // Insert node
            Nodes.Insert((int)node.DataSize, node);
            IncrementInternalFolderData((int)node.DataSize, 1);
        }

        public void RemoveFile(int index)
        {
            IncrementInternalFolderData(index, -1);
            Nodes.RemoveAt(index);
        }

        public void RemoveFolder(int index)
        {
            int newIndex = GetNodeFolder(index);

            int start = (int)Nodes[newIndex].DataSize - 1;
            int end   = newIndex;
            for(int i = start; i >= end; i--)
                RemoveFile(i);
        }

        public void Rename(int index, string name)
        {
            Nodes[index].Name = name;
        }

        public int GetNodeFolder(int index)
        {
            for (; Nodes[index].Type != _Node.NodeType.Directory && index > 0; index--) { }
            return index;
        }

        private void IncrementInternalFolderData(int index, int value)
        {
            index = GetNodeFolder(index);
            Nodes[0].DataSize += (uint)value;

            // Update parent folders
            int parent = index;
            for(; parent > 0; parent = (int)Nodes[parent].DataOffset)
                Nodes[parent].DataSize += (uint)value;

            // Update following folders and nested folders
            for (parent = index; Nodes[parent].DataOffset > 0; parent = (int)Nodes[parent].DataOffset) { }
            for (int i = index + 1; i < Nodes.Count; i++)
            {
                if (Nodes[i].Type == _Node.NodeType.Directory)
                {
                    if (Nodes[i].DataOffset > parent) Nodes[i].DataOffset += (uint)value;
                    Nodes[i].DataSize += (uint)value;
                }
            }
        }

        private void UpdateInternalData()
        {
            //// Update root node
            //Nodes[0].DataSize = (uint)Nodes.Count - 1;

            //// Update string pool offsets
            //uint stringPoolOffset = 1u;
            //for (int i = 1; i < Nodes.Count; i++)
            //{
            //    Nodes[i].NameOffset = stringPoolOffset;
            //    stringPoolOffset += (uint)Nodes[i].Name.Length + 1;
            //}

            //// Get offset to end of string pool
            //uint offset = HEADER_SIZE + (uint)Nodes.Count * NODE_SIZE;
            //offset += stringPoolOffset;

            //// Update size of all nodes and realign offset
            //Header.NodeSize = (int)offset - HEADER_SIZE;
            //for (; offset % 32u != 0; offset++) { }

            //// Update data offsets
            //Header.DataOffset = (int)offset;
            //uint dataOffset = 0u;
            //for(int i = 1; i < Nodes.Count; i++)
            //{
            //    if (Nodes[i].Type == _Node.NodeType.File)
            //    {
            //        Nodes[i].DataOffset = dataOffset + (uint)Header.DataOffset;
            //        Nodes[i].DataSize = (uint)Nodes[i].Data.Length;
            //        dataOffset += Nodes[i].DataSize;
            //        for (; dataOffset % 32u != 0; dataOffset++) { }
            //    }
            //}

            uint num = (uint)(32 + this.Nodes.Count * 12);
            uint num2 = 1u;
            for (int i = 1; i < this.Nodes.Count; i++)
            {
                this.Nodes[i].NameOffset = num2;
                num2 += (uint)(this.Nodes[i].Name.Length + 1);
            }
            num += num2;
            this.Header.NodeSize = (int)num - 32;
            for (; num % 32u != 0; num++)
            {
            }
            this.Header.DataOffset = (int)num;
            num2 = 0u;
            for (int j = 1; j < this.Nodes.Count; j++)
            {
                if (this.Nodes[j].Type == _Node.NodeType.File)
                {
                    this.Nodes[j].DataOffset = num2 + (uint)this.Header.DataOffset;
                    this.Nodes[j].DataSize = (uint)this.Nodes[j].Data.Length;
                    for (num2 += (uint)this.Nodes[j].Data.Length; num2 % 32u != 0; num2++)
                    {
                    }
                }
            }
        }

    }
}
