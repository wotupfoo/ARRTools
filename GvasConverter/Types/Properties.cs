using RailroadsOnlineSaveViewer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using GvasFormat.Serialization.UETypes;

namespace RailroadsOnlineSaveViewer
{

    public class Properties
    {
        public string SaveGameDate;

        public Player[] Players;
        public Spline[] Splines;
        public StaticObject[] StaticObjects;
        public Turntable[] Turntables;
        public Vehicle[] Vehicles;
        public Location[] RemovedVegetationAssets;

        private Location[] splineControlPoints;
        private bool[] splineVisibilityPoints;

        private Switch[] Switches;
        private Watertower[] Watertowers;
        private Sandhouse[] Sandhouses;
        private Industry[] Industries;

        public Properties(GvasFormat.Gvas save)
        {
            save.Properties.ForEach(property => {
                switch (property.Name)
                {
                    case "SaveGameDate":
                        SaveGameDate = ((UEStringProperty)property).Value;
                        break;
                    case "PlayerNameArray":
                    case "PlayerLocationArray":
                    case "PlayerRotationArray":
                    case "PlayerXPArray":
                    case "PlayerMoneyArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Players == null)
                        {
                            Players = new Player[arrayProperty.Items.Length];
                            for (int i = 0; i < Players.Length; i++)
                                Players[i] = new Player();
                        }

                        switch (arrayProperty.Name)
                        {
                            case "PlayerNameArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Players[i].Name = ((UEStringProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "PlayerLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Players[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "PlayerRotationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Players[i].Rotation = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "PlayerXPArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Players[i].XP = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "PlayerMoneyArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Players[i].Money = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                        }
                        break;
                    }
                    case "SplineLocationArray":
                    case "SplineTypeArray":
                    case "SplineControlPointsArray":
                    case "SplineControlPointsIndexStartArray":
                    case "SplineControlPointsIndexEndArray":
                    case "SplineSegmentsVisibilityArray":
                    case "SplineVisibilityStartArray":
                    case "SplineVisibilityEndArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Splines == null)
                        {
                            Splines = new Spline[arrayProperty.Items.Length];
                            for (int i = 0; i < Splines.Length; i++)
                                Splines[i] = new Spline();
                        }

                        switch (arrayProperty.Name)
                        {
                            case "SplineLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Splines[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "SplineTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    SplineType type = (SplineType)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Splines[i].Type = type;
                                }
                                break;
                            case "SplineControlPointsArray":
                                splineControlPoints = new Location[arrayProperty.Items.Length];
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    splineControlPoints[i] = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "SplineControlPointsIndexStartArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    int index = (int)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Splines[i].SetControlPointIndexStart(index);
                                }
                                break;
                            case "SplineControlPointsIndexEndArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    int index = (int)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Splines[i].SetControlPointIndexEnd(index);
                                }
                                break;
                            case "SplineSegmentsVisibilityArray":
                                splineVisibilityPoints = new bool[arrayProperty.Items.Length];
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    bool visible = (bool)((UEBoolProperty) arrayProperty.Items[i]).Value;

                                    splineVisibilityPoints[i] = visible;
                                }
                                break;
                            case "SplineVisibilityStartArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    int index = (int)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Splines[i].SetVisibilityIndexStart(index);
                                }
                                break;
                            case "SplineVisibilityEndArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    int index = (int)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Splines[i].SetVisibilityIndexEnd(index);
                                }
                                break;

                        }
                        break;
                    }
                    case "SwitchTypeArray":
                    case "SwitchLocationArray":
                    case "SwitchRotationArray":
                    case "SwitchStateArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Switches == null)
                        {
                            Switches = new Switch[arrayProperty.Items.Length];
                            for (int i = 0; i < Switches.Length; i++)
                                Switches[i] = new Switch();
                        }
                        switch (arrayProperty.Name)
                        {
                            case "SwitchTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    SwitchType type = (SwitchType)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Switches[i].Type = type;
                                }
                                break;
                            case "SwitchLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Switches[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "SwitchRotationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty rotation = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Switches[i].Rotation = new Rotation(rotation.X, rotation.Y, rotation.Z);
                                }
                                break;
                            case "SwitchStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    SwitchState state = (SwitchState)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Switches[i].State = state;
                                }
                                break;
                        }
                        break;
                    }
                    case "TurntableLocationArray":
                    case "TurntableRotatorArray":
                    case "TurntableDeckRotationArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Turntables == null)
                        {
                            Turntables = new Turntable[arrayProperty.Items.Length];
                            for (int i = 0; i < Turntables.Length; i++)
                                Turntables[i] = new Turntable();
                        }

                        switch (arrayProperty.Name)
                        {
                            case "TurntableLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Turntables[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "TurntableRotatorArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty rotation = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Turntables[i].Rotator = new Rotation(rotation.X, rotation.Y, rotation.Z);
                                }
                                break;
                            case "TurntableDeckRotationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty rotation = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Turntables[i].DeckRotation = new Rotation(rotation.X, rotation.Y, rotation.Z);
                                }
                                break;

                        }
                        break;
                    }
                    case "WatertowerTypeArray":
                    case "WatertowerLocationArray":
                    case "WatertowerRotationArray":
                    case "WatertowerWaterlevelArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Watertowers == null)
                        {
                            Watertowers = new Watertower[arrayProperty.Items.Length];
                            for (int i = 0; i < Watertowers.Length; i++)
                                Watertowers[i] = new Watertower();
                        }

                        switch (arrayProperty.Name)
                        {
                            case "WatertowerTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    WatertowerType type = (WatertowerType)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Watertowers[i].Type = type;
                                }
                                break;
                            case "WatertowerLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Watertowers[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "WatertowerRotationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty rotation = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Watertowers[i].Rotation = new Rotation(rotation.X, rotation.Y, rotation.Z);
                                }
                                break;
                            case "WatertowerWaterlevelArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Watertowers[i].WaterLevel = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;

                        }
                        break;
                    }
                    case "SandhouseTypeArray":
                    case "SandhouseLocationArray":
                    case "SandhouseRotationArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Sandhouses == null)
                        {
                            Sandhouses = new Sandhouse[arrayProperty.Items.Length];
                            for (int i = 0; i < Sandhouses.Length; i++)
                                Sandhouses[i] = new Sandhouse();
                        }

                        switch (arrayProperty.Name)
                        {
                            case "SandhouseTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    SandhouseType type = (SandhouseType)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Sandhouses[i].Type = type;
                                }
                                break;
                            case "SandhouseLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Sandhouses[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "SandhouseRotationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty rotation = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Sandhouses[i].Rotation = new Rotation(rotation.X, rotation.Y, rotation.Z);
                                }
                                break;

                        }
                        break;
                    }
                    case "IndustryTypeArray":
                    case "IndustryLocationArray":
                    case "IndustryRotationArray":
                    case "IndustryStorageEduct1Array":
                    case "IndustryStorageEduct2Array":
                    case "IndustryStorageEduct3Array":
                    case "IndustryStorageEduct4Array":
                    case "IndustryStorageProduct1Array":
                    case "IndustryStorageProduct2Array":
                    case "IndustryStorageProduct3Array":
                    case "IndustryStorageProduct4Array":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Industries == null)
                        {
                            Industries = new Industry[arrayProperty.Items.Length];
                            for (int i = 0; i < Industries.Length; i++)
                                Industries[i] = new Industry();
                        }

                        switch (arrayProperty.Name)
                        {
                            case "IndustryTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    IndustryType type = (IndustryType)((UEIntProperty) arrayProperty.Items[i]).Value;

                                    Industries[i].Type = type;
                                }
                                break;
                            case "IndustryLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Industries[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "IndustryRotationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty rotation = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Industries[i].Rotation = new Rotation(rotation.X, rotation.Y, rotation.Z);
                                }
                                break;
                            case "IndustryStorageEduct1":
                            case "IndustryStorageEduct2":
                            case "IndustryStorageEduct3":
                            case "IndustryStorageEduct4":
                            {
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    if (Industries[i].EductsStored == null)
                                    {
                                        Industries[i].EductsStored = new int[4];
                                    }

                                    switch (arrayProperty.Name)
                                    {
                                        case "IndustryStorageEduct1":
                                            Industries[i].EductsStored[0] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                        case "IndustryStorageEduct2":
                                            Industries[i].EductsStored[1] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                        case "IndustryStorageEduct3":
                                            Industries[i].EductsStored[2] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                        case "IndustryStorageEduct4":
                                            Industries[i].EductsStored[3] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                    }
                                }
                                break;
                            }
                            case "IndustryStorageProduct1":
                            case "IndustryStorageProduct2":
                            case "IndustryStorageProduct3":
                            case "IndustryStorageProduct4":
                            {
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    if (Industries[i].ProductsStored == null)
                                    {
                                        Industries[i].ProductsStored = new int[4];
                                    }

                                    switch (arrayProperty.Name)
                                    {
                                        case "IndustryStorageProduct1":
                                            Industries[i].ProductsStored[0] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                        case "IndustryStorageProduct2":
                                            Industries[i].ProductsStored[1] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                        case "IndustryStorageProduct3":
                                            Industries[i].ProductsStored[2] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                        case "IndustryStorageProduct4":
                                            Industries[i].ProductsStored[3] = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                            break;
                                    }
                                }
                                break;
                            }

                        }
                        break;
                    }
                    case "FrameTypeArray":
                    case "FrameLocationArray":
                    case "FrameRotationArray":
                    case "FrameNumberArray":
                    case "FrameNameArray":
                    case "SmokestackTypeArray":
                    case "HeadlightTypeArray":
                    case "BoilerFuelAmountArray":
                    case "BoilerFireTempArray":
                    case "BoilerWaterTempArray":
                    case "BoilerWaterLeverArray":
                    case "BoilerPressureArray":
                    case "HeadlightFrontStateArray":
                    case "HeadlightRearStateArray":
                    case "CouplerFrontStateArray":
                    case "CouplerRearStateArray":
                    case "TenderFuelAmountArray":
                    case "TenderWaterAmountArray":
                    case "CompressorAirPressureArray":
                    case "MarkerLightsFrontRightStateArray":
                    case "MarkerLightsFrontLeftStateArray":
                    case "MarkerLightsRearRightStateArray":
                    case "MarkerLightsRearLeftStateArray":
                    case "FreightTypeArray":
                    case "FreightAmountArray":
                    case "RegulatorValueArray":
                    case "BrakeValueArray":
                    case "GeneratorValveValueArray":
                    case "CompressorValveValueArray":
                    case "ReverserValueArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        if (Vehicles == null)
                        {
                            Vehicles = new Vehicle[arrayProperty.Items.Length];
                            for (int i = 0; i < Vehicles.Length; i++)
                                Vehicles[i] = new Vehicle();
                        }

                        switch (arrayProperty.Name)
                        {
                            case "FrameTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    string type = ((UEStringProperty) arrayProperty.Items[i]).Value;

                                    Vehicles[i].Type = type;
                                }
                                break;
                            case "FrameLocationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Vehicles[i].Location = new Location(location.X, location.Y, location.Z);
                                }
                                break;
                            case "FrameRotationArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    UEVectorStructProperty rotation = (UEVectorStructProperty) arrayProperty.Items[i];

                                    Vehicles[i].Rotation = new Rotation(rotation.X, rotation.Y, rotation.Z);
                                }
                                break;
                            case "FrameNumberArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].Number = ((UETextProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "FrameNameArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].Name = ((UETextProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "SmokestackTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].SmokestackType = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "HeadlightTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].HeadlightType = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "BoilerFuelAmountArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].BoilerFuelAmount = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "BoilerFireTempArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].BoilerFireTemp = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "BoilerWaterTempArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].BoilerWaterTemp = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "BoilerWaterLeverArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].BoilerWaterLevel = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "BoilerPressureArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].BoilerPressure = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "HeadlightFrontStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].HeadlightFront = ((UEBoolProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "HeadlightRearStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].HeadlightRear = ((UEBoolProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "CouplerFrontStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].CouplerFrontState = ((UEBoolProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "CouplerRearStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].CouplerRearState = ((UEBoolProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "TenderFuelAmountArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].TenderFuelAmount = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "TenderWaterAmountArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].TenderWaterAmount = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "CompressorAirPressureArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].CompressorAirPressure = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "MarkerLightsFrontRightStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].MarkerLightsFrontRightState = (MarkerLightState)((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "MarkerLightsFrontLeftStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].MarkerLightsFrontLeftState = (MarkerLightState)((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "MarkerLightsRearRightStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].MarkerLightsRearRightState = (MarkerLightState)((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "MarkerLightsRearLeftStateArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].MarkerLightsRearLeftState = (MarkerLightState)((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "FreightTypeArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].FreightType = ((UEStringProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "FreightAmountArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].FreightAmount = ((UEIntProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "RegulatorValueArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].RegulatorValue = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "BrakeValueArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].BrakeValue = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "GeneratorValveValueArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].GeneratorValveValue = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "CompressorValveValueArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].CompressorValveValue = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                            case "ReverserValueArray":
                                for (int i = 0; i < arrayProperty.Items.Length; i++)
                                {
                                    Vehicles[i].ReverserValue = ((UEFloatProperty)arrayProperty.Items[i]).Value;
                                }
                                break;
                        }

                        break;
                    }
                    case "RemovedVegetationAssetsArray":
                    {
                        UEArrayProperty arrayProperty = (UEArrayProperty)property;
                        RemovedVegetationAssets = new Location[arrayProperty.Items.Length];

                        for (int i = 0; i < RemovedVegetationAssets.Length; i++)
                        {
                            UEVectorStructProperty location = (UEVectorStructProperty) arrayProperty.Items[i];

                            RemovedVegetationAssets[i] = new Location(location.X, location.Y, location.Z);
                        }
                        break;
                    }
                }
            });

            for(int i = 0; i < Splines.Length; i++)
            {
                Splines[i].BuildSegments(splineControlPoints, splineVisibilityPoints, i);
            }

            StaticObjects = new StaticObject[Switches.Length + Watertowers.Length + Sandhouses.Length + Industries.Length];
            Switches.CopyTo(StaticObjects, 0);
            Watertowers.CopyTo(StaticObjects, Switches.Length);
            Sandhouses.CopyTo(StaticObjects, Switches.Length + Watertowers.Length);
            Industries.CopyTo(StaticObjects, Switches.Length + Watertowers.Length + Sandhouses.Length);
        }
    }
}
