using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEIntProperty : UEProperty
    {
        public UEIntProperty() { }
        public UEIntProperty(BinaryReader reader, long valueLength, bool noTerminator = false)
        {
            if (!noTerminator)
            {
                var terminator = reader.ReadByte();
                if (terminator != 0)
                    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            }

            if (valueLength != sizeof(int))
                throw new FormatException($"Expected int value of length {sizeof(int)}, but was {valueLength}");

            Value = reader.ReadInt32();
        }

        public static UEProperty[] Read(BinaryReader reader, long valueLength, int count)
        {
            UEProperty[] result = new UEProperty[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new UEIntProperty(reader, valueLength / (count + 1), true);
            }
            return result;
        }

        public override void Serialize(BinaryWriter writer) { throw new NotImplementedException(); }

        public int Value;
    }
}