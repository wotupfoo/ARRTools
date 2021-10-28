using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ARRSaveFormat.Types;

namespace ARRSaveFormat.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SwitchType
    {
        SwitchLeft,
        SwitchRight,
        SwitchY,
        SwitchYMirror,
        SwitchRightMirror,
        SwitchLeftMirror,
        SwitchCross90
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SwitchState
    {
        Main,
        Side,
        Switching //TODO: check if exists actually
    }

    public class Switch : StaticObject
    {
        public SwitchType Type;
        public SwitchState State;

        public Switch() { }

        public Switch(SwitchType type, Location location, Rotation rotation, SwitchState state) : base(location, rotation)
        {
            Type = type;
            State = state;
        }
    }
}
