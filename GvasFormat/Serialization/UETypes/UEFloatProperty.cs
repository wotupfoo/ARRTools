using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEFloatProperty : UEProperty
    {
        public UEFloatProperty() { }
        public UEFloatProperty(BinaryReader reader, long valueLength, bool noTerminator = false)
        {
            if (!noTerminator)
            {
                byte terminator = reader.ReadByte();
                if (terminator != 0)
                {
                    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
                }
            }

            if (valueLength != sizeof(float))
            {
                throw new FormatException($"Expected float value of length {sizeof(float)}, but was {valueLength}");
            }

            Value = reader.ReadSingle();
        }

        public static UEProperty[] Read(BinaryReader reader, long valueLength, int count)
        {
            UEProperty[] result = new UEProperty[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new UEFloatProperty(reader, valueLength / (count + 1), true);
            }
            return result;
        }

        public override void Serialize(BinaryWriter writer) { throw new NotImplementedException(); }

        public float Value;
    }
}