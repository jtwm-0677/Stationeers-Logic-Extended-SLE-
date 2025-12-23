# Changelog

All notable changes to Stationeers Logic Extended (SLE) will be documented in this file.

## [2.7.1] - 2024-12-23

### Fixed
- Logic Reader hover now displays custom LogicType names correctly
  - Added patches for `EnumCollection<LogicType>.GetName` and `GetNameFromValue`
  - Previously only patched `Enum.GetName` which the game doesn't use for hover UI

---

## [2.7.0] - 2024-12-22

### Added - Landing Pad Center Support (5 LogicTypes)
Fixes vanilla oversight where CanLogicRead returns false despite GetLogicValue supporting Mode/Vertical!

- `PadContactStatus` (1820) - Contact status enum: 0=NoContact, 1=Approaching, 2=WaitingApproach, 3=WaitingDoors, 4=Landed
- `PadIsTraderReady` (1821) - Trader is at pad and ready to trade
- `PadHasContact` (1822) - Pad has an active trading contact
- `PadIsLocked` (1823) - Pad lock state
- `PadWaypointHeight` (1824) - Virtual waypoint height setting (0-50)

### Added - Area Power Controller Support (1 LogicType)
- `APCMaximumPower` (1830) - Total maximum power (Battery + Network capacity combined)
  - Vanilla `Maximum` only returns battery capacity, this returns total system capacity

### Fixed
- Quarry and HorizontalQuarry patches now correctly target DeviceImportExport base class
  - These devices don't override CanLogicRead/GetLogicValue directly
  - Now uses same pattern as Harvester patches

### Changed
- Total LogicTypes: 224 → 230
- Supported device types: 33 → 35

---

## [2.3.1] - 2024-12-18

### Added
- **WindDirection** (1124) - Wind direction in degrees 0-360 for StructureWindTurbine only (tall turbine with rotating head)

### Fixed
- Stationpedia bug where custom LogicTypes incorrectly appeared on unrelated devices (LarreDoc devices showing heater/vent LogicTypes)
- Added type guards to all Harmony patches to prevent LogicTypes from appearing on derived device classes

---

## [2.3.0] - 2024-12-17

### Added - HVAC Device Support
- **Air Conditioner** (6 LogicTypes)
  - `ACEnergyMoved` (1500) - Joules of thermal energy transferred this tick
  - `ACIsFullyConnected` (1501) - All three pipe networks connected
  - `ACPowerUsed` (1502) - Power consumption during operation
  - `ACEfficiency` (1503) - Combined efficiency (TempDiff × OpTemp × Pressure, 0-1)
  - `ACOutputLimit` (1510) - Pressure limit placeholder (read/write)
  - `ACInputLimit` (1511) - Pressure limit placeholder (read/write)

- **Wall Cooler** (4 LogicTypes)
  - `CoolerIsEnvironmentOkay` (1520) - Room atmosphere above Armstrong limit
  - `CoolerIsPipeOkay` (1521) - Pipe atmosphere above Armstrong limit
  - `CoolerPowerUsed` (1522) - Power/energy consumed this tick
  - `CoolerEnergyMoved` (1523) - Joules transferred from room to pipe

- **Wall Heater** (5 LogicTypes)
  - `HeaterIsEnvironmentOkay` (1530) - Room has valid atmosphere (HasOpenGrid)
  - `HeaterIsPipeOkay` (1531) - Returns -1 (N/A - no pipe connection)
  - `HeaterPowerUsed` (1532) - Power consumed this tick
  - `HeaterEnergyTransfer` (1533) - Joules added per tick (constant)
  - `HeaterMaxTemperature` (1534) - Maximum temperature limit (2500K)

- **Active Vent** (3 LogicTypes)
  - `VentFlowStatus` (1540) - FlowIndicatorState enum value (0-6)
  - `VentIsConnected` (1541) - Has valid pipe network connection
  - `VentPowerUsed` (1542) - Power when operating

### Added - Filtration Pressure Limits
- `FiltrationOutputLimit` (1420) - Maximum output pressure in kPa (read/write, 0 = unlimited)
- `FiltrationInputLimit` (1421) - Minimum input pressure in kPa (read/write, 0 = no minimum)

### Added - Stationpedia Improvements
- LogicTypes now display with color-coded labels:
  - **GREEN** - "Revealed" LogicTypes (hidden game data now accessible)
  - **BLUE** - "Added" LogicTypes (new functionality like pressure limits)

### Fixed
- Harmony patch targeting for FiltrationMachineBase (now uses TargetMethod pattern)
- About.xml structure for StationeersLaunchPad compatibility

---

## [2.2.0] - 2024-12-16

### Added - Filtration Machine Support (13 LogicTypes)
- **Machine State**
  - `FilterSlotIndex` (1400) - Select filter slot by index (read/write)
  - `FilterSlotCount` (1401) - Total number of filter slots
  - `HasEmptyFilter` (1402) - Any filter completely depleted
  - `IsFullyConnected` (1403) - All pipe networks connected
  - `FilterPowerUsed` (1404) - Power consumed during filtration
  - `FiltrationProcessedMoles` (1405) - Moles processed this tick

- **Per-Filter Properties** (selected by FilterSlotIndex)
  - `FilterQuantity` (1410) - Filter charge remaining (0.0-1.0)
  - `FilterIsLow` (1411) - Charge at or below 5%
  - `FilterIsEmpty` (1412) - Filter completely empty
  - `FilterTypeHash` (1413) - GasType hash of filtered gas
  - `FilterLife` (1414) - Filter life tier (0-3)
  - `FilterUsedTicks` (1415) - Ticks since last degradation
  - `FilterMaxTicks` (1416) - Max ticks before degradation

### Added
- IC10 named constants for all Filtration LogicTypes
- Stationpedia integration showing LogicTypes on Filtration device pages

---

## [2.1.0] - 2024-12-15

### Added - Furnace Support (20 LogicTypes)
- **Furnace / Advanced Furnace** (12 LogicTypes)
  - `FurnaceTemperature` (1300) - Internal temperature in Kelvin
  - `FurnacePressure` (1301) - Internal pressure in kPa
  - `FurnaceTotalMoles` (1302) - Total gas moles inside
  - `FurnaceInflamed` (1303) - Currently burning/smelting
  - `FurnaceMode` (1304) - Valid recipe detected
  - `ReagentQuantity` (1305) - Total mineral quantity loaded
  - `FurnaceOverpressure` (1306) - Dangerous overpressure state
  - `FurnaceStressed` (1308) - Pressure at 66% warning level
  - `FurnaceHasBlown` (1309) - Furnace has exploded
  - `CurrentRecipeEnergy` (1307) - Energy required for recipe
  - `FurnaceMaxPressure` (1310) - Max safe pressure (60795 kPa)
  - `FurnaceVolume` (1311) - Internal volume in litres

- **Advanced Furnace Additional** (4 LogicTypes)
  - `MinSettingInput` (1320), `MaxSettingInput` (1321)
  - `MinSettingOutput` (1322), `MaxSettingOutput` (1323)

- **Arc Furnace** (4 LogicTypes)
  - `ArcFurnaceActivate` (1330) - Smelting state (readable!)
  - `ImportStackSize` (1331) - Quantity in import slot
  - `SmeltingPower` (1332) - Power during smelting
  - `ArcFurnaceIsSmelting` (1333) - Actively processing

### Added
- Stationpedia integration for Furnace devices

---

## [2.0.2] - 2024-12-14

### Fixed
- Bug fixes and stability improvements
