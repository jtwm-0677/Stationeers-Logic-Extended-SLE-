# Stationeers Logic Extended (SLE)

A BepInEx mod for [Stationeers](https://store.steampowered.com/app/544550/Stationeers/) that significantly expands IC10 programmable circuit capabilities by adding **230 new LogicTypes** across **35 device types**.

## Features

- **230 new LogicTypes** - Access previously hidden device data and new functionality
- **IC10 named constants** - All LogicTypes available as named constants in your IC10 code
- **Stationpedia integration** - LogicTypes appear on device pages with color coding:
  - **GREEN** - "Revealed" LogicTypes (hidden game data now accessible)
  - **BLUE** - "Added" LogicTypes (new functionality)
- **Vanilla-safe** - All custom LogicTypes use values 1000+ to avoid conflicts

## Requirements

1. **[BepInEx 5.4+](https://github.com/BepInEx/BepInEx/releases)** - Extract to your Stationeers game folder
2. **[StationeersLaunchPad](https://github.com/StationeersLaunchPad/StationeersLaunchPad/releases)** - Extract to `BepInEx/plugins`
3. **This mod** - Subscribe on Steam Workshop or download the release

## Installation

### Steam Workshop (Recommended)
Subscribe to the mod on [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3625190467)

### Manual Installation
1. Download the latest release
2. Extract to `Documents/My Games/Stationeers/mods/StationeersLogicExtended/`

## Supported Devices & LogicTypes

### Satellite Dish (19 LogicTypes)
| LogicType | ID | Description |
|-----------|-----|-------------|
| `ContactIndex` | 1000 | Current contact index (r/w) |
| `ContactCount` | 1001 | Number of contacts matching filter |
| `FilterMode` | 1002 | Filter mode (r/w): 0=All, 1=Traders, 2=Asteroids, 3=Rockets, 4=Shuttles |
| `ContactShuttleType` | 1003 | Current contact's shuttle type enum |
| `ContactLifetime` | 1004 | Contact lifetime remaining (seconds) |
| `ContactResolved` | 1005 | Contact is resolved (bool) |
| `ContactContacted` | 1006 | Contact has been contacted (bool) |
| `DishWattageOnContact` | 1007 | Power consumption when contacting |
| `DishIsInterrogating` | 1008 | Currently interrogating contact (bool) |
| `ContactPosition` | 1010 | Contact Position value |
| `ContactVelocity` | 1011 | Contact Velocity value |
| `ContactDistance` | 1012 | Contact Distance in meters |
| `TotalContactCount` | 1013 | Total unfiltered contact count |
| `DishIsResolved` | 1014 | Current contact is resolved (bool) |
| `DishIsContacted` | 1015 | Current contact has been contacted (bool) |
| `ContactHash` | 1020 | Contact's prefab hash |
| `ContactWorldId` | 1021 | Contact's world ID |
| `ContactDrillQuantity` | 1022 | Contact's drill quantity |
| `ContactCurrentFuel` | 1023 | Contact's current fuel |

### Daylight Sensor (8 LogicTypes)
| LogicType | ID | Description |
|-----------|-----|-------------|
| `TimeOfDay` | 1110 | Current time of day (0.0-1.0) |
| `IsEclipse` | 1111 | Currently in eclipse (bool) |
| `EclipseRatio` | 1112 | Eclipse progress (0.0-1.0) |
| `DaysPast` | 1113 | Total days elapsed |
| `DayLengthSeconds` | 1114 | Day length in seconds |
| `Latitude` | 1115 | Sensor latitude |
| `Longitude` | 1116 | Sensor longitude |
| `WeatherSolarRatio` | 1117 | Solar ratio affected by weather |

### Pipe Analyzer (26 LogicTypes)
Includes partial pressures for all 9 gas types, total moles, combustion data, and pipe stress monitoring:

| LogicType | ID | Description |
|-----------|-----|-------------|
| `PPOxygen` | 1250 | Partial pressure - Oxygen |
| `PPNitrogen` | 1251 | Partial pressure - Nitrogen |
| `PPCO2` | 1252 | Partial pressure - CO2 |
| `PPVolatiles` | 1253 | Partial pressure - Volatiles |
| `PPPollutant` | 1254 | Partial pressure - Pollutant |
| `PPWater` | 1255 | Partial pressure - Water |
| `PPNO2` | 1256 | Partial pressure - NO2 |
| `PPN2O` | 1257 | Partial pressure - N2O |
| `PPSteam` | 1258 | Partial pressure - Steam |
| `TotalMolesGasses` | 1260 | Total gas moles |
| `TotalMolesLiquids` | 1261 | Total liquid moles |
| `CombustionLemonite` | 1270 | Combustion - Lemonite |
| `CombustionIron` | 1271 | Combustion - Iron |
| `CombustionSilver` | 1272 | Combustion - Silver |
| `CombustionGold` | 1273 | Combustion - Gold |
| `CombustionCopper` | 1274 | Combustion - Copper |
| `CombustionCoal` | 1275 | Combustion - Coal |
| `CombustionUranium` | 1276 | Combustion - Uranium |
| `CombustionSteel` | 1277 | Combustion - Steel |
| `PipeStressRatio` | 1280 | Pipe stress ratio (0.0-1.0) |
| `PipeIsStressed` | 1281 | Pipe is stressed (bool) |
| `PipeDamageRatio` | 1282 | Pipe damage ratio |
| `PipeIsBurst` | 1283 | Pipe has burst (bool) |
| `PipeDamageType` | 1284 | Pipe damage type enum |
| `NetworkHasFault` | 1285 | Network has any fault (bool) |
| `NetworkWorstDamage` | 1286 | Worst damage ratio in network |

### Furnace / Advanced Furnace (16 LogicTypes)
| LogicType | ID | Description |
|-----------|-----|-------------|
| `FurnaceTemperature` | 1300 | Internal temperature (K) |
| `FurnacePressure` | 1301 | Internal pressure (kPa) |
| `FurnaceTotalMoles` | 1302 | Total gas moles inside |
| `FurnaceInflamed` | 1303 | Currently burning (bool) |
| `FurnaceMode` | 1304 | Valid recipe detected (bool) |
| `ReagentQuantity` | 1305 | Total mineral quantity |
| `FurnaceOverpressure` | 1306 | Dangerous pressure (bool) |
| `CurrentRecipeEnergy` | 1307 | Recipe energy required |
| `FurnaceStressed` | 1308 | 66% pressure warning (bool) |
| `FurnaceHasBlown` | 1309 | Has exploded (bool) |
| `FurnaceMaxPressure` | 1310 | Max safe pressure |
| `FurnaceVolume` | 1311 | Internal volume (L) |
| `MinSettingInput` | 1320 | Advanced: Min input setting |
| `MaxSettingInput` | 1321 | Advanced: Max input setting |
| `MinSettingOutput` | 1322 | Advanced: Min output setting |
| `MaxSettingOutput` | 1323 | Advanced: Max output setting |

### Arc Furnace (4 LogicTypes)
| LogicType | ID | Description |
|-----------|-----|-------------|
| `ArcFurnaceActivate` | 1330 | Smelting state (readable!) |
| `ImportStackSize` | 1331 | Import slot quantity |
| `SmeltingPower` | 1332 | Power during smelting |
| `ArcFurnaceIsSmelting` | 1333 | Actively processing (bool) |

### Filtration Machine (15 LogicTypes)
| LogicType | ID | Description |
|-----------|-----|-------------|
| `FilterSlotIndex` | 1400 | Current filter slot (r/w) |
| `FilterSlotCount` | 1401 | Total filter slots |
| `HasEmptyFilter` | 1402 | Any filter depleted (bool) |
| `IsFullyConnected` | 1403 | All pipes connected (bool) |
| `FilterPowerUsed` | 1404 | Power consumption |
| `FiltrationProcessedMoles` | 1405 | Moles processed/tick |
| `FilterQuantity` | 1410 | Filter charge (0.0-1.0) |
| `FilterIsLow` | 1411 | At or below 5% (bool) |
| `FilterIsEmpty` | 1412 | Completely empty (bool) |
| `FilterTypeHash` | 1413 | Filtered gas hash |
| `FilterLife` | 1414 | Filter life tier (0-3) |
| `FilterUsedTicks` | 1415 | Ticks since degradation |
| `FilterMaxTicks` | 1416 | Max ticks before degrade |
| `FiltrationOutputLimit` | 1420 | Max output pressure (r/w) |
| `FiltrationInputLimit` | 1421 | Min input pressure (r/w) |

### HVAC Devices

**Air Conditioner (6 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `ACEnergyMoved` | 1500 | Joules transferred/tick |
| `ACIsFullyConnected` | 1501 | All networks connected (bool) |
| `ACPowerUsed` | 1502 | Power consumption |
| `ACEfficiency` | 1503 | Combined efficiency (0.0-1.0) |
| `ACOutputLimit` | 1510 | Output pressure limit (r/w) |
| `ACInputLimit` | 1511 | Input pressure limit (r/w) |

**Wall Cooler (4 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `CoolerIsEnvironmentOkay` | 1520 | Room above Armstrong limit (bool) |
| `CoolerIsPipeOkay` | 1521 | Pipe above Armstrong limit (bool) |
| `CoolerPowerUsed` | 1522 | Power/energy consumed |
| `CoolerEnergyMoved` | 1523 | Joules room→pipe |

**Wall Heater (5 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `HeaterIsEnvironmentOkay` | 1530 | Room has atmosphere (bool) |
| `HeaterIsPipeOkay` | 1531 | Returns -1 (N/A) |
| `HeaterPowerUsed` | 1532 | Power consumed |
| `HeaterEnergyTransfer` | 1533 | Joules added/tick |
| `HeaterMaxTemperature` | 1534 | Max temp limit (2500K) |

**Active Vent (3 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `VentFlowStatus` | 1540 | Flow indicator (0-6) |
| `VentIsConnected` | 1541 | Has pipe network (bool) |
| `VentPowerUsed` | 1542 | Power when operating |

### Power Devices

**Battery (6 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `PowerDelta` | 1210 | Net power flow |
| `BatteryIsSubmerged` | 1211 | In water (bool) |
| `InputSubmergedTicks` | 1212 | Input submerged time |
| `OutputSubmergedTicks` | 1213 | Output submerged time |
| `BatteryIsEmpty` | 1214 | Charge = 0 (bool) |
| `BatteryIsCharged` | 1215 | Fully charged (bool) |

**Solar Panel (9 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `SolarVisibility` | 1220 | Sun visibility |
| `SolarDamageRatio` | 1221 | Damage ratio |
| `SolarDamageTotal` | 1222 | Total damage |
| `SolarHealth` | 1223 | Health remaining |
| `SolarEfficiency` | 1224 | Current efficiency |
| `SolarIsOperable` | 1225 | Can operate (bool) |
| `SolarIsBroken` | 1226 | Is broken (bool) |
| `SolarMovementSpeedH` | 1227 | Horizontal tracking speed |
| `SolarMovementSpeedV` | 1228 | Vertical tracking speed |

**Landing Pad Center (5 LogicTypes)**
*Fixes vanilla bug where CanLogicRead returns false despite GetLogicValue supporting Mode/Vertical!*
| LogicType | ID | Description |
|-----------|-----|-------------|
| `PadContactStatus` | 1820 | Contact enum: 0=None, 1=Approaching, 2=WaitingApproach, 3=WaitingDoors, 4=Landed |
| `PadIsTraderReady` | 1821 | Trader ready to trade (bool) |
| `PadHasContact` | 1822 | Has active contact (bool) |
| `PadIsLocked` | 1823 | Pad locked (bool) |
| `PadWaypointHeight` | 1824 | Virtual waypoint height (0-50) |

**Area Power Controller (1 LogicType)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `APCMaximumPower` | 1830 | Total capacity (Battery + Network) |

### Processing & Fabrication

**Fabricator/Printer (6 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `FabricatorCurrentIndex` | 1600 | Current recipe index |
| `FabricatorRecipeCount` | 1601 | Total recipes |
| `FabricatorCurrentTier` | 1602 | Current recipe tier |
| `FabricatorTimeMultiplier` | 1603 | Time multiplier |
| `FabricatorPowerUsed` | 1604 | Power consumption |
| `FabricatorMakingIndex` | 1605 | Currently making index |

**Advanced Composter (8 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `ComposterGrindProgress` | 1610 | Grind progress (0.0-1.0) |
| `ComposterBatchProgress` | 1611 | Batch progress (0.0-1.0) |
| `ComposterDecayCount` | 1612 | Decayed item count |
| `ComposterNormalCount` | 1613 | Normal item count |
| `ComposterBiomassCount` | 1614 | Biomass count |
| `ComposterPowerUsed` | 1615 | Power consumption |
| `ComposterCanProcess` | 1616 | Can process (bool) |
| `ComposterIsOperable` | 1617 | Is operable (bool) |

**Recycler (5 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `RecyclerReagentTotal` | 1760 | Total reagents extracted |
| `RecyclerIsExporting` | 1761 | Currently exporting (bool) |
| `RecyclerAtCapacity` | 1762 | At output capacity (bool) |
| `RecyclerIdleTicks` | 1763 | Idle tick count |
| `RecyclerIsProcessing` | 1764 | Processing items (bool) |

### Mining Devices

**AIMeE Bot (8 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `RobotIsStorageEmpty` | 1700 | Storage empty (bool) |
| `RobotIsStorageFull` | 1701 | Storage full (bool) |
| `RobotIsOperable` | 1702 | Is operable (bool) |
| `RobotBatteryRatio` | 1703 | Battery level (0.0-1.0) |
| `RobotIsBusy` | 1704 | Currently busy (bool) |
| `RobotDamageRatio` | 1705 | Damage level |
| `RobotStorageCount` | 1706 | Items in storage |
| `RobotStorageCapacity` | 1707 | Max storage capacity |

**Autominer Small (7 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `QuarryDrillState` | 1720 | Drill state enum |
| `QuarryOreCount` | 1721 | Ore mined count |
| `QuarryDepth` | 1722 | Current depth |
| `QuarryMaxDepth` | 1723 | Maximum depth |
| `QuarryIsDrillFinished` | 1724 | Drilling complete (bool) |
| `QuarryIsTransporting` | 1725 | Transporting ore (bool) |
| `QuarryIsDelivering` | 1726 | Delivering ore (bool) |

**Ogre / HorizontalQuarry (6 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `OgreState` | 1740 | Mining state enum |
| `OgreOreCount` | 1741 | Ore queue count |
| `OgrePosition` | 1742 | Current position |
| `OgreMiningComplete` | 1743 | Mining complete (bool) |
| `OgreIsReturning` | 1744 | Returning home (bool) |
| `OgreQueueFull` | 1745 | Queue full (bool) |

**Rocket Miner (4 LogicTypes)**
| LogicType | ID | Description |
|-----------|-----|-------------|
| `RocketMiningProgress` | 1800 | Mining progress (0.0-1.0) |
| `RocketNextOreHash` | 1801 | Next ore prefab hash |
| `RocketMiningQuantity` | 1802 | Mining quantity |
| `RocketIsMining` | 1803 | Currently mining (bool) |

### Additional Devices
- **Wind Turbine** (4 LogicTypes) - Wind direction, efficiency data
- **Hydroponics** (10 LogicTypes) - Light exposure, growth efficiency, water content
- **Harvester** (10 LogicTypes) - Arm state, planting status, fertilizer cycles
- **Centrifuge** (4 LogicTypes) - Processing state data
- **H2 Combustor** (4 LogicTypes) - Processed moles, power usage
- **Electrolyzer** (4 LogicTypes) - Processed moles, power usage
- **Chambers** (5 LogicTypes) - Energy transfer, liquid ratio
- **Stirling Engine** (6 LogicTypes) - Temperature data, efficiency

## LogicType Ranges

| Range | Device Category |
|-------|-----------------|
| 1000-1039 | Satellite Dish |
| 1100-1103 | Centrifuge |
| 1110-1119 | Daylight Sensor |
| 1120-1159 | Power Generation |
| 1160-1199 | Farming |
| 1210-1228 | Battery/Solar |
| 1230-1249 | Electrolysis |
| 1250-1292 | Pipe Analyzer |
| 1300-1339 | Furnaces |
| 1400-1421 | Filtration |
| 1500-1549 | HVAC |
| 1600-1629 | Processing |
| 1700-1707 | Robot |
| 1720-1726 | Quarry |
| 1740-1745 | Ogre |
| 1760-1764 | Recycler |
| 1780-1785 | Stirling |
| 1800-1803 | Rocket Miner |
| 1820-1824 | Landing Pad |
| 1830+ | APC |

## Example IC10 Code

```
# Read satellite dish contact information
alias dish d0
l r0 dish ContactCount      # Get number of contacts
l r1 dish ContactIndex      # Get current index
l r2 dish ContactDistance   # Get contact distance

# Monitor furnace status
alias furnace d1
l r3 furnace FurnaceTemperature
l r4 furnace FurnacePressure
l r5 furnace FurnaceStressed
bgtz r5 FurnaceWarning

# Check battery status
alias battery d2
l r6 battery BatteryIsCharged
l r7 battery PowerDelta

# Monitor composter progress
alias composter d3
l r8 composter ComposterBatchProgress

# Check landing pad for traders
alias pad d4
l r9 pad PadIsTraderReady
l r10 pad PadContactStatus
```

## Building from Source

1. Clone this repository
2. Ensure you have the game's `Assembly-CSharp.dll` referenced
3. Build with `dotnet build --configuration Release`
4. Copy `plugins/StationeersLogicExtended.dll` to your mod folder

## Contributing

Suggestions for new LogicTypes are welcome! Please open an issue to discuss before implementing.

When proposing new LogicTypes:
- Verify the data isn't already exposed through vanilla LogicTypes
- Ensure the data provides meaningful value to IC10 automation
- Use values in the 1000+ range to avoid vanilla conflicts

## License

MIT License - See LICENSE file for details.

## Credits

- **SLE Team** - Development
- **RocketWerkz** - Stationeers game
- **BepInEx Team** - Modding framework
- **HarmonyLib** - Runtime patching library
