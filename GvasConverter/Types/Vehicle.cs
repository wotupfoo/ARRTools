using Newtonsoft.Json.Converters;
using RailroadsOnlineSaveViewer.Types;
using System.Text.Json.Serialization;

namespace RailroadsOnlineSaveViewer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarkerLightState
    {
        None,
        Green,
        Red
    }

    public class Vehicle
    {
        public string Type;
        public Location Location;
        public Rotation Rotation;
        public string Number;
        public string Name;
        public int SmokestackType;
        public int HeadlightType;
        public float BoilerFuelAmount;
        public float BoilerFireTemp;
        public float BoilerWaterTemp;
        public float BoilerWaterLevel;
        public float BoilerPressure;
        public bool HeadlightFront;
        public bool HeadlightRear;
        public bool CouplerFrontState;
        public bool CouplerRearState;
        public float TenderFuelAmount;
        public float TenderWaterAmount;
        public float CompressorAirPressure;
        public MarkerLightState MarkerLightsFrontRightState;
        public MarkerLightState MarkerLightsFrontLeftState;
        public MarkerLightState MarkerLightsRearRightState;
        public MarkerLightState MarkerLightsRearLeftState;
        public string FreightType;
        public int FreightAmount;
        public float RegulatorValue;
        public float BrakeValue;
        public float GeneratorValveValue;
        public float CompressorValveValue;
        public float ReverserValue;

        public Vehicle() { }

        public Vehicle(string type, Location location, Rotation rotation, string number, string name, int smokestackType, int headlightType, float boilerFuelAmmount, float boilerFireTemp, float boilerWaterTemp, float boilerWaterLevel, float boilerPressure, bool headlightFront, bool headlightRear, bool couplerFrontState, bool couplerRearState, float tenderFuelAmmount, float tenderWaterAmmount, float compressorAirPressure, MarkerLightState markerLightsFrontRightState, MarkerLightState markerLightsFrontLeftState, MarkerLightState markerLightsRearRightState, MarkerLightState markerLightsRearLeftState, string freightType, int freightAmmount, float regulatorValue, float brakeValue, float generatorValveValue, float compressorValveValue, float reverserValue)
        {
            Type = type;
            Location = location;
            Rotation = rotation;
            Number = number;
            Name = name;
            SmokestackType = smokestackType;
            HeadlightType = headlightType;
            BoilerFuelAmount = boilerFuelAmmount;
            BoilerFireTemp = boilerFireTemp;
            BoilerWaterTemp = boilerWaterTemp;
            BoilerWaterLevel = boilerWaterLevel;
            BoilerPressure = boilerPressure;
            HeadlightFront = headlightFront;
            HeadlightRear = headlightRear;
            CouplerFrontState = couplerFrontState;
            CouplerRearState = couplerRearState;
            TenderFuelAmount = tenderFuelAmmount;
            TenderWaterAmount = tenderWaterAmmount;
            CompressorAirPressure = compressorAirPressure;
            MarkerLightsFrontRightState = markerLightsFrontRightState;
            MarkerLightsFrontLeftState = markerLightsFrontLeftState;
            MarkerLightsRearRightState = markerLightsRearRightState;
            MarkerLightsRearLeftState = markerLightsRearLeftState;
            FreightType = freightType;
            FreightAmount = freightAmmount;
            RegulatorValue = regulatorValue;
            BrakeValue = brakeValue;
            GeneratorValveValue = generatorValveValue;
            CompressorValveValue = compressorValveValue;
            ReverserValue = reverserValue;
        }
    }
}
