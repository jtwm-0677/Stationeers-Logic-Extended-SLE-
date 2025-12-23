namespace SLE.Core
{
    /// <summary>
    /// Custom LogicType values for Stationeers Logic Extended.
    /// Values start at 1000 to avoid conflicts with game updates.
    /// </summary>
    public enum SLELogicType : ushort
    {
        // ============================================
        // Contact Selection & Filtering (1000-1009)
        // ============================================

        /// <summary>Select contact by index (0-based). Read/Write.</summary>
        ContactIndex = 1000,

        /// <summary>Total visible contacts. Read only.</summary>
        ContactCount = 1001,

        /// <summary>Filter mode selector. Read/Write. See FilterMode enum.</summary>
        FilterMode = 1002,

        /// <summary>Filter parameter value (e.g., ShuttleType). Read/Write.</summary>
        FilterValue = 1003,

        /// <summary>Count of contacts matching current filter. Read only.</summary>
        FilteredCount = 1004,

        // ============================================
        // Selected Contact Properties (1010-1029)
        // ============================================

        /// <summary>ShuttleType enum value of selected contact. Read only.</summary>
        ContactShuttleType = 1010,

        /// <summary>Seconds until contact leaves range. Read only.</summary>
        ContactLifetime = 1011,

        /// <summary>Alignment angle in degrees. Read only.</summary>
        ContactDegreeOffset = 1012,

        /// <summary>1 if contact is resolved, 0 if not. Read only.</summary>
        ContactResolved = 1013,

        /// <summary>1 if trader has been contacted, 0 if not. Read only.</summary>
        ContactContacted = 1014,

        /// <summary>Resolution progress 0.0-1.0. Read only.</summary>
        ContactResolutionProgress = 1015,

        /// <summary>Minimum watts required to resolve. Read only.</summary>
        ContactMinWattsResolve = 1016,

        /// <summary>Minimum watts required to contact. Read only.</summary>
        ContactMinWattsContact = 1017,

        /// <summary>Seconds required to establish contact. Read only.</summary>
        ContactSecondsToContact = 1018,

        /// <summary>Trader type hash (same as game's ContactTypeId). Read only.</summary>
        ContactTraderHash = 1019,

        /// <summary>Unique reference ID of contact. Read only.</summary>
        ContactReferenceId = 1020,

        // Note: Pad sizes use vanilla SizeX/SizeZ LogicTypes

        // ============================================
        // Dish State (1030-1039)
        // ============================================

        /// <summary>Actual watts reaching selected contact. Read only.</summary>
        DishWattageOnContact = 1030,

        /// <summary>1 if dish is currently interrogating. Read only.</summary>
        DishIsInterrogating = 1031,

        /// <summary>ReferenceId of contact being interrogated. Read only.</summary>
        DishInterrogatingId = 1032,

        // ============================================
        // Centrifuge (1100-1109)
        // ============================================

        /// <summary>Processing progress 0-100%. Read only. Works on both Centrifuge and CombustionCentrifuge.</summary>
        CentrifugeProcessing = 1100,

        // ============================================
        // DaylightSensor / Realtime Data (1110-1119)
        // ============================================

        /// <summary>Time of day 0-360 degrees. Read only.</summary>
        TimeOfDay = 1110,

        /// <summary>1 if eclipse is occurring, 0 if not. Read only.</summary>
        IsEclipse = 1111,

        /// <summary>Eclipse intensity 0.0-1.0. Read only.</summary>
        EclipseRatio = 1112,

        /// <summary>Number of days since world creation. Read only.</summary>
        DaysPast = 1113,

        /// <summary>Length of a day in seconds. Read only.</summary>
        DayLengthSeconds = 1114,

        /// <summary>World latitude in degrees. Read only.</summary>
        Latitude = 1115,

        /// <summary>World longitude in degrees. Read only.</summary>
        Longitude = 1116,

        /// <summary>Weather solar ratio 0-1 (1 = full sun, lower during storms). Read only.</summary>
        WeatherSolarRatio = 1117,

        // ============================================
        // WindTurbineGenerator (1120-1129)
        // ============================================

        /// <summary>Current global wind strength 0-1. Read only.</summary>
        WindSpeed = 1120,

        /// <summary>Current max power output (storm-aware). Read only.</summary>
        MaxPower = 1121,

        /// <summary>Current turbine blade rotation speed 0-1. Read only.</summary>
        TurbineSpeed = 1122,

        /// <summary>Clamped atmospheric pressure in kPa. Read only.</summary>
        AtmosphericPressure = 1123,

        // ============================================
        // SolidFuelGenerator (1130-1139)
        // ============================================

        /// <summary>Fuel buffer remaining in ticks. Read only.</summary>
        FuelTicks = 1130,

        // ============================================
        // WeatherStation (1140-1149)
        // ============================================

        /// <summary>Current weather event wind strength 0-1. Read only.</summary>
        WeatherWindStrength = 1140,

        /// <summary>Days since last weather event ended. Read only.</summary>
        DaysSinceLastWeatherEvent = 1141,

        // ============================================
        // DeepMiner (1150-1159)
        // ============================================

        /// <summary>Mining cycle progress 0-100%. Read only.</summary>
        MiningProgress = 1150,

        /// <summary>Hash of current ore type being produced. Read only.</summary>
        CurrentOreHash = 1151,

        // ============================================
        // HydroponicsTrayDevice (1160-1179)
        // ============================================

        /// <summary>Current light exposure level. Read only.</summary>
        LightExposure = 1160,

        /// <summary>1 if lit by a powered grow light, 0 if not. Read only.</summary>
        IsLitByGrowLight = 1161,

        /// <summary>Water amount in internal atmosphere (moles). Read only.</summary>
        WaterMoles = 1162,

        /// <summary>1 if plant has been fertilized, 0 if not. Read only.</summary>
        PlantIsFertilized = 1163,

        /// <summary>Plant growth efficiency 0-100%. Read only.</summary>
        PlantGrowthEfficiency = 1164,

        /// <summary>Plant breathing/gas efficiency 0-100%. Read only.</summary>
        BreathingEfficiency = 1165,

        /// <summary>Plant temperature efficiency 0-100%. Read only.</summary>
        TemperatureEfficiency = 1166,

        /// <summary>Plant light efficiency 0-100%. Read only.</summary>
        PlantLightEfficiency = 1167,

        /// <summary>Plant pressure efficiency 0-100%. Read only.</summary>
        PlantPressureEfficiency = 1168,

        /// <summary>Plant hydration efficiency 0-100%. Read only.</summary>
        HydrationEfficiency = 1169,

        // ============================================
        // GasFuelGenerator (1200-1209)
        // ============================================

        /// <summary>Combustion energy produced (before efficiency conversion). Read only.</summary>
        CombustionEnergy = 1200,

        /// <summary>1 if atmosphere meets pressure/temp requirements, 0 if not. Read only.</summary>
        IsValidAtmosphere = 1201,

        /// <summary>1 if conditions will trigger shutdown, 0 if not. Read only.</summary>
        DoShutdown = 1202,

        /// <summary>Minimum operating temperature in Kelvin. Read only.</summary>
        MinTemperature = 1203,

        /// <summary>Maximum operating temperature in Kelvin. Read only.</summary>
        MaxTemperature = 1204,

        /// <summary>Minimum operating pressure in Pa. Read only.</summary>
        MinPressure = 1205,

        // ============================================
        // Battery (1210-1219)
        // ============================================

        /// <summary>Power deficit (PowerStored - PowerMaximum). Negative when not full. Read only.</summary>
        PowerDelta = 1210,

        /// <summary>1 if battery is submerged in liquid (short risk), 0 if not. Read only.</summary>
        BatteryIsSubmerged = 1211,

        /// <summary>Ticks input connection has been submerged. Read only.</summary>
        InputSubmergedTicks = 1212,

        /// <summary>Ticks output connection has been submerged. Read only.</summary>
        OutputSubmergedTicks = 1213,

        /// <summary>1 if battery is empty (Mode == 0), 0 if not. Read only.</summary>
        BatteryIsEmpty = 1214,

        /// <summary>1 if battery is fully charged (Mode == 6), 0 if not. Read only.</summary>
        BatteryIsCharged = 1215,

        // ============================================
        // SolarPanel (1220-1229)
        // ============================================

        /// <summary>Sun visibility factor 0-1 (affected by obstructions). Read only.</summary>
        SolarVisibility = 1220,

        /// <summary>Damage ratio 0-1 (0=undamaged, 1=fully damaged). Read only.</summary>
        SolarDamageRatio = 1221,

        /// <summary>Total damage points accumulated. Read only.</summary>
        SolarDamageTotal = 1222,

        /// <summary>Current health as percentage 0-100. Read only.</summary>
        SolarHealth = 1223,

        /// <summary>Current efficiency as percentage 0-100 (includes damage). Read only.</summary>
        SolarEfficiency = 1224,

        /// <summary>1 if panel is operable, 0 if not. Read only.</summary>
        SolarIsOperable = 1225,

        /// <summary>1 if panel is broken, 0 if not. Read only.</summary>
        SolarIsBroken = 1226,

        /// <summary>Horizontal rotation speed in degrees/sec. Read only.</summary>
        SolarMovementSpeedH = 1227,

        /// <summary>Vertical rotation speed in degrees/sec. Read only.</summary>
        SolarMovementSpeedV = 1228,

        // ============================================
        // H2Combustor (1230-1239)
        // ============================================

        /// <summary>Moles processed this atmospheric tick. Read only.</summary>
        H2CombustorProcessedMoles = 1230,

        /// <summary>Power consumed this tick (3600W active, 50W idle). Read only.</summary>
        H2CombustorUsedPower = 1231,

        /// <summary>1 if device is operable (connections valid), 0 if not. Read only.</summary>
        H2CombustorIsOperable = 1232,

        /// <summary>IC chip error state code. Read only.</summary>
        H2CombustorCodeError = 1233,

        // ============================================
        // Electrolyzer (1240-1249)
        // ============================================

        /// <summary>Moles processed this atmospheric tick. Read only.</summary>
        ElectrolyzerProcessedMoles = 1240,

        /// <summary>Power consumed this tick (3600W active, 50W idle). Read only.</summary>
        ElectrolyzerUsedPower = 1241,

        /// <summary>1 if device is operable (connections valid), 0 if not. Read only.</summary>
        ElectrolyzerIsOperable = 1242,

        /// <summary>IC chip error state code. Read only.</summary>
        ElectrolyzerCodeError = 1243,

        // ============================================
        // Harvester (1180-1199)
        // ============================================

        /// <summary>1 if harvester is positioned over a hydroponics tray, 0 if not. Read only.</summary>
        HasTray = 1180,

        /// <summary>1 if currently performing harvest operation, 0 if not. Read only.</summary>
        IsHarvesting = 1181,

        /// <summary>1 if currently performing plant operation, 0 if not. Read only.</summary>
        IsPlanting = 1182,

        /// <summary>Arm state: 0=Idle, 1=Planting, 2=Harvesting. Read only.</summary>
        ArmState = 1183,

        /// <summary>1 if plant is ready in import slot, 0 if not. Read only.</summary>
        HasImportPlant = 1184,

        /// <summary>PrefabHash of plant in import slot. Read only.</summary>
        ImportPlantHash = 1185,

        /// <summary>1 if fertilizer is in tray's fertilizer slot, 0 if not. Read only.</summary>
        HasFertilizer = 1186,

        /// <summary>Remaining fertilizer cycles. Read only.</summary>
        FertilizerCycles = 1187,

        /// <summary>Fertilizer harvest yield multiplier. Read only.</summary>
        FertilizerHarvestBoost = 1188,

        /// <summary>Fertilizer growth speed multiplier. Read only.</summary>
        FertilizerGrowthSpeed = 1189,

        // ============================================
        // PipeAnalyzer - Partial Pressures (1250-1259)
        // ============================================

        /// <summary>Partial pressure of Oxygen in kPa. Read only.</summary>
        PartialPressureO2 = 1250,

        /// <summary>Partial pressure of Carbon Dioxide in kPa. Read only.</summary>
        PartialPressureCO2 = 1251,

        /// <summary>Partial pressure of Nitrogen in kPa. Read only.</summary>
        PartialPressureN2 = 1252,

        /// <summary>Partial pressure of Volatiles in kPa. Read only.</summary>
        PartialPressureVolatiles = 1253,

        /// <summary>Partial pressure of Nitrous Oxide in kPa. Read only.</summary>
        PartialPressureN2O = 1254,

        /// <summary>Partial pressure of Pollutant in kPa. Read only.</summary>
        PartialPressurePollutant = 1255,

        /// <summary>Partial pressure of Hydrogen in kPa. Read only.</summary>
        PartialPressureH2 = 1256,

        /// <summary>Partial pressure of Steam/Water in kPa. Read only.</summary>
        PartialPressureSteam = 1257,

        /// <summary>Partial pressure of Toxins in kPa. Read only.</summary>
        PartialPressureToxins = 1258,

        // ============================================
        // PipeAnalyzer - Gas/Liquid Separation (1260-1269)
        // ============================================

        /// <summary>Total moles of gases only (excludes liquids). Read only.</summary>
        TotalMolesGasses = 1260,

        /// <summary>Total moles of liquids only (excludes gases). Read only.</summary>
        TotalMolesLiquids = 1261,

        /// <summary>Ratio of liquid volume to total volume 0-1. Read only.</summary>
        LiquidVolumeRatio = 1262,

        /// <summary>Gas-only pressure in kPa (excludes liquid pressure). Read only.</summary>
        PressureGasses = 1263,

        /// <summary>Liquid pressure offset in kPa. Read only.</summary>
        LiquidPressureOffset = 1264,

        // ============================================
        // PipeAnalyzer - Combustion & Reactions (1270-1279)
        // ============================================

        /// <summary>Energy from combustion reactions in joules. Read only.</summary>
        PipeCombustionEnergy = 1270,

        /// <summary>Clean burn rate/efficiency 0-1. Read only.</summary>
        CleanBurnRate = 1271,

        /// <summary>1 if atmosphere is inflamed/on fire, 0 if not. Read only.</summary>
        Inflamed = 1272,

        /// <summary>Fire suppression ticks remaining. Read only.</summary>
        Suppressed = 1273,

        // ============================================
        // PipeAnalyzer - Network & State (1280-1289)
        // ============================================

        /// <summary>Heat energy being convected in watts. Read only.</summary>
        EnergyConvected = 1280,

        /// <summary>Heat energy being radiated in watts. Read only.</summary>
        EnergyRadiated = 1281,

        /// <summary>1 if pressure is above Armstrong limit (6.3kPa), 0 if not. Read only.</summary>
        IsAboveArmstrong = 1282,

        /// <summary>1 if condensation is occurring, 0 if not. Read only.</summary>
        Condensation = 1283,

        /// <summary>Network content type: 0=Gas, 1=Liquid. Read only.</summary>
        NetworkContentType = 1284,

        /// <summary>Maximum burst pressure for connected pipe type in kPa. Read only.</summary>
        PipeMaxPressure = 1285,

        /// <summary>Max of pressure stress OR liquid stress (0-1+, >=0.8 = stressed). Read only.</summary>
        PipeStressRatio = 1286,

        /// <summary>1 if pipe is stressed (high pressure OR liquid in gas pipe), 0 if not. Read only.</summary>
        PipeIsStressed = 1287,

        /// <summary>Pipe damage ratio 0-1 (1 = burst/destroyed). Read only.</summary>
        PipeDamageRatio = 1288,

        /// <summary>1 if pipe has burst, 0 if intact. Read only.</summary>
        PipeIsBurst = 1289,

        /// <summary>Damage type bitmask: 1=Pressure, 2=Liquid, 4=Frozen. Read only.</summary>
        PipeDamageType = 1290,

        /// <summary>1 if any pipe in network has a fault, 0 if not. Read only.</summary>
        NetworkHasFault = 1291,

        /// <summary>Highest damage ratio of any pipe in the network 0-1. Read only.</summary>
        NetworkWorstDamage = 1292,

        // ============================================
        // FurnaceBase - All Furnaces (1300-1319)
        // ============================================

        /// <summary>Internal furnace temperature in Kelvin. Read only.</summary>
        FurnaceTemperature = 1300,

        /// <summary>Internal furnace pressure in kPa. Read only.</summary>
        FurnacePressure = 1301,

        /// <summary>Total moles of gas inside furnace. Read only.</summary>
        FurnaceTotalMoles = 1302,

        /// <summary>1 if furnace is actively burning/smelting, 0 if not. Read only.</summary>
        FurnaceInflamed = 1303,

        /// <summary>1 if valid recipe detected, 0 if not. Read only.</summary>
        FurnaceMode = 1304,

        /// <summary>Total quantity of minerals loaded in furnace. Read only.</summary>
        ReagentQuantity = 1305,

        /// <summary>1 if furnace is in dangerous overpressure state, 0 if not. Read only.</summary>
        FurnaceOverpressure = 1306,

        /// <summary>Energy required to complete current recipe. Read only.</summary>
        CurrentRecipeEnergy = 1307,

        /// <summary>1 if pressure is at warning level (66% of max), 0 if not. Read only.</summary>
        FurnaceStressed = 1308,

        /// <summary>1 if furnace has already exploded, 0 if not. Read only.</summary>
        FurnaceHasBlown = 1309,

        /// <summary>Maximum pressure differential before explosion (60795 kPa). Read only.</summary>
        FurnaceMaxPressure = 1310,

        /// <summary>Internal furnace volume in litres. Read only.</summary>
        FurnaceVolume = 1311,

        // ============================================
        // AdvancedFurnace Specific (1320-1329)
        // ============================================

        /// <summary>Minimum input setting value. Read only.</summary>
        MinSettingInput = 1320,

        /// <summary>Maximum input setting value. Read only.</summary>
        MaxSettingInput = 1321,

        /// <summary>Minimum output setting value. Read only.</summary>
        MinSettingOutput = 1322,

        /// <summary>Maximum output setting value. Read only.</summary>
        MaxSettingOutput = 1323,

        // ============================================
        // ArcFurnace Specific (1330-1339)
        // ============================================

        /// <summary>Arc furnace activate state: 0=idle, 1=smelting. Read only.</summary>
        ArcFurnaceActivate = 1330,

        /// <summary>Quantity of items in import slot. Read only.</summary>
        ImportStackSize = 1331,

        /// <summary>Power being consumed for smelting this tick. Read only.</summary>
        SmeltingPower = 1332,

        /// <summary>1 if arc furnace is actively smelting, 0 if not. Read only.</summary>
        ArcFurnaceIsSmelting = 1333,

        // ============================================
        // FiltrationMachineBase - Machine State (1400-1409)
        // ============================================

        /// <summary>Select filter slot by index (0-based). Read/Write.</summary>
        FilterSlotIndex = 1400,

        /// <summary>Total number of gas filter slots. Read only.</summary>
        FilterSlotCount = 1401,

        /// <summary>1 if any filter is empty, 0 if all have charge. Read only.</summary>
        HasEmptyFilter = 1402,

        /// <summary>1 if all pipe networks are connected, 0 if not. Read only.</summary>
        IsFullyConnected = 1403,

        /// <summary>Power consumed during filtration this tick. Read only.</summary>
        FilterPowerUsed = 1404,

        /// <summary>Moles processed during this atmospheric tick. Read only.</summary>
        FiltrationProcessedMoles = 1405,

        // ============================================
        // FiltrationMachineBase - Per-Filter (1410-1419)
        // Selected by FilterSlotIndex
        // ============================================

        /// <summary>Filter charge remaining 0.0-1.0. Read only.</summary>
        FilterQuantity = 1410,

        /// <summary>1 if filter charge is low (<=5%), 0 if not. Read only.</summary>
        FilterIsLow = 1411,

        /// <summary>1 if filter is completely empty, 0 if not. Read only.</summary>
        FilterIsEmpty = 1412,

        /// <summary>Chemistry.GasType hash of filtered gas. Read only.</summary>
        FilterTypeHash = 1413,

        /// <summary>Filter life tier: 0=Normal, 1=Medium, 2=Large, 3=SuperHeavy. Read only.</summary>
        FilterLife = 1414,

        /// <summary>Ticks used since last degradation. Read only.</summary>
        FilterUsedTicks = 1415,

        /// <summary>Max ticks before degradation based on filter life. Read only.</summary>
        FilterMaxTicks = 1416,

        // ============================================
        // Air Conditioner (1500-1519)
        // ============================================

        /// <summary>Joules of thermal energy moved this tick. Read only.</summary>
        ACEnergyMoved = 1500,

        /// <summary>1 if both pipe networks are connected, 0 if not. Read only.</summary>
        ACIsFullyConnected = 1501,

        /// <summary>Power consumed during operation this tick. Read only.</summary>
        ACPowerUsed = 1502,

        /// <summary>Current cooling/heating efficiency 0-1. Read only.</summary>
        ACEfficiency = 1503,

        // ============================================
        // Wall Cooler (1520-1529)
        // ============================================

        /// <summary>1 if room environment is valid for operation, 0 if not. Read only.</summary>
        CoolerIsEnvironmentOkay = 1520,

        /// <summary>1 if pipe environment is valid for operation, 0 if not. Read only.</summary>
        CoolerIsPipeOkay = 1521,

        /// <summary>Power consumed during cooling this tick. Read only.</summary>
        CoolerPowerUsed = 1522,

        /// <summary>Joules of thermal energy moved this tick. Read only.</summary>
        CoolerEnergyMoved = 1523,

        // ============================================
        // Wall Heater (1530-1539)
        // ============================================

        /// <summary>1 if room environment is valid for operation, 0 if not. Read only.</summary>
        HeaterIsEnvironmentOkay = 1530,

        /// <summary>1 if pipe environment is valid for operation, 0 if not. Read only.</summary>
        HeaterIsPipeOkay = 1531,

        /// <summary>Power consumed during heating this tick. Read only.</summary>
        HeaterPowerUsed = 1532,

        /// <summary>Joules of thermal energy transferred per tick. Read only.</summary>
        HeaterEnergyTransfer = 1533,

        /// <summary>Maximum temperature limit in Kelvin (hardcoded 850K). Read only.</summary>
        HeaterMaxTemperature = 1534,

        // ============================================
        // Active Vent (1540-1549)
        // ============================================

        /// <summary>Flow indicator state: 0=Idle, 1=InwardOff, 2=InwardOn, 3=OutwardOff, 4=OutwardOn, 5=BlockedIn, 6=BlockedOut. Read only.</summary>
        VentFlowStatus = 1540,

        /// <summary>1 if vent has valid atmosphere connection, 0 if not. Read only.</summary>
        VentIsConnected = 1541,

        /// <summary>Power consumed during operation this tick. Read only.</summary>
        VentPowerUsed = 1542,

        // ============================================
        // StateChangeDevice - Condensation/Evaporation Chambers (1600-1609)
        // ============================================

        /// <summary>Heat energy transfer rate in joules/tick. Read only.</summary>
        ChamberEnergyTransfer = 1600,

        /// <summary>1 if device is operable (structure complete, powered), 0 if not. Read only.</summary>
        ChamberIsOperable = 1601,

        /// <summary>Internal chamber volume in litres. Read only.</summary>
        ChamberVolume = 1602,

        /// <summary>Heat exchange efficiency 0-1 (based on pressures). Read only.</summary>
        ChamberHeatExchangeRatio = 1603,

        /// <summary>Liquid volume ratio 0-1 in internal atmosphere. Read only.</summary>
        ChamberLiquidRatio = 1604,

        // ============================================
        // Fabricator/Printer (1610-1619)
        // ============================================

        /// <summary>Index of currently selected recipe (0 to RecipeCount-1). Read only.</summary>
        FabricatorCurrentIndex = 1610,

        /// <summary>Total number of recipes available at current tier. Read only.</summary>
        FabricatorRecipeCount = 1611,

        /// <summary>Current machine tier (0=basic, 1=tier1, 2=tier2). Read only.</summary>
        FabricatorCurrentTier = 1612,

        /// <summary>Build time multiplier for current tier. Read only.</summary>
        FabricatorTimeMultiplier = 1613,

        /// <summary>Power consumed during production this tick. Read only.</summary>
        FabricatorPowerUsed = 1614,

        /// <summary>Index of recipe currently being fabricated. Read only.</summary>
        FabricatorMakingIndex = 1615,

        // ============================================
        // Advanced Composter (1620-1629)
        // ============================================

        /// <summary>Grinding progress for current item (0 to 1.5 seconds). Read only.</summary>
        ComposterGrindProgress = 1620,

        /// <summary>Batch processing time (0 to 60 seconds until fertilizer). Read only.</summary>
        ComposterBatchProgress = 1621,

        /// <summary>Count of items that boost GrowthSpeed. Read only.</summary>
        ComposterDecayCount = 1622,

        /// <summary>Count of items that boost HarvestQuantity. Read only.</summary>
        ComposterNormalCount = 1623,

        /// <summary>Count of items that boost GrowthCycles. Read only.</summary>
        ComposterBiomassCount = 1624,

        /// <summary>Power consumed during processing this tick. Read only.</summary>
        ComposterPowerUsed = 1625,

        /// <summary>1 if ready to process (3+ items), 0 if not. Read only.</summary>
        ComposterCanProcess = 1626,

        /// <summary>1 if device is operable (input connected), 0 if not. Read only.</summary>
        ComposterIsOperable = 1627,

        // ============================================
        // AIMeE Bot / RobotMining (1700-1719)
        // ============================================

        /// <summary>1 if robot storage is empty, 0 if not. Read only.</summary>
        RobotIsStorageEmpty = 1700,

        /// <summary>1 if robot storage is full, 0 if not. Read only.</summary>
        RobotIsStorageFull = 1701,

        /// <summary>1 if robot is operable (chip valid, no errors), 0 if not. Read only.</summary>
        RobotIsOperable = 1702,

        /// <summary>Battery charge ratio 0-1 (current/max). Read only.</summary>
        RobotBatteryRatio = 1703,

        /// <summary>1 if robot is busy pathfinding/working, 0 if idle. Read only.</summary>
        RobotIsBusy = 1704,

        /// <summary>Robot damage ratio 0-1 (0=undamaged, 1=destroyed). Read only.</summary>
        RobotDamageRatio = 1705,

        /// <summary>Number of ore slots currently filled. Read only.</summary>
        RobotStorageCount = 1706,

        /// <summary>Total ore storage slot capacity. Read only.</summary>
        RobotStorageCapacity = 1707,

        // ============================================
        // Quarry / Autominer Small (1720-1739)
        // ============================================

        /// <summary>Drill state: 0=Idle, 1=Drilling, 2=Transporting, 3=Delivering. Read only.</summary>
        QuarryDrillState = 1720,

        /// <summary>Number of ore items mined and ready for export. Read only.</summary>
        QuarryOreCount = 1721,

        /// <summary>Current drill depth in voxels. Read only.</summary>
        QuarryDepth = 1722,

        /// <summary>Maximum drill depth reached. Read only.</summary>
        QuarryMaxDepth = 1723,

        /// <summary>1 if drill has finished mining, 0 if not. Read only.</summary>
        QuarryIsDrillFinished = 1724,

        /// <summary>1 if transporting ore to output, 0 if not. Read only.</summary>
        QuarryIsTransporting = 1725,

        /// <summary>1 if delivering ore to chute, 0 if not. Read only.</summary>
        QuarryIsDelivering = 1726,

        // ============================================
        // HorizontalQuarry / Ogre (1740-1759)
        // ============================================

        /// <summary>Ogre state: 0=Idle, 1=Mining, 2=Returning, 3=Delivering, 4=AwaitingExport. Read only.</summary>
        OgreState = 1740,

        /// <summary>Number of ore items mined and ready for export. Read only.</summary>
        OgreOreCount = 1741,

        /// <summary>Current horizontal position along mining path. Read only.</summary>
        OgrePosition = 1742,

        /// <summary>1 if mining path complete, 0 if mining in progress. Read only.</summary>
        OgreMiningComplete = 1743,

        /// <summary>1 if ogre is returning to start position, 0 if not. Read only.</summary>
        OgreIsReturning = 1744,

        /// <summary>1 if ore queue is full, 0 if not. Read only.</summary>
        OgreQueueFull = 1745,

        // ============================================
        // Recycler (1760-1779)
        // ============================================

        /// <summary>Total reagent quantity from all processed items. Read only.</summary>
        RecyclerReagentTotal = 1760,

        /// <summary>1 if currently exporting reagents, 0 if not. Read only.</summary>
        RecyclerIsExporting = 1761,

        /// <summary>1 if output is at capacity, 0 if not. Read only.</summary>
        RecyclerAtCapacity = 1762,

        /// <summary>Number of ticks spent idle. Read only.</summary>
        RecyclerIdleTicks = 1763,

        /// <summary>1 if device is currently processing an item, 0 if not. Read only.</summary>
        RecyclerIsProcessing = 1764,

        // ============================================
        // Stirling Engine (1780-1799)
        // ============================================

        /// <summary>Hot side (input) temperature in Kelvin. Read only.</summary>
        StirlingHotTemperature = 1780,

        /// <summary>Cold side (output) temperature in Kelvin. Read only.</summary>
        StirlingColdTemperature = 1781,

        /// <summary>Temperature differential between hot and cold sides. Read only.</summary>
        StirlingTemperatureDelta = 1782,

        /// <summary>Current operating efficiency 0-1. Read only.</summary>
        StirlingEfficiency = 1783,

        /// <summary>Maximum power output at current conditions in watts. Read only.</summary>
        StirlingMaxPower = 1784,

        /// <summary>1 if both atmospheres are connected, 0 if not. Read only.</summary>
        StirlingIsConnected = 1785,

        // ============================================
        // Rocket Miner (1800-1819)
        // ============================================

        /// <summary>Current mining progress 0-1. Read only.</summary>
        RocketMiningProgress = 1800,

        /// <summary>Hash of next ore type to be produced. Read only.</summary>
        RocketNextOreHash = 1801,

        /// <summary>Quantity of ore mined this cycle. Read only.</summary>
        RocketMiningQuantity = 1802,

        /// <summary>1 if rocket miner is actively mining, 0 if not. Read only.</summary>
        RocketIsMining = 1803,

        // ============================================
        // Centrifuge Expansion (1101-1109)
        // ============================================

        /// <summary>Current centrifuge RPM. Read only.</summary>
        CentrifugeRPM = 1101,

        /// <summary>Total reagent quantity in centrifuge. Read only.</summary>
        CentrifugeReagentTotal = 1102,

        /// <summary>1 if lid is closed, 0 if open. Read only.</summary>
        CentrifugeLidClosed = 1103,

        // ============================================
        // Landing Pad Center (1820-1829)
        // ============================================

        /// <summary>Contact status: 0=NoContact, 1=Approaching, 2=WaitingApproach, 3=WaitingDoors, 4=Landed. Read only.</summary>
        PadContactStatus = 1820,

        /// <summary>1 if trader is ready to trade, 0 if not. Read only.</summary>
        PadIsTraderReady = 1821,

        /// <summary>1 if pad has an active contact, 0 if not. Read only.</summary>
        PadHasContact = 1822,

        /// <summary>1 if pad is locked, 0 if not. Read only.</summary>
        PadIsLocked = 1823,

        /// <summary>Virtual waypoint height setting (0-50). Read only.</summary>
        PadWaypointHeight = 1824,

        // ============================================
        // Area Power Controller (1830-1839)
        // ============================================

        /// <summary>Total maximum power (Battery + Network capacity). Read only.</summary>
        APCMaximumPower = 1830,
    }

    /// <summary>
    /// Filter modes for contact filtering.
    /// </summary>
    public enum SLEFilterMode
    {
        /// <summary>No filtering, show all contacts.</summary>
        All = 0,

        /// <summary>Filter by ShuttleType (set FilterValue to desired type).</summary>
        ShuttleType = 1,

        /// <summary>Only show resolved contacts.</summary>
        Resolved = 2,

        /// <summary>Only show unresolved contacts.</summary>
        Unresolved = 3,

        /// <summary>Only show contacted traders.</summary>
        Contacted = 4,

        /// <summary>Only show not-yet-contacted traders.</summary>
        NotContacted = 5,
    }
}
