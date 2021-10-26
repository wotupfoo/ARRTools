using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ARRSaveFormat.Types;

namespace ARRSaveFormat.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WatertowerType
    {
        watertower_small
    }

    public class Watertower : StaticObject
    {
        public WatertowerType Type;
        public float WaterLevel;

        public Watertower() { }

        public Watertower(WatertowerType type, Location location, Rotation rotation, float waterLevel) : base(location, rotation)
        {
            Type = type;
            WaterLevel = waterLevel;
        }
    }
}
