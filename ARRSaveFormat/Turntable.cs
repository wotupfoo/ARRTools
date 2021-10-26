using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ARRSaveFormat.Types;

namespace ARRSaveFormat.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TurntableType
    {
        turntable_small_nodeck,
        turntable_small_deck
    }

    public class Turntable: StaticObject
    {
        public TurntableType Type;
        public Rotation Rotator;

        public Turntable() { }

        public Turntable(TurntableType type, Location location, Rotation rotator, Rotation deckRotation): base(location, deckRotation)
        {
            Type = type;
            Rotator = rotator;
        }
    }
}
