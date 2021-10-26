using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ARRSaveFormat.Types;

namespace ARRSaveFormat.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SandhouseType
    {
        sandhouse
    }

    public class Sandhouse : StaticObject
    {
        public SandhouseType Type;

        public Sandhouse() { }

        public Sandhouse(SandhouseType type, Location location, Rotation rotation) : base(location, rotation)
        {
            Type = type;
        }
    }
}
