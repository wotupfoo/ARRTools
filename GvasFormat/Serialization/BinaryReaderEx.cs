using System.IO;
using System.Text;

namespace GvasFormat.Serialization
{
    public static class BinaryReaderEx
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public static string ReadUEString(this BinaryReader reader)
        {
            /*if (reader.PeekChar() < 0)
                return null;*/

            var length = reader.ReadInt32();
            if (length == 0)
                return null;

            if (length == 1 || length == -1)
                return "";

            byte[] valueBytes = reader.ReadBytes(length > 0 ? length : -2*length);

            if (length > 0)
                return Utf8.GetString(valueBytes, 0, valueBytes.Length - 1);
            else
                return Encoding.Unicode.GetString(valueBytes, 0, valueBytes.Length - 2);            ;
        }

        public static void WriteUEString(this BinaryWriter writer, string value)
        {
            if (value == null)
            {
                writer.Write(0);
                return;
            }

            var valueBytes = Utf8.GetBytes(value);
            writer.Write(valueBytes.Length + 1);
            if (valueBytes.Length > 0)
                writer.Write(valueBytes);
            writer.Write((byte)0);
        }
    }
}
