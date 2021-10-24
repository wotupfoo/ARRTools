using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RailroadsOnlineSaveViewer.Types;

namespace RailroadsOnlineSaveViewer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TurntableType
    {
        Wooden1,
        Wooden2
    }

    public class Turntable
    {
        public TurntableType Type;
        public Location Location;
        public Rotation Rotator;
        public Rotation DeckRotation;

        public Turntable() { }

        public Turntable(TurntableType type, Location location, Rotation rotator, Rotation deckRotation)
        {
            Type = type;
            Location = location;
            Rotator = rotator;
            DeckRotation = deckRotation;
        }
    }
}
