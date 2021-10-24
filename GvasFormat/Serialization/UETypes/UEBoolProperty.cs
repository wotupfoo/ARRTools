using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEBoolProperty : UEProperty
    {
        public UEBoolProperty() { }
        public UEBoolProperty(BinaryReader reader, long valueLength)
        {
            if (valueLength != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected bool value length 0, but was {valueLength}");

            var val = reader.ReadByte();
            if (val == 0)
                Value = false;
            else if (val == 1)
                Value = true;
            else
                throw new InvalidOperationException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected bool value, but was {val}");
        }
        public static UEProperty[] Read(BinaryReader reader, long valueLength, int count)
        {
            UEProperty[] result = new UEProperty[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new UEBoolProperty(reader, 0);
            }
            return result;
        }

        public override void Serialize(BinaryWriter writer) { throw new NotImplementedException(); }

        public bool Value;
    }
}