using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Map.Count}", Name = "{Name}")]
    public sealed class UEMapProperty : UEProperty
    {
        public UEMapProperty() { }
        public UEMapProperty(BinaryReader reader, long valueLength)
        {
            string keyType = reader.ReadUEString();
            string valueType = reader.ReadUEString();
            byte[] unknown = reader.ReadBytes(5);
            if (unknown.Any(b => b != 0))
            {
                throw new InvalidOperationException($"Offset: 0x{reader.BaseStream.Position - 5:x8}. Expected ??? to be 0, but was 0x{unknown.AsHex()}");
            }

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                UEProperty key, value;
                if (keyType == "StructProperty")
                {
                    key = Read(reader);
                }
                else
                {
                    key = UESerializer.Deserialize(null, keyType, -1, reader);
                }

                List<UEProperty> values = new List<UEProperty>();
                do
                {
                    if (valueType == "StructProperty")
                    {
                        value = Read(reader);
                    }
                    else
                    {
                        value = UESerializer.Deserialize(null, valueType, -1, reader);
                    }

                    values.Add(value);
                } while (!(value is UENoneProperty));
                Map.Add(new UEKeyValuePair { Key = key, Values = values });
            }
        }
        public override void Serialize(BinaryWriter writer) { throw new NotImplementedException(); }

        public List<UEKeyValuePair> Map = new List<UEKeyValuePair>();

        public class UEKeyValuePair
        {
            public UEProperty Key;
            public List<UEProperty> Values;
        }
    }
}