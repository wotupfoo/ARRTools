using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ARRSaveFormat.Types;

namespace ARRSaveFormat
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndustryType
    {
        unknown,
        industry_loggingcamp,
        industry_sawmill,
        industry_smelter,
        industry_ironworks,
        industry_oilfield,
        industry_refinery,
        industry_coalmine,
        industry_ironoremine,
        industry_freightdepot,
        industry_firewooddepot
    }

    public class Industry : StaticObject
    {
        public IndustryType Type;
        public int[] EductsStored;
        public int[] ProductsStored;

        public Industry() { }

        public Industry(IndustryType type, Location location, Rotation rotation, int[] eductsStored, int[] productsStored) : base(location, rotation)
        {
            Type = type;
            EductsStored = eductsStored;
            ProductsStored = productsStored;
        }
    }
}
