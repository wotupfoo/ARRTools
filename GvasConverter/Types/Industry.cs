using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RailroadsOnlineSaveViewer.Types;

namespace RailroadsOnlineSaveViewer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndustryType
    {
        Spawn,
        CargoDepot, //TODO: check industry types order!!!
        LoggingCamp,
        Sawmill,
        Smelter,
        IronMine,
        CoalMine,
        Ironworks,
        Oilfield,
        Refinery,
        Firewood
    }

    public class Industry: StaticObject
    {
        public IndustryType Type;
        public int[] EductsStored;
        public int[] ProductsStored;

        public Industry() { }

        public Industry(IndustryType type, Location location, Rotation rotation, int[] eductsStored, int[] productsStored): base(location, rotation)
        {
            Type = type;
            EductsStored = eductsStored;
            ProductsStored = productsStored;
        }
    }
}
