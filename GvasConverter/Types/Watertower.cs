using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RailroadsOnlineSaveViewer.Types;

namespace RailroadsOnlineSaveViewer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WatertowerType
    {
        Wooden
    }

    public class Watertower: StaticObject
    {
        public WatertowerType Type;
        public float WaterLevel;

        public Watertower() { }

        public Watertower(WatertowerType type, Location location, Rotation rotation, float waterLevel): base(location, rotation)
        {
            Type = type;
            WaterLevel = waterLevel;
        }
    }
}
