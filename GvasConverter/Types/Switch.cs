using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RailroadsOnlineSaveViewer.Types;

namespace RailroadsOnlineSaveViewer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SwitchType
    {
        LeftControlLeft,
        LeftControlRight,
        RightControlLeft,
        RightControlRight
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SwitchState
    {
        Main,
        Side,
        Switching //TODO: check if exists actually
    }

    public class Switch: StaticObject
    {
        public SwitchType Type;
        public SwitchState State;

        public Switch() { }

        public Switch(SwitchType type, Location location, Rotation rotation, SwitchState state): base(location, rotation)
        {
            Type = type;
            State = state;
        }
    }
}
