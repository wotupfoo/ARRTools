using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RailroadsOnlineSaveViewer.Types;

namespace RailroadsOnlineSaveViewer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SandhouseType
    {
        Wooden
    }

    public class Sandhouse: StaticObject
    {
        public SandhouseType Type;

        public Sandhouse() { }

        public Sandhouse(SandhouseType type, Location location, Rotation rotation): base(location, rotation)
        {
            Type = type;
        }
    }
}
