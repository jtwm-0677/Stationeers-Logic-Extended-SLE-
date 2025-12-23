using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SLE.Core
{
    /// <summary>
    /// Registry of all custom LogicTypes with metadata.
    /// </summary>
    public static class LogicTypeRegistry
    {
        private static readonly Dictionary<ushort, LogicTypeInfo> _byValue = new Dictionary<ushort, LogicTypeInfo>();
        private static readonly Dictionary<string, LogicTypeInfo> _byName = new Dictionary<string, LogicTypeInfo>();

        public static int Count => _byValue.Count;

        public static IEnumerable<LogicTypeInfo> All => _byValue.Values;

        public static void Initialize()
        {
            _byValue.Clear();
            _byName.Clear();

            // Contact Selection & Filtering
            Register(new LogicTypeInfo(
                "ContactIndex", "ContactIndex",
                "Select contact by index (0-based)",
                (ushort)SLELogicType.ContactIndex,
                "int", "read-write", "ContactSelection"));

            Register(new LogicTypeInfo(
                "ContactCount", "ContactCount",
                "Total visible contacts",
                (ushort)SLELogicType.ContactCount,
                "int", "read", "ContactSelection"));

            Register(new LogicTypeInfo(
                "FilterMode", "FilterMode",
                "Filter type: 0=All, 1=ShuttleType, 2=Resolved, 3=Unresolved, 4=Contacted, 5=NotContacted",
                (ushort)SLELogicType.FilterMode,
                "int", "read-write", "ContactSelection"));

            Register(new LogicTypeInfo(
                "FilterValue", "FilterValue",
                "Filter parameter value (e.g., ShuttleType when FilterMode=1)",
                (ushort)SLELogicType.FilterValue,
                "int", "read-write", "ContactSelection"));

            Register(new LogicTypeInfo(
                "FilteredCount", "FilteredCount",
                "Count of contacts matching current filter",
                (ushort)SLELogicType.FilteredCount,
                "int", "read", "ContactSelection"));

            // Selected Contact Properties
            Register(new LogicTypeInfo(
                "ContactShuttleType", "ContactShuttleType",
                "ShuttleType enum: 0=None, 1=Small, 2=SmallGas, 3=Medium, 4=MediumGas, 5=Large, 6=LargeGas, 7=MediumPlane, 8=LargePlane",
                (ushort)SLELogicType.ContactShuttleType,
                "int", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactLifetime", "ContactLifetime",
                "Seconds until contact leaves range",
                (ushort)SLELogicType.ContactLifetime,
                "float", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactDegreeOffset", "ContactDegreeOffset",
                "Alignment angle in degrees (lower = better aligned)",
                (ushort)SLELogicType.ContactDegreeOffset,
                "float", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactResolved", "ContactResolved",
                "1 if contact is resolved, 0 if not",
                (ushort)SLELogicType.ContactResolved,
                "bool", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactContacted", "ContactContacted",
                "1 if trader has been contacted, 0 if not",
                (ushort)SLELogicType.ContactContacted,
                "bool", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactResolutionProgress", "ContactResolutionProgress",
                "Resolution progress 0.0-1.0",
                (ushort)SLELogicType.ContactResolutionProgress,
                "float", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactMinWattsResolve", "ContactMinWattsResolve",
                "Minimum watts required to resolve this contact",
                (ushort)SLELogicType.ContactMinWattsResolve,
                "float", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactMinWattsContact", "ContactMinWattsContact",
                "Minimum watts required to contact this trader",
                (ushort)SLELogicType.ContactMinWattsContact,
                "float", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactSecondsToContact", "ContactSecondsToContact",
                "Seconds required to establish contact",
                (ushort)SLELogicType.ContactSecondsToContact,
                "float", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactTraderHash", "ContactTraderHash",
                "Trader type hash (same as game's ContactTypeId)",
                (ushort)SLELogicType.ContactTraderHash,
                "int", "read", "ContactProperties"));

            Register(new LogicTypeInfo(
                "ContactReferenceId", "ContactReferenceId",
                "Unique reference ID of contact",
                (ushort)SLELogicType.ContactReferenceId,
                "long", "read", "ContactProperties"));

            // Note: Pad sizes use vanilla SizeX/SizeZ LogicTypes

            // Dish State
            Register(new LogicTypeInfo(
                "DishWattageOnContact", "DishWattageOnContact",
                "Actual watts reaching selected contact",
                (ushort)SLELogicType.DishWattageOnContact,
                "float", "read", "DishState"));

            Register(new LogicTypeInfo(
                "DishIsInterrogating", "DishIsInterrogating",
                "1 if dish is currently interrogating a contact",
                (ushort)SLELogicType.DishIsInterrogating,
                "bool", "read", "DishState"));

            Register(new LogicTypeInfo(
                "DishInterrogatingId", "DishInterrogatingId",
                "ReferenceId of contact being interrogated, 0 if none",
                (ushort)SLELogicType.DishInterrogatingId,
                "long", "read", "DishState"));

            // Centrifuge
            Register(new LogicTypeInfo(
                "CentrifugeProcessing", "CentrifugeProcessing",
                "Processing progress 0-100%. Works on Centrifuge and CombustionCentrifuge",
                (ushort)SLELogicType.CentrifugeProcessing,
                "int", "read", "Centrifuge"));

            // DaylightSensor / Realtime Data
            Register(new LogicTypeInfo(
                "TimeOfDay", "TimeOfDay",
                "Time of day 0-1 (0=sunrise, 0.25=noon, 0.5=sunset, 0.75=midnight)",
                (ushort)SLELogicType.TimeOfDay,
                "float", "read", "RealtimeData"));

            Register(new LogicTypeInfo(
                "IsEclipse", "IsEclipse",
                "1 if eclipse is occurring, 0 if not",
                (ushort)SLELogicType.IsEclipse,
                "bool", "read", "RealtimeData"));

            Register(new LogicTypeInfo(
                "EclipseRatio", "EclipseRatio",
                "Eclipse intensity 0.0-1.0 (0=no eclipse, 1=full eclipse)",
                (ushort)SLELogicType.EclipseRatio,
                "float", "read", "RealtimeData"));

            Register(new LogicTypeInfo(
                "DaysPast", "DaysPast",
                "Number of days since world creation",
                (ushort)SLELogicType.DaysPast,
                "int", "read", "RealtimeData"));

            Register(new LogicTypeInfo(
                "DayLengthSeconds", "DayLengthSeconds",
                "Length of a day in seconds",
                (ushort)SLELogicType.DayLengthSeconds,
                "int", "read", "RealtimeData"));

            Register(new LogicTypeInfo(
                "Latitude", "Latitude",
                "World latitude in degrees",
                (ushort)SLELogicType.Latitude,
                "float", "read", "RealtimeData"));

            Register(new LogicTypeInfo(
                "Longitude", "Longitude",
                "World longitude in degrees",
                (ushort)SLELogicType.Longitude,
                "float", "read", "RealtimeData"));

            Register(new LogicTypeInfo(
                "WeatherSolarRatio", "WeatherSolarRatio",
                "Weather solar ratio 0-1 (1=full sun, lower during storms)",
                (ushort)SLELogicType.WeatherSolarRatio,
                "float", "read", "RealtimeData"));

            // WindTurbineGenerator
            Register(new LogicTypeInfo(
                "WindSpeed", "WindSpeed",
                "Current global wind strength 0-1",
                (ushort)SLELogicType.WindSpeed,
                "float", "read", "WindTurbine"));

            Register(new LogicTypeInfo(
                "MaxPower", "MaxPower",
                "Current max power output (storm-aware)",
                (ushort)SLELogicType.MaxPower,
                "float", "read", "WindTurbine"));

            Register(new LogicTypeInfo(
                "TurbineSpeed", "TurbineSpeed",
                "Current turbine blade rotation speed 0-1",
                (ushort)SLELogicType.TurbineSpeed,
                "float", "read", "WindTurbine"));

            Register(new LogicTypeInfo(
                "AtmosphericPressure", "AtmosphericPressure",
                "Clamped atmospheric pressure in kPa",
                (ushort)SLELogicType.AtmosphericPressure,
                "float", "read", "WindTurbine"));

            // SolidFuelGenerator
            Register(new LogicTypeInfo(
                "FuelTicks", "FuelTicks",
                "Fuel buffer remaining in ticks",
                (ushort)SLELogicType.FuelTicks,
                "int", "read", "SolidFuelGenerator"));

            // WeatherStation
            Register(new LogicTypeInfo(
                "WeatherWindStrength", "WeatherWindStrength",
                "Current weather event wind strength 0-1",
                (ushort)SLELogicType.WeatherWindStrength,
                "float", "read", "WeatherStation"));

            Register(new LogicTypeInfo(
                "DaysSinceLastWeatherEvent", "DaysSinceLastWeatherEvent",
                "Days since last weather event ended",
                (ushort)SLELogicType.DaysSinceLastWeatherEvent,
                "float", "read", "WeatherStation"));

            // DeepMiner
            Register(new LogicTypeInfo(
                "MiningProgress", "MiningProgress",
                "Mining cycle progress 0-100%",
                (ushort)SLELogicType.MiningProgress,
                "float", "read", "DeepMiner"));

            Register(new LogicTypeInfo(
                "CurrentOreHash", "CurrentOreHash",
                "Hash of current ore type in export slot",
                (ushort)SLELogicType.CurrentOreHash,
                "int", "read", "DeepMiner"));

            // HydroponicsTrayDevice
            Register(new LogicTypeInfo(
                "LightExposure", "LightExposure",
                "Current light exposure level (grow light + solar)",
                (ushort)SLELogicType.LightExposure,
                "float", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "IsLitByGrowLight", "IsLitByGrowLight",
                "1 if lit by a powered grow light, 0 if not",
                (ushort)SLELogicType.IsLitByGrowLight,
                "bool", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "WaterMoles", "WaterMoles",
                "Water amount in internal atmosphere (moles)",
                (ushort)SLELogicType.WaterMoles,
                "float", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "PlantIsFertilized", "PlantIsFertilized",
                "1 if plant has been fertilized, 0 if not",
                (ushort)SLELogicType.PlantIsFertilized,
                "bool", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "PlantGrowthEfficiency", "PlantGrowthEfficiency",
                "Plant overall growth efficiency 0-100%",
                (ushort)SLELogicType.PlantGrowthEfficiency,
                "int", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "BreathingEfficiency", "BreathingEfficiency",
                "Plant breathing/gas efficiency 0-100%",
                (ushort)SLELogicType.BreathingEfficiency,
                "int", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "TemperatureEfficiency", "TemperatureEfficiency",
                "Plant temperature efficiency 0-100%",
                (ushort)SLELogicType.TemperatureEfficiency,
                "int", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "PlantLightEfficiency", "PlantLightEfficiency",
                "Plant light efficiency 0-100%",
                (ushort)SLELogicType.PlantLightEfficiency,
                "int", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "PlantPressureEfficiency", "PlantPressureEfficiency",
                "Plant pressure efficiency 0-100%",
                (ushort)SLELogicType.PlantPressureEfficiency,
                "int", "read", "HydroponicsDevice"));

            Register(new LogicTypeInfo(
                "HydrationEfficiency", "HydrationEfficiency",
                "Plant hydration/water efficiency 0-100%",
                (ushort)SLELogicType.HydrationEfficiency,
                "int", "read", "HydroponicsDevice"));

            // GasFuelGenerator
            Register(new LogicTypeInfo(
                "CombustionEnergy", "CombustionEnergy",
                "Combustion energy produced (before 17% efficiency conversion)",
                (ushort)SLELogicType.CombustionEnergy,
                "float", "read", "GasFuelGenerator"));

            Register(new LogicTypeInfo(
                "IsValidAtmosphere", "IsValidAtmosphere",
                "1 if atmosphere meets pressure/temp requirements, 0 if not",
                (ushort)SLELogicType.IsValidAtmosphere,
                "bool", "read", "GasFuelGenerator"));

            Register(new LogicTypeInfo(
                "DoShutdown", "DoShutdown",
                "1 if conditions will trigger shutdown, 0 if not",
                (ushort)SLELogicType.DoShutdown,
                "bool", "read", "GasFuelGenerator"));

            Register(new LogicTypeInfo(
                "MinTemperature", "MinTemperature",
                "Minimum operating temperature in Kelvin",
                (ushort)SLELogicType.MinTemperature,
                "float", "read", "GasFuelGenerator"));

            Register(new LogicTypeInfo(
                "MaxTemperature", "MaxTemperature",
                "Maximum operating temperature in Kelvin",
                (ushort)SLELogicType.MaxTemperature,
                "float", "read", "GasFuelGenerator"));

            Register(new LogicTypeInfo(
                "MinPressure", "MinPressure",
                "Minimum operating pressure in Pa",
                (ushort)SLELogicType.MinPressure,
                "float", "read", "GasFuelGenerator"));

            // Battery
            Register(new LogicTypeInfo(
                "PowerDelta", "PowerDelta",
                "Power deficit (PowerStored - PowerMaximum). Negative when not full",
                (ushort)SLELogicType.PowerDelta,
                "float", "read", "Battery"));

            Register(new LogicTypeInfo(
                "BatteryIsSubmerged", "BatteryIsSubmerged",
                "1 if battery is submerged in liquid (short circuit risk), 0 if not",
                (ushort)SLELogicType.BatteryIsSubmerged,
                "bool", "read", "Battery"));

            Register(new LogicTypeInfo(
                "InputSubmergedTicks", "InputSubmergedTicks",
                "Number of ticks input connection has been submerged",
                (ushort)SLELogicType.InputSubmergedTicks,
                "int", "read", "Battery"));

            Register(new LogicTypeInfo(
                "OutputSubmergedTicks", "OutputSubmergedTicks",
                "Number of ticks output connection has been submerged",
                (ushort)SLELogicType.OutputSubmergedTicks,
                "int", "read", "Battery"));

            Register(new LogicTypeInfo(
                "BatteryIsEmpty", "BatteryIsEmpty",
                "1 if battery is empty (Mode == 0), 0 if not",
                (ushort)SLELogicType.BatteryIsEmpty,
                "bool", "read", "Battery"));

            Register(new LogicTypeInfo(
                "BatteryIsCharged", "BatteryIsCharged",
                "1 if battery is fully charged (Mode == 6), 0 if not",
                (ushort)SLELogicType.BatteryIsCharged,
                "bool", "read", "Battery"));

            // SolarPanel
            Register(new LogicTypeInfo(
                "SolarVisibility", "SolarVisibility",
                "Sun visibility factor 0-1 (affected by obstructions)",
                (ushort)SLELogicType.SolarVisibility,
                "float", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarDamageRatio", "SolarDamageRatio",
                "Damage ratio 0-1 (0=undamaged, 1=fully damaged)",
                (ushort)SLELogicType.SolarDamageRatio,
                "float", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarDamageTotal", "SolarDamageTotal",
                "Total damage points accumulated",
                (ushort)SLELogicType.SolarDamageTotal,
                "float", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarHealth", "SolarHealth",
                "Current health as percentage 0-100",
                (ushort)SLELogicType.SolarHealth,
                "int", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarEfficiency", "SolarEfficiency",
                "Current efficiency as percentage 0-100 (includes damage)",
                (ushort)SLELogicType.SolarEfficiency,
                "int", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarIsOperable", "SolarIsOperable",
                "1 if panel is operable, 0 if not",
                (ushort)SLELogicType.SolarIsOperable,
                "bool", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarIsBroken", "SolarIsBroken",
                "1 if panel is broken, 0 if not",
                (ushort)SLELogicType.SolarIsBroken,
                "bool", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarMovementSpeedH", "SolarMovementSpeedH",
                "Horizontal rotation speed in degrees/sec",
                (ushort)SLELogicType.SolarMovementSpeedH,
                "float", "read", "SolarPanel"));

            Register(new LogicTypeInfo(
                "SolarMovementSpeedV", "SolarMovementSpeedV",
                "Vertical rotation speed in degrees/sec",
                (ushort)SLELogicType.SolarMovementSpeedV,
                "float", "read", "SolarPanel"));

            // H2Combustor
            Register(new LogicTypeInfo(
                "H2CombustorProcessedMoles", "H2CombustorProcessedMoles",
                "Moles processed this atmospheric tick",
                (ushort)SLELogicType.H2CombustorProcessedMoles,
                "float", "read", "H2Combustor"));

            Register(new LogicTypeInfo(
                "H2CombustorUsedPower", "H2CombustorUsedPower",
                "Power consumed this tick (3600W active, 50W idle)",
                (ushort)SLELogicType.H2CombustorUsedPower,
                "float", "read", "H2Combustor"));

            Register(new LogicTypeInfo(
                "H2CombustorIsOperable", "H2CombustorIsOperable",
                "1 if device is operable (connections valid), 0 if not",
                (ushort)SLELogicType.H2CombustorIsOperable,
                "bool", "read", "H2Combustor"));

            Register(new LogicTypeInfo(
                "H2CombustorCodeError", "H2CombustorCodeError",
                "IC chip error state code",
                (ushort)SLELogicType.H2CombustorCodeError,
                "int", "read", "H2Combustor"));

            // Electrolyzer
            Register(new LogicTypeInfo(
                "ElectrolyzerProcessedMoles", "ElectrolyzerProcessedMoles",
                "Moles processed this atmospheric tick",
                (ushort)SLELogicType.ElectrolyzerProcessedMoles,
                "float", "read", "Electrolyzer"));

            Register(new LogicTypeInfo(
                "ElectrolyzerUsedPower", "ElectrolyzerUsedPower",
                "Power consumed this tick (3600W active, 50W idle)",
                (ushort)SLELogicType.ElectrolyzerUsedPower,
                "float", "read", "Electrolyzer"));

            Register(new LogicTypeInfo(
                "ElectrolyzerIsOperable", "ElectrolyzerIsOperable",
                "1 if device is operable (connections valid), 0 if not",
                (ushort)SLELogicType.ElectrolyzerIsOperable,
                "bool", "read", "Electrolyzer"));

            Register(new LogicTypeInfo(
                "ElectrolyzerCodeError", "ElectrolyzerCodeError",
                "IC chip error state code",
                (ushort)SLELogicType.ElectrolyzerCodeError,
                "int", "read", "Electrolyzer"));

            // Harvester
            Register(new LogicTypeInfo(
                "HasTray", "HasTray",
                "1 if harvester is positioned over a hydroponics tray, 0 if not",
                (ushort)SLELogicType.HasTray,
                "bool", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "IsHarvesting", "IsHarvesting",
                "1 if currently performing harvest operation, 0 if not",
                (ushort)SLELogicType.IsHarvesting,
                "bool", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "IsPlanting", "IsPlanting",
                "1 if currently performing plant operation, 0 if not",
                (ushort)SLELogicType.IsPlanting,
                "bool", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "ArmState", "ArmState",
                "Arm state: 0=Idle, 1=Planting, 2=Harvesting",
                (ushort)SLELogicType.ArmState,
                "int", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "HasImportPlant", "HasImportPlant",
                "1 if plant/seed is ready in import slot, 0 if not",
                (ushort)SLELogicType.HasImportPlant,
                "bool", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "ImportPlantHash", "ImportPlantHash",
                "PrefabHash of plant/seed in import slot",
                (ushort)SLELogicType.ImportPlantHash,
                "int", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "HasFertilizer", "HasFertilizer",
                "1 if fertilizer is in tray's fertilizer slot, 0 if not",
                (ushort)SLELogicType.HasFertilizer,
                "bool", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "FertilizerCycles", "FertilizerCycles",
                "Remaining fertilizer cycles",
                (ushort)SLELogicType.FertilizerCycles,
                "float", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "FertilizerHarvestBoost", "FertilizerHarvestBoost",
                "Fertilizer harvest yield multiplier",
                (ushort)SLELogicType.FertilizerHarvestBoost,
                "float", "read", "Harvester"));

            Register(new LogicTypeInfo(
                "FertilizerGrowthSpeed", "FertilizerGrowthSpeed",
                "Fertilizer growth speed multiplier",
                (ushort)SLELogicType.FertilizerGrowthSpeed,
                "float", "read", "Harvester"));

            // PipeAnalyzer - Partial Pressures
            Register(new LogicTypeInfo(
                "PartialPressureO2", "PartialPressureO2",
                "Partial pressure of Oxygen in kPa",
                (ushort)SLELogicType.PartialPressureO2,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressureCO2", "PartialPressureCO2",
                "Partial pressure of Carbon Dioxide in kPa",
                (ushort)SLELogicType.PartialPressureCO2,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressureN2", "PartialPressureN2",
                "Partial pressure of Nitrogen in kPa",
                (ushort)SLELogicType.PartialPressureN2,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressureVolatiles", "PartialPressureVolatiles",
                "Partial pressure of Volatiles in kPa",
                (ushort)SLELogicType.PartialPressureVolatiles,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressureN2O", "PartialPressureN2O",
                "Partial pressure of Nitrous Oxide in kPa",
                (ushort)SLELogicType.PartialPressureN2O,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressurePollutant", "PartialPressurePollutant",
                "Partial pressure of Pollutant in kPa",
                (ushort)SLELogicType.PartialPressurePollutant,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressureH2", "PartialPressureH2",
                "Partial pressure of Hydrogen in kPa",
                (ushort)SLELogicType.PartialPressureH2,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressureSteam", "PartialPressureSteam",
                "Partial pressure of Steam/Water in kPa",
                (ushort)SLELogicType.PartialPressureSteam,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PartialPressureToxins", "PartialPressureToxins",
                "Partial pressure of Toxins in kPa",
                (ushort)SLELogicType.PartialPressureToxins,
                "float", "read", "PipeAnalyzer"));

            // PipeAnalyzer - Gas/Liquid Separation
            Register(new LogicTypeInfo(
                "TotalMolesGasses", "TotalMolesGasses",
                "Total moles of gases only (excludes liquids)",
                (ushort)SLELogicType.TotalMolesGasses,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "TotalMolesLiquids", "TotalMolesLiquids",
                "Total moles of liquids only (excludes gases)",
                (ushort)SLELogicType.TotalMolesLiquids,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "LiquidVolumeRatio", "LiquidVolumeRatio",
                "Ratio of liquid volume to total volume 0-1",
                (ushort)SLELogicType.LiquidVolumeRatio,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PressureGasses", "PressureGasses",
                "Gas-only pressure in kPa (excludes liquid pressure)",
                (ushort)SLELogicType.PressureGasses,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "LiquidPressureOffset", "LiquidPressureOffset",
                "Liquid pressure offset in kPa",
                (ushort)SLELogicType.LiquidPressureOffset,
                "float", "read", "PipeAnalyzer"));

            // PipeAnalyzer - Combustion & Reactions
            Register(new LogicTypeInfo(
                "PipeCombustionEnergy", "PipeCombustionEnergy",
                "Energy from combustion reactions in joules",
                (ushort)SLELogicType.PipeCombustionEnergy,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "CleanBurnRate", "CleanBurnRate",
                "Clean burn rate/efficiency 0-1",
                (ushort)SLELogicType.CleanBurnRate,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "Inflamed", "Inflamed",
                "1 if atmosphere is inflamed/on fire, 0 if not",
                (ushort)SLELogicType.Inflamed,
                "bool", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "Suppressed", "Suppressed",
                "Fire suppression ticks remaining",
                (ushort)SLELogicType.Suppressed,
                "int", "read", "PipeAnalyzer"));

            // PipeAnalyzer - Network & State
            Register(new LogicTypeInfo(
                "EnergyConvected", "EnergyConvected",
                "Heat energy being convected in watts",
                (ushort)SLELogicType.EnergyConvected,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "EnergyRadiated", "EnergyRadiated",
                "Heat energy being radiated in watts",
                (ushort)SLELogicType.EnergyRadiated,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "IsAboveArmstrong", "IsAboveArmstrong",
                "1 if pressure is above Armstrong limit (6.3kPa), 0 if not",
                (ushort)SLELogicType.IsAboveArmstrong,
                "bool", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "Condensation", "Condensation",
                "1 if condensation is occurring, 0 if not",
                (ushort)SLELogicType.Condensation,
                "bool", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "NetworkContentType", "NetworkContentType",
                "Network content type: 0=Gas, 1=Liquid",
                (ushort)SLELogicType.NetworkContentType,
                "int", "read", "PipeAnalyzer"));

            // PipeAnalyzer - Stress Monitoring
            Register(new LogicTypeInfo(
                "PipeMaxPressure", "PipeMaxPressure",
                "Maximum burst pressure for connected pipe type in kPa",
                (ushort)SLELogicType.PipeMaxPressure,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PipeStressRatio", "PipeStressRatio",
                "Current pressure / max pressure ratio (0-1+, >=0.8 = stressed)",
                (ushort)SLELogicType.PipeStressRatio,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PipeIsStressed", "PipeIsStressed",
                "1 if pipe is stressed (high pressure OR liquid in gas pipe), 0 if not",
                (ushort)SLELogicType.PipeIsStressed,
                "bool", "read", "PipeAnalyzer"));

            // PipeAnalyzer - Damage Monitoring
            Register(new LogicTypeInfo(
                "PipeDamageRatio", "PipeDamageRatio",
                "Pipe damage ratio 0-1 (1 = burst/destroyed)",
                (ushort)SLELogicType.PipeDamageRatio,
                "float", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PipeIsBurst", "PipeIsBurst",
                "1 if pipe has burst, 0 if intact",
                (ushort)SLELogicType.PipeIsBurst,
                "bool", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "PipeDamageType", "PipeDamageType",
                "Damage type bitmask: 1=Pressure, 2=Liquid, 4=Frozen",
                (ushort)SLELogicType.PipeDamageType,
                "int", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "NetworkHasFault", "NetworkHasFault",
                "1 if any pipe in network has a fault, 0 if not",
                (ushort)SLELogicType.NetworkHasFault,
                "bool", "read", "PipeAnalyzer"));

            Register(new LogicTypeInfo(
                "NetworkWorstDamage", "NetworkWorstDamage",
                "Highest damage ratio of any pipe in the network 0-1",
                (ushort)SLELogicType.NetworkWorstDamage,
                "float", "read", "PipeAnalyzer"));

            // FurnaceBase - All Furnaces
            Register(new LogicTypeInfo(
                "FurnaceTemperature", "FurnaceTemperature",
                "Internal furnace temperature in Kelvin",
                (ushort)SLELogicType.FurnaceTemperature,
                "float", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnacePressure", "FurnacePressure",
                "Internal furnace pressure in kPa",
                (ushort)SLELogicType.FurnacePressure,
                "float", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceTotalMoles", "FurnaceTotalMoles",
                "Total moles of gas inside furnace",
                (ushort)SLELogicType.FurnaceTotalMoles,
                "float", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceInflamed", "FurnaceInflamed",
                "1 if furnace is actively burning/smelting, 0 if not",
                (ushort)SLELogicType.FurnaceInflamed,
                "bool", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceMode", "FurnaceMode",
                "1 if valid recipe detected, 0 if not",
                (ushort)SLELogicType.FurnaceMode,
                "bool", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "ReagentQuantity", "ReagentQuantity",
                "Total quantity of minerals loaded in furnace",
                (ushort)SLELogicType.ReagentQuantity,
                "float", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceOverpressure", "FurnaceOverpressure",
                "1 if furnace is in dangerous overpressure state, 0 if not",
                (ushort)SLELogicType.FurnaceOverpressure,
                "bool", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "CurrentRecipeEnergy", "CurrentRecipeEnergy",
                "Energy required to complete current recipe",
                (ushort)SLELogicType.CurrentRecipeEnergy,
                "float", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceStressed", "FurnaceStressed",
                "1 if pressure is at warning level (66% of max), 0 if not",
                (ushort)SLELogicType.FurnaceStressed,
                "bool", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceHasBlown", "FurnaceHasBlown",
                "1 if furnace has already exploded, 0 if not",
                (ushort)SLELogicType.FurnaceHasBlown,
                "bool", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceMaxPressure", "FurnaceMaxPressure",
                "Maximum pressure differential before explosion (60795 kPa)",
                (ushort)SLELogicType.FurnaceMaxPressure,
                "float", "read", "Furnace"));

            Register(new LogicTypeInfo(
                "FurnaceVolume", "FurnaceVolume",
                "Internal furnace volume in litres",
                (ushort)SLELogicType.FurnaceVolume,
                "float", "read", "Furnace"));

            // AdvancedFurnace Specific
            Register(new LogicTypeInfo(
                "MinSettingInput", "MinSettingInput",
                "Minimum input setting value",
                (ushort)SLELogicType.MinSettingInput,
                "float", "read", "AdvancedFurnace"));

            Register(new LogicTypeInfo(
                "MaxSettingInput", "MaxSettingInput",
                "Maximum input setting value",
                (ushort)SLELogicType.MaxSettingInput,
                "float", "read", "AdvancedFurnace"));

            Register(new LogicTypeInfo(
                "MinSettingOutput", "MinSettingOutput",
                "Minimum output setting value",
                (ushort)SLELogicType.MinSettingOutput,
                "float", "read", "AdvancedFurnace"));

            Register(new LogicTypeInfo(
                "MaxSettingOutput", "MaxSettingOutput",
                "Maximum output setting value",
                (ushort)SLELogicType.MaxSettingOutput,
                "float", "read", "AdvancedFurnace"));

            // ArcFurnace Specific
            Register(new LogicTypeInfo(
                "ArcFurnaceActivate", "ArcFurnaceActivate",
                "Arc furnace activate state: 0=idle, 1=smelting",
                (ushort)SLELogicType.ArcFurnaceActivate,
                "int", "read", "ArcFurnace"));

            Register(new LogicTypeInfo(
                "ImportStackSize", "ImportStackSize",
                "Quantity of items in import slot",
                (ushort)SLELogicType.ImportStackSize,
                "int", "read", "ArcFurnace"));

            Register(new LogicTypeInfo(
                "SmeltingPower", "SmeltingPower",
                "Power being consumed for smelting this tick",
                (ushort)SLELogicType.SmeltingPower,
                "float", "read", "ArcFurnace"));

            Register(new LogicTypeInfo(
                "ArcFurnaceIsSmelting", "ArcFurnaceIsSmelting",
                "1 if arc furnace is actively smelting, 0 if not",
                (ushort)SLELogicType.ArcFurnaceIsSmelting,
                "bool", "read", "ArcFurnace"));

            // FiltrationMachineBase - Machine State
            Register(new LogicTypeInfo(
                "FilterSlotIndex", "FilterSlotIndex",
                "Select filter slot by index (0-based)",
                (ushort)SLELogicType.FilterSlotIndex,
                "int", "read-write", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterSlotCount", "FilterSlotCount",
                "Total number of gas filter slots",
                (ushort)SLELogicType.FilterSlotCount,
                "int", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "HasEmptyFilter", "HasEmptyFilter",
                "1 if any filter is empty, 0 if all have charge",
                (ushort)SLELogicType.HasEmptyFilter,
                "bool", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "IsFullyConnected", "IsFullyConnected",
                "1 if all pipe networks are connected, 0 if not",
                (ushort)SLELogicType.IsFullyConnected,
                "bool", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterPowerUsed", "FilterPowerUsed",
                "Power consumed during filtration this tick",
                (ushort)SLELogicType.FilterPowerUsed,
                "float", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FiltrationProcessedMoles", "FiltrationProcessedMoles",
                "Moles processed during this atmospheric tick",
                (ushort)SLELogicType.FiltrationProcessedMoles,
                "float", "read", "Filtration"));

            // FiltrationMachineBase - Per-Filter Properties
            Register(new LogicTypeInfo(
                "FilterQuantity", "FilterQuantity",
                "Filter charge remaining 0.0-1.0",
                (ushort)SLELogicType.FilterQuantity,
                "float", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterIsLow", "FilterIsLow",
                "1 if filter charge is low (<=5%), 0 if not",
                (ushort)SLELogicType.FilterIsLow,
                "bool", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterIsEmpty", "FilterIsEmpty",
                "1 if filter is completely empty, 0 if not",
                (ushort)SLELogicType.FilterIsEmpty,
                "bool", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterTypeHash", "FilterTypeHash",
                "Chemistry.GasType hash of filtered gas",
                (ushort)SLELogicType.FilterTypeHash,
                "int", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterLife", "FilterLife",
                "Filter life tier: 0=Normal, 1=Medium, 2=Large, 3=SuperHeavy",
                (ushort)SLELogicType.FilterLife,
                "int", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterUsedTicks", "FilterUsedTicks",
                "Ticks used since last degradation",
                (ushort)SLELogicType.FilterUsedTicks,
                "int", "read", "Filtration"));

            Register(new LogicTypeInfo(
                "FilterMaxTicks", "FilterMaxTicks",
                "Max ticks before degradation based on filter life",
                (ushort)SLELogicType.FilterMaxTicks,
                "int", "read", "Filtration"));

            // Air Conditioner - Revealed LogicTypes
            Register(new LogicTypeInfo(
                "ACEnergyMoved", "ACEnergyMoved",
                "Joules of thermal energy moved this tick",
                (ushort)SLELogicType.ACEnergyMoved,
                "float", "read", "AirConditioner"));

            Register(new LogicTypeInfo(
                "ACIsFullyConnected", "ACIsFullyConnected",
                "1 if both pipe networks are connected, 0 if not",
                (ushort)SLELogicType.ACIsFullyConnected,
                "bool", "read", "AirConditioner"));

            Register(new LogicTypeInfo(
                "ACPowerUsed", "ACPowerUsed",
                "Power consumed during operation this tick",
                (ushort)SLELogicType.ACPowerUsed,
                "float", "read", "AirConditioner"));

            Register(new LogicTypeInfo(
                "ACEfficiency", "ACEfficiency",
                "Current cooling/heating efficiency 0-1",
                (ushort)SLELogicType.ACEfficiency,
                "float", "read", "AirConditioner"));

            // Wall Cooler - Revealed LogicTypes
            Register(new LogicTypeInfo(
                "CoolerIsEnvironmentOkay", "CoolerIsEnvironmentOkay",
                "1 if room environment is valid for operation, 0 if not",
                (ushort)SLELogicType.CoolerIsEnvironmentOkay,
                "bool", "read", "WallCooler"));

            Register(new LogicTypeInfo(
                "CoolerIsPipeOkay", "CoolerIsPipeOkay",
                "1 if pipe environment is valid for operation, 0 if not",
                (ushort)SLELogicType.CoolerIsPipeOkay,
                "bool", "read", "WallCooler"));

            Register(new LogicTypeInfo(
                "CoolerPowerUsed", "CoolerPowerUsed",
                "Power consumed during cooling this tick",
                (ushort)SLELogicType.CoolerPowerUsed,
                "float", "read", "WallCooler"));

            Register(new LogicTypeInfo(
                "CoolerEnergyMoved", "CoolerEnergyMoved",
                "Joules of thermal energy moved this tick",
                (ushort)SLELogicType.CoolerEnergyMoved,
                "float", "read", "WallCooler"));

            // Wall Heater - Revealed LogicTypes
            Register(new LogicTypeInfo(
                "HeaterIsEnvironmentOkay", "HeaterIsEnvironmentOkay",
                "1 if room environment is valid for operation, 0 if not",
                (ushort)SLELogicType.HeaterIsEnvironmentOkay,
                "bool", "read", "WallHeater"));

            Register(new LogicTypeInfo(
                "HeaterIsPipeOkay", "HeaterIsPipeOkay",
                "1 if pipe environment is valid for operation, 0 if not",
                (ushort)SLELogicType.HeaterIsPipeOkay,
                "bool", "read", "WallHeater"));

            Register(new LogicTypeInfo(
                "HeaterPowerUsed", "HeaterPowerUsed",
                "Power consumed during heating this tick",
                (ushort)SLELogicType.HeaterPowerUsed,
                "float", "read", "WallHeater"));

            Register(new LogicTypeInfo(
                "HeaterEnergyTransfer", "HeaterEnergyTransfer",
                "Joules of thermal energy transferred per tick",
                (ushort)SLELogicType.HeaterEnergyTransfer,
                "float", "read", "WallHeater"));

            Register(new LogicTypeInfo(
                "HeaterMaxTemperature", "HeaterMaxTemperature",
                "Maximum temperature limit in Kelvin (hardcoded 850K)",
                (ushort)SLELogicType.HeaterMaxTemperature,
                "float", "read", "WallHeater"));

            // Active Vent - Revealed LogicTypes
            Register(new LogicTypeInfo(
                "VentFlowStatus", "VentFlowStatus",
                "Flow indicator: 0=Idle, 1=InwardOff, 2=InwardOn, 3=OutwardOff, 4=OutwardOn, 5=BlockedIn, 6=BlockedOut",
                (ushort)SLELogicType.VentFlowStatus,
                "int", "read", "ActiveVent"));

            Register(new LogicTypeInfo(
                "VentIsConnected", "VentIsConnected",
                "1 if vent has valid atmosphere connection, 0 if not",
                (ushort)SLELogicType.VentIsConnected,
                "bool", "read", "ActiveVent"));

            Register(new LogicTypeInfo(
                "VentPowerUsed", "VentPowerUsed",
                "Power consumed during operation this tick",
                (ushort)SLELogicType.VentPowerUsed,
                "float", "read", "ActiveVent"));

            // StateChangeDevice - Condensation/Evaporation Chambers
            Register(new LogicTypeInfo(
                "ChamberEnergyTransfer", "ChamberEnergyTransfer",
                "Heat energy transfer rate in joules/tick",
                (ushort)SLELogicType.ChamberEnergyTransfer,
                "float", "read", "StateChangeDevice"));

            Register(new LogicTypeInfo(
                "ChamberIsOperable", "ChamberIsOperable",
                "1 if device is operable (structure complete, powered), 0 if not",
                (ushort)SLELogicType.ChamberIsOperable,
                "bool", "read", "StateChangeDevice"));

            Register(new LogicTypeInfo(
                "ChamberVolume", "ChamberVolume",
                "Internal chamber volume in litres",
                (ushort)SLELogicType.ChamberVolume,
                "float", "read", "StateChangeDevice"));

            Register(new LogicTypeInfo(
                "ChamberHeatExchangeRatio", "ChamberHeatExchangeRatio",
                "Heat exchange efficiency 0-1 (based on pressures)",
                (ushort)SLELogicType.ChamberHeatExchangeRatio,
                "float", "read", "StateChangeDevice"));

            Register(new LogicTypeInfo(
                "ChamberLiquidRatio", "ChamberLiquidRatio",
                "Liquid volume ratio 0-1 in internal atmosphere",
                (ushort)SLELogicType.ChamberLiquidRatio,
                "float", "read", "StateChangeDevice"));

            // Fabricator/Printer
            Register(new LogicTypeInfo(
                "FabricatorCurrentIndex", "FabricatorCurrentIndex",
                "Index of currently selected recipe (0 to RecipeCount-1)",
                (ushort)SLELogicType.FabricatorCurrentIndex,
                "int", "read", "Fabricator"));

            Register(new LogicTypeInfo(
                "FabricatorRecipeCount", "FabricatorRecipeCount",
                "Total number of recipes available at current tier",
                (ushort)SLELogicType.FabricatorRecipeCount,
                "int", "read", "Fabricator"));

            Register(new LogicTypeInfo(
                "FabricatorCurrentTier", "FabricatorCurrentTier",
                "Current machine tier (0=basic, 1=tier1, 2=tier2)",
                (ushort)SLELogicType.FabricatorCurrentTier,
                "int", "read", "Fabricator"));

            Register(new LogicTypeInfo(
                "FabricatorTimeMultiplier", "FabricatorTimeMultiplier",
                "Build time multiplier for current tier",
                (ushort)SLELogicType.FabricatorTimeMultiplier,
                "float", "read", "Fabricator"));

            Register(new LogicTypeInfo(
                "FabricatorPowerUsed", "FabricatorPowerUsed",
                "Power consumed during production this tick",
                (ushort)SLELogicType.FabricatorPowerUsed,
                "float", "read", "Fabricator"));

            Register(new LogicTypeInfo(
                "FabricatorMakingIndex", "FabricatorMakingIndex",
                "Index of recipe currently being fabricated",
                (ushort)SLELogicType.FabricatorMakingIndex,
                "int", "read", "Fabricator"));

            // Advanced Composter
            Register(new LogicTypeInfo(
                "ComposterGrindProgress", "ComposterGrindProgress",
                "Grinding progress for current item (0 to 1.5 seconds)",
                (ushort)SLELogicType.ComposterGrindProgress,
                "float", "read", "AdvancedComposter"));

            Register(new LogicTypeInfo(
                "ComposterBatchProgress", "ComposterBatchProgress",
                "Batch processing time (0 to 60 seconds until fertilizer)",
                (ushort)SLELogicType.ComposterBatchProgress,
                "float", "read", "AdvancedComposter"));

            Register(new LogicTypeInfo(
                "ComposterDecayCount", "ComposterDecayCount",
                "Count of items that boost GrowthSpeed",
                (ushort)SLELogicType.ComposterDecayCount,
                "int", "read", "AdvancedComposter"));

            Register(new LogicTypeInfo(
                "ComposterNormalCount", "ComposterNormalCount",
                "Count of items that boost HarvestQuantity",
                (ushort)SLELogicType.ComposterNormalCount,
                "int", "read", "AdvancedComposter"));

            Register(new LogicTypeInfo(
                "ComposterBiomassCount", "ComposterBiomassCount",
                "Count of items that boost GrowthCycles",
                (ushort)SLELogicType.ComposterBiomassCount,
                "int", "read", "AdvancedComposter"));

            Register(new LogicTypeInfo(
                "ComposterPowerUsed", "ComposterPowerUsed",
                "Power consumed during processing this tick",
                (ushort)SLELogicType.ComposterPowerUsed,
                "float", "read", "AdvancedComposter"));

            Register(new LogicTypeInfo(
                "ComposterCanProcess", "ComposterCanProcess",
                "1 if ready to process (3+ items), 0 if not",
                (ushort)SLELogicType.ComposterCanProcess,
                "bool", "read", "AdvancedComposter"));

            Register(new LogicTypeInfo(
                "ComposterIsOperable", "ComposterIsOperable",
                "1 if device is operable (input connected), 0 if not",
                (ushort)SLELogicType.ComposterIsOperable,
                "bool", "read", "AdvancedComposter"));

            // AIMeE Bot / RobotMining
            Register(new LogicTypeInfo(
                "RobotIsStorageEmpty", "RobotIsStorageEmpty",
                "1 if robot storage is empty, 0 if not",
                (ushort)SLELogicType.RobotIsStorageEmpty,
                "bool", "read", "RobotMining"));

            Register(new LogicTypeInfo(
                "RobotIsStorageFull", "RobotIsStorageFull",
                "1 if robot storage is full, 0 if not",
                (ushort)SLELogicType.RobotIsStorageFull,
                "bool", "read", "RobotMining"));

            Register(new LogicTypeInfo(
                "RobotIsOperable", "RobotIsOperable",
                "1 if robot is operable (chip valid, no errors), 0 if not",
                (ushort)SLELogicType.RobotIsOperable,
                "bool", "read", "RobotMining"));

            Register(new LogicTypeInfo(
                "RobotBatteryRatio", "RobotBatteryRatio",
                "Battery charge ratio 0-1 (current/max)",
                (ushort)SLELogicType.RobotBatteryRatio,
                "float", "read", "RobotMining"));

            Register(new LogicTypeInfo(
                "RobotIsBusy", "RobotIsBusy",
                "1 if robot is busy pathfinding/working, 0 if idle",
                (ushort)SLELogicType.RobotIsBusy,
                "bool", "read", "RobotMining"));

            Register(new LogicTypeInfo(
                "RobotDamageRatio", "RobotDamageRatio",
                "Robot damage ratio 0-1 (0=undamaged, 1=destroyed)",
                (ushort)SLELogicType.RobotDamageRatio,
                "float", "read", "RobotMining"));

            Register(new LogicTypeInfo(
                "RobotStorageCount", "RobotStorageCount",
                "Number of ore slots currently filled",
                (ushort)SLELogicType.RobotStorageCount,
                "int", "read", "RobotMining"));

            Register(new LogicTypeInfo(
                "RobotStorageCapacity", "RobotStorageCapacity",
                "Total ore storage slot capacity",
                (ushort)SLELogicType.RobotStorageCapacity,
                "int", "read", "RobotMining"));

            // Quarry / Autominer Small
            Register(new LogicTypeInfo(
                "QuarryDrillState", "QuarryDrillState",
                "Drill state: 0=Idle, 1=Drilling, 2=Transporting, 3=Delivering",
                (ushort)SLELogicType.QuarryDrillState,
                "int", "read", "Quarry"));

            Register(new LogicTypeInfo(
                "QuarryOreCount", "QuarryOreCount",
                "Number of ore items mined and ready for export",
                (ushort)SLELogicType.QuarryOreCount,
                "int", "read", "Quarry"));

            Register(new LogicTypeInfo(
                "QuarryDepth", "QuarryDepth",
                "Current drill depth in voxels",
                (ushort)SLELogicType.QuarryDepth,
                "float", "read", "Quarry"));

            Register(new LogicTypeInfo(
                "QuarryMaxDepth", "QuarryMaxDepth",
                "Maximum drill depth reached",
                (ushort)SLELogicType.QuarryMaxDepth,
                "float", "read", "Quarry"));

            Register(new LogicTypeInfo(
                "QuarryIsDrillFinished", "QuarryIsDrillFinished",
                "1 if drill has finished mining, 0 if not",
                (ushort)SLELogicType.QuarryIsDrillFinished,
                "bool", "read", "Quarry"));

            Register(new LogicTypeInfo(
                "QuarryIsTransporting", "QuarryIsTransporting",
                "1 if transporting ore to output, 0 if not",
                (ushort)SLELogicType.QuarryIsTransporting,
                "bool", "read", "Quarry"));

            Register(new LogicTypeInfo(
                "QuarryIsDelivering", "QuarryIsDelivering",
                "1 if delivering ore to chute, 0 if not",
                (ushort)SLELogicType.QuarryIsDelivering,
                "bool", "read", "Quarry"));

            // HorizontalQuarry / Ogre
            Register(new LogicTypeInfo(
                "OgreState", "OgreState",
                "State: 0=Idle, 1=Mining, 2=Returning, 3=Delivering, 4=AwaitingExport",
                (ushort)SLELogicType.OgreState,
                "int", "read", "HorizontalQuarry"));

            Register(new LogicTypeInfo(
                "OgreOreCount", "OgreOreCount",
                "Number of ore items mined and ready for export",
                (ushort)SLELogicType.OgreOreCount,
                "int", "read", "HorizontalQuarry"));

            Register(new LogicTypeInfo(
                "OgrePosition", "OgrePosition",
                "Current horizontal position along mining path",
                (ushort)SLELogicType.OgrePosition,
                "float", "read", "HorizontalQuarry"));

            Register(new LogicTypeInfo(
                "OgreMiningComplete", "OgreMiningComplete",
                "1 if mining path complete, 0 if mining in progress",
                (ushort)SLELogicType.OgreMiningComplete,
                "bool", "read", "HorizontalQuarry"));

            Register(new LogicTypeInfo(
                "OgreIsReturning", "OgreIsReturning",
                "1 if ogre is returning to start position, 0 if not",
                (ushort)SLELogicType.OgreIsReturning,
                "bool", "read", "HorizontalQuarry"));

            Register(new LogicTypeInfo(
                "OgreQueueFull", "OgreQueueFull",
                "1 if ore queue is full, 0 if not",
                (ushort)SLELogicType.OgreQueueFull,
                "bool", "read", "HorizontalQuarry"));

            // Recycler
            Register(new LogicTypeInfo(
                "RecyclerReagentTotal", "RecyclerReagentTotal",
                "Total reagent quantity from all processed items",
                (ushort)SLELogicType.RecyclerReagentTotal,
                "float", "read", "Recycler"));

            Register(new LogicTypeInfo(
                "RecyclerIsExporting", "RecyclerIsExporting",
                "1 if currently exporting reagents, 0 if not",
                (ushort)SLELogicType.RecyclerIsExporting,
                "bool", "read", "Recycler"));

            Register(new LogicTypeInfo(
                "RecyclerAtCapacity", "RecyclerAtCapacity",
                "1 if output is at capacity, 0 if not",
                (ushort)SLELogicType.RecyclerAtCapacity,
                "bool", "read", "Recycler"));

            Register(new LogicTypeInfo(
                "RecyclerIdleTicks", "RecyclerIdleTicks",
                "Number of ticks spent idle",
                (ushort)SLELogicType.RecyclerIdleTicks,
                "int", "read", "Recycler"));

            Register(new LogicTypeInfo(
                "RecyclerIsProcessing", "RecyclerIsProcessing",
                "1 if device is currently processing an item, 0 if not",
                (ushort)SLELogicType.RecyclerIsProcessing,
                "bool", "read", "Recycler"));

            // Stirling Engine
            Register(new LogicTypeInfo(
                "StirlingHotTemperature", "StirlingHotTemperature",
                "Hot side (input) temperature in Kelvin",
                (ushort)SLELogicType.StirlingHotTemperature,
                "float", "read", "StirlingEngine"));

            Register(new LogicTypeInfo(
                "StirlingColdTemperature", "StirlingColdTemperature",
                "Cold side (output) temperature in Kelvin",
                (ushort)SLELogicType.StirlingColdTemperature,
                "float", "read", "StirlingEngine"));

            Register(new LogicTypeInfo(
                "StirlingTemperatureDelta", "StirlingTemperatureDelta",
                "Temperature differential between hot and cold sides",
                (ushort)SLELogicType.StirlingTemperatureDelta,
                "float", "read", "StirlingEngine"));

            Register(new LogicTypeInfo(
                "StirlingEfficiency", "StirlingEfficiency",
                "Current operating efficiency 0-1",
                (ushort)SLELogicType.StirlingEfficiency,
                "float", "read", "StirlingEngine"));

            Register(new LogicTypeInfo(
                "StirlingMaxPower", "StirlingMaxPower",
                "Maximum power output at current conditions in watts",
                (ushort)SLELogicType.StirlingMaxPower,
                "float", "read", "StirlingEngine"));

            Register(new LogicTypeInfo(
                "StirlingIsConnected", "StirlingIsConnected",
                "1 if both atmospheres are connected, 0 if not",
                (ushort)SLELogicType.StirlingIsConnected,
                "bool", "read", "StirlingEngine"));

            // Rocket Miner
            Register(new LogicTypeInfo(
                "RocketMiningProgress", "RocketMiningProgress",
                "Current mining progress 0-1",
                (ushort)SLELogicType.RocketMiningProgress,
                "float", "read", "RocketMiner"));

            Register(new LogicTypeInfo(
                "RocketNextOreHash", "RocketNextOreHash",
                "Hash of next ore type to be produced",
                (ushort)SLELogicType.RocketNextOreHash,
                "int", "read", "RocketMiner"));

            Register(new LogicTypeInfo(
                "RocketMiningQuantity", "RocketMiningQuantity",
                "Quantity of ore mined this cycle",
                (ushort)SLELogicType.RocketMiningQuantity,
                "int", "read", "RocketMiner"));

            Register(new LogicTypeInfo(
                "RocketIsMining", "RocketIsMining",
                "1 if rocket miner is actively mining, 0 if not",
                (ushort)SLELogicType.RocketIsMining,
                "bool", "read", "RocketMiner"));

            // Centrifuge Expansion
            Register(new LogicTypeInfo(
                "CentrifugeRPM", "CentrifugeRPM",
                "Current centrifuge RPM",
                (ushort)SLELogicType.CentrifugeRPM,
                "float", "read", "Centrifuge"));

            Register(new LogicTypeInfo(
                "CentrifugeReagentTotal", "CentrifugeReagentTotal",
                "Total reagent quantity in centrifuge",
                (ushort)SLELogicType.CentrifugeReagentTotal,
                "float", "read", "Centrifuge"));

            Register(new LogicTypeInfo(
                "CentrifugeLidClosed", "CentrifugeLidClosed",
                "1 if lid is closed, 0 if open",
                (ushort)SLELogicType.CentrifugeLidClosed,
                "bool", "read", "Centrifuge"));

            // Landing Pad Center
            Register(new LogicTypeInfo(
                "PadContactStatus", "PadContactStatus",
                "Contact status: 0=NoContact, 1=Approaching, 2=WaitingApproach, 3=WaitingDoors, 4=Landed",
                (ushort)SLELogicType.PadContactStatus,
                "int", "read", "LandingPad"));

            Register(new LogicTypeInfo(
                "PadIsTraderReady", "PadIsTraderReady",
                "1 if trader is ready to trade, 0 if not",
                (ushort)SLELogicType.PadIsTraderReady,
                "bool", "read", "LandingPad"));

            Register(new LogicTypeInfo(
                "PadHasContact", "PadHasContact",
                "1 if pad has an active contact, 0 if not",
                (ushort)SLELogicType.PadHasContact,
                "bool", "read", "LandingPad"));

            Register(new LogicTypeInfo(
                "PadIsLocked", "PadIsLocked",
                "1 if pad is locked, 0 if not",
                (ushort)SLELogicType.PadIsLocked,
                "bool", "read", "LandingPad"));

            Register(new LogicTypeInfo(
                "PadWaypointHeight", "PadWaypointHeight",
                "Virtual waypoint height setting (0-50)",
                (ushort)SLELogicType.PadWaypointHeight,
                "float", "read", "LandingPad"));

            // Area Power Controller
            Register(new LogicTypeInfo(
                "APCMaximumPower", "APCMaximumPower",
                "Total maximum power (Battery + Network capacity)",
                (ushort)SLELogicType.APCMaximumPower,
                "float", "read", "APC"));

            UnityEngine.Debug.Log($"[SLE] Registered {Count} custom LogicTypes");
        }

        private static void Register(LogicTypeInfo info)
        {
            _byValue[info.Value] = info;
            _byName[info.Name] = info;
        }

        public static bool TryGet(ushort value, out LogicTypeInfo info)
        {
            return _byValue.TryGetValue(value, out info);
        }

        public static bool TryGet(string name, out LogicTypeInfo info)
        {
            return _byName.TryGetValue(name, out info);
        }

        public static bool IsCustomLogicType(ushort value)
        {
            return value >= 1000 && _byValue.ContainsKey(value);
        }

        /// <summary>
        /// Export all LogicTypes to JSON for compiler integration.
        /// </summary>
        public static string ExportToJson()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"logicTypes\": [");

            bool first = true;
            foreach (var info in All)
            {
                if (!first) sb.AppendLine(",");
                first = false;

                sb.AppendLine("    {");
                sb.AppendLine($"      \"name\": \"{info.Name}\",");
                sb.AppendLine($"      \"displayName\": \"{info.DisplayName}\",");
                sb.AppendLine($"      \"description\": \"{EscapeJson(info.Description)}\",");
                sb.AppendLine($"      \"value\": {info.Value},");
                sb.AppendLine($"      \"hash\": {info.Hash},");
                sb.AppendLine($"      \"dataType\": \"{info.DataType}\",");
                sb.AppendLine($"      \"access\": \"{info.Access}\",");
                sb.AppendLine($"      \"category\": \"{info.Category}\"");
                sb.Append("    }");
            }

            sb.AppendLine();
            sb.AppendLine("  ]");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Save JSON export to file.
        /// </summary>
        public static void SaveToFile(string path)
        {
            var json = ExportToJson();
            File.WriteAllText(path, json);
            UnityEngine.Debug.Log($"[SLE] Exported LogicTypes to {path}");
        }

        private static string EscapeJson(string s)
        {
            return s?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";
        }
    }
}
