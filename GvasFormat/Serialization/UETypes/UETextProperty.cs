using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UETextProperty : UEProperty
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public UETextProperty() { }
        public UETextProperty(BinaryReader reader, long valueLength, bool noTerminator = false)
        {
            var pos = reader.BaseStream.Position;
            var terminator = reader.ReadByte();

            Flags = reader.ReadInt64();

            switch (terminator)
            {
                case 1:
                    byte[] b1 = reader.ReadBytes(5);
                    Type = reader.ReadUEString();
                    string separator = reader.ReadUEString();
                    int numRows = reader.ReadInt32();
                    (string, byte, UETextProperty)[] rowsData = new (string, byte, UETextProperty)[numRows];
                    for (int i = 0; i < numRows; i++)
                    {
                        string rowId = reader.ReadUEString();
                        byte rowType = reader.ReadByte();
                        if (rowType != 4)
                            throw new NotImplementedException();

                        UETextProperty rowData = new UETextProperty(reader, valueLength);

                        rowsData[i] = (rowId, rowType, rowData);
                    }
                    Value = string.Format(separator, rowsData.Select(x => x.Item3.Value).ToArray());
                    break;
                case 2:
                    if (((Flags >> 32) & 0xFFFFFFFF) != 0)
                        Value = reader.ReadUEString();
                    break;

            }

            /*if (((Flags >> 32) & 0b1) != 0)
                Id = reader.ReadUEString();
            else
                Id = null;

            if (((Flags >> 32) & 0b10) != 0)
                Value = reader.ReadUEString();
            else
                Value = null;*/
        }

        public static UEProperty[] Read(BinaryReader reader, long valueLength, int count)
        {
            UEProperty[] result = new UEProperty[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new UETextProperty(reader, 0, true);
            }
            return result;
        }

        public override void Serialize(BinaryWriter writer) => throw new NotImplementedException();

        public long Flags;
        public string Id;
        public string Value;
    }
}