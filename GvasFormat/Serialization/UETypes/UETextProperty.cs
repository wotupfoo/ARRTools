using System;
using System.Diagnostics;
using System.IO;
using System.Text;

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
            /*if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");*/

            // valueLength starts here
            Flags = reader.ReadInt64();
            /*
                    if (Flags != 0)
                        throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 8:x8}. Expected text ??? {0x00}, but was {Flags:x16}");
            */

            //var terminator1 = reader.ReadByte();
            /*if (terminator1 != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator1:x2})");*/

            switch (terminator)
            {
                case 1:
                    var terminator1 = reader.ReadByte();
                    var str = reader.ReadUEString();
                    Id = reader.ReadUEString();
                    Value = reader.ReadUEString();
                    byte[] b = reader.ReadBytes(11);
                    break;
                case 2:
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