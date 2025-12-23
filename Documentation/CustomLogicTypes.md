# Stationeers Logic Extended - Custom LogicTypes

This document lists all custom LogicTypes added by the SLE mod, organized by device.

All LogicType names use **CamelCase** to match vanilla Stationeers conventions.

---

## SatelliteDish

The SatelliteDish receives 19 new LogicTypes for advanced contact management and automation.

### Contact Selection & Filtering

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| ContactIndex | 1000 | read-write | int | Select contact by index (0-based). Set this to choose which contact's properties to read. |
| ContactCount | 1001 | read | int | Total number of visible contacts (before filtering). |
| FilterMode | 1002 | read-write | int | Filter type for contact list. See FilterMode values below. |
| FilterValue | 1003 | read-write | int | Filter parameter value. Used when FilterMode requires a value (e.g., ShuttleType). |
| FilteredCount | 1004 | read | int | Count of contacts matching the current filter criteria. |

#### FilterMode Values

| Value | Mode | Description |
|-------|------|-------------|
| 0 | All | No filtering, show all contacts |
| 1 | ShuttleType | Filter by ShuttleType (use FilterValue to specify type) |
| 2 | Resolved | Show only resolved contacts |
| 3 | Unresolved | Show only unresolved contacts |
| 4 | Contacted | Show only contacted traders |
| 5 | NotContacted | Show only traders not yet contacted |

### Selected Contact Properties

These LogicTypes return information about the contact at the current ContactIndex.

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| ContactShuttleType | 1010 | read | int | ShuttleType enum value. See ShuttleType values below. |
| ContactLifetime | 1011 | read | float | Seconds remaining until contact leaves range. |
| ContactDegreeOffset | 1012 | read | float | Alignment angle in degrees. Lower values = better aligned with dish. |
| ContactResolved | 1013 | read | bool | 1 if contact is resolved, 0 if not. |
| ContactContacted | 1014 | read | bool | 1 if trader has been contacted, 0 if not. |
| ContactResolutionProgress | 1015 | read | float | Resolution progress from 0.0 to 1.0. |
| ContactMinWattsResolve | 1016 | read | float | Minimum watts required to resolve this contact. |
| ContactMinWattsContact | 1017 | read | float | Minimum watts required to contact this trader. |
| ContactSecondsToContact | 1018 | read | float | Seconds required to establish contact with trader. |
| ContactTraderHash | 1019 | read | int | Trader type hash (same as game's ContactTypeId). |
| ContactReferenceId | 1020 | read | long | Unique reference ID of the contact. |

#### ShuttleType Values

| Value | Type |
|-------|------|
| 0 | None |
| 1 | Small |
| 2 | SmallGas |
| 3 | Medium |
| 4 | MediumGas |
| 5 | Large |
| 6 | LargeGas |
| 7 | MediumPlane |
| 8 | LargePlane |

### Dish State

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| DishWattageOnContact | 1030 | read | float | Actual watts reaching the currently selected contact. |
| DishIsInterrogating | 1031 | read | bool | 1 if dish is currently interrogating a contact, 0 if not. |
| DishInterrogatingId | 1032 | read | long | ReferenceId of contact being interrogated, 0 if none. |

---

## Example IC10 Code

### Read all contacts and find unresolved ones

```
alias dish d0
alias count r0
alias index r1
alias resolved r2

# Get total contact count
l count dish 1001  # ContactCount

# Loop through contacts
move index 0
loop:
    # Set contact index
    s dish 1000 index  # ContactIndex

    # Check if resolved
    l resolved dish 1013  # ContactResolved
    beqz resolved found_unresolved

    # Next contact
    add index index 1
    blt index count loop

    # All resolved
    j done

found_unresolved:
    # index now contains the unresolved contact
    # Do something with it...

done:
```

### Filter by ShuttleType

```
alias dish d0

# Set filter to ShuttleType mode
s dish 1002 1  # FilterMode = ShuttleType

# Filter for Large shuttles only
s dish 1003 5  # FilterValue = Large

# Get count of large shuttles
l r0 dish 1004  # FilteredCount
```

---

## Combustion Centrifuge

The CombustionCentrifuge receives a new LogicType for processing progress.

> **Note:** The regular Centrifuge has no logic support in the vanilla game and cannot be patched without more invasive changes. Only CombustionCentrifuge is supported.

The CombustionCentrifuge already has Rpm, Stress, Throttle, and CombustionLimiter. SLE adds:

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| CentrifugeProcessing | 1100 | read | int | Processing progress 0-100%. |

### Example IC10 Code

```
alias centrifuge d0
alias progress r0
alias rpm r1

# Read current RPM
l rpm centrifuge Rpm

# Read processing progress (0-100)
l progress centrifuge 1100  # CentrifugeProcessing

# Wait until processing complete
loop:
    l progress centrifuge 1100
    blt progress 100 loop
    # Processing complete!
```

---

---

## DaylightSensor (Realtime World Data)

The DaylightSensor receives new LogicTypes for accessing realtime world data.

The DaylightSensor already supports vanilla: Horizontal, Vertical, SolarAngle, SolarIrradiance, Activate. SLE adds:

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| TimeOfDay | 1110 | read | float | Time of day 0-1 (0=sunrise, 0.25=noon, 0.5=sunset, 0.75=midnight). |
| IsEclipse | 1111 | read | bool | 1 if eclipse is occurring, 0 if not. |
| EclipseRatio | 1112 | read | float | Eclipse intensity 0.0-1.0 (0=no eclipse, 1=full). |
| DaysPast | 1113 | read | int | Number of days since world creation. |
| DayLengthSeconds | 1114 | read | int | Length of a day in seconds. |
| Latitude | 1115 | read | float | World latitude in degrees. |
| Longitude | 1116 | read | float | World longitude in degrees. |
| WeatherSolarRatio | 1117 | read | float | Weather solar ratio 0-1 (1=full sun, lower during storms). |

### Example IC10 Code

```
alias sensor d0
alias timeOfDay r0
alias dayCount r1

# Read time of day (0-360 degrees)
l timeOfDay sensor 1110  # TimeOfDay

# Check if it's daytime (roughly 90-270 degrees)
blt timeOfDay 90 nighttime
bgt timeOfDay 270 nighttime
# It's daytime
j daytime

nighttime:
    # Handle night operations
    j done

daytime:
    # Handle day operations

done:

# Read days since world start
l dayCount sensor 1113  # DaysPast
```

### Weather-aware solar tracking

```
alias sensor d0
alias solarRatio r0
alias weatherRatio r1
alias effectiveSolar r2

# Read solar irradiance (vanilla)
l solarRatio sensor SolarIrradiance

# Read weather impact on solar (1=clear, lower during storms)
l weatherRatio sensor 1117  # WeatherSolarRatio

# Calculate effective solar output
mul effectiveSolar solarRatio weatherRatio
```

### Read world position

```
alias sensor d0
alias lat r0
alias lon r1
alias dayLength r2

# Read world coordinates
l lat sensor 1115  # Latitude
l lon sensor 1116  # Longitude

# Read day length in seconds
l dayLength sensor 1114  # DayLengthSeconds
```

---

## WindTurbineGenerator

Both the regular WindTurbineGenerator (small) and LargeWindTurbineGenerator receive new LogicTypes.

The WindTurbineGenerator already supports vanilla: PowerGeneration. SLE adds:

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| WindSpeed | 1120 | read | float | Current global wind strength 0-1. Same for all turbines. |
| MaxPower | 1121 | read | float | Current max power output in watts (storm-aware). |
| TurbineSpeed | 1122 | read | float | Current turbine blade rotation speed 0-1. |
| AtmosphericPressure | 1123 | read | float | Clamped atmospheric pressure in kPa. |

### MaxPower Values

MaxPower returns the appropriate maximum based on current weather:

| Turbine Type | Normal Max | Storm Max |
|--------------|------------|-----------|
| WindTurbineGenerator (small) | 500 W | 1000 W |
| LargeWindTurbineGenerator | 1000 W | 20000 W |

### Example IC10 Code

```
alias turbine d0
alias windSpeed r0
alias maxPower r1
alias currentPower r2
alias efficiency r3

# Read wind speed (0-1)
l windSpeed turbine 1120  # WindSpeed

# Read current max power (storm-aware)
l maxPower turbine 1121  # MaxPower

# Read actual power generation
l currentPower turbine PowerGeneration

# Calculate efficiency percentage
div efficiency currentPower maxPower
mul efficiency efficiency 100
```

### Monitor for storms

```
alias turbine d0
alias maxPower r0
alias normalMax r1

define NormalMaxSmall 500
define NormalMaxLarge 1000

# Read current max power
l maxPower turbine 1121  # MaxPower

# Check if storm is active (max > normal)
# For small turbine:
bgt maxPower NormalMaxSmall storm_active
j normal_weather

storm_active:
    # Storm detected! Max power increased
    # Take advantage of higher output

normal_weather:
    # Normal operations
```

---

## SolidFuelGenerator

The SolidFuelGenerator (solid fuel generator) receives new LogicTypes for fuel monitoring.

The SolidFuelGenerator already supports vanilla: PowerGeneration, ImportCount, ClearMemory (write). SLE adds:

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| FuelTicks | 1130 | read | int | Fuel buffer remaining in ticks. Decrements each tick while generating. |

### Understanding FuelTicks

The SolidFuelGenerator maintains an internal fuel buffer measured in "ticks":
- Each atmospheric tick while generating, `FuelTicks` decreases by 1
- When ore is consumed, `FuelTicks` increases based on the ore's energy value
- When `FuelTicks` reaches 0, power generation stops
- Use this to predict when to feed more ore before the generator runs dry

### Example IC10 Code

```
alias generator d0
alias fuelTicks r0
alias lowFuelThreshold r1

define LowFuelWarning 10  # Warn when less than 10 ticks remain

# Read current fuel buffer
l fuelTicks generator 1130  # FuelTicks

# Check if fuel is running low
slt r2 fuelTicks LowFuelWarning
beqz r2 fuel_ok

# Fuel low - trigger ore feed or warning
# (activate chute, sorter, or warning light)

fuel_ok:
    # Normal operations
```

### Auto-feed example

```
alias generator d0
alias chute d1
alias fuelTicks r0

define RefillThreshold 5

loop:
    # Check fuel level
    l fuelTicks generator 1130  # FuelTicks

    # Enable chute when fuel low
    slt r1 fuelTicks RefillThreshold
    s chute On r1

    yield
    j loop
```

---

## WeatherStation

The WeatherStation receives new LogicTypes for weather event data.

The WeatherStation already supports vanilla LogicTypes for current conditions. SLE adds:

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| WeatherWindStrength | 1140 | read | float | Current weather event wind strength 0-1 (0 when no event). |
| DaysSinceLastWeatherEvent | 1141 | read | float | Days since the last weather event ended. |

### Example IC10 Code

```
alias station d0
alias windStrength r0
alias daysSince r1

# Read current weather wind strength
l windStrength station 1140  # WeatherWindStrength

# Check if a storm is active (wind > 0)
bgtz windStrength storm_active
j no_storm

storm_active:
    # Storm in progress, take precautions
    # (retract solar panels, secure equipment)

no_storm:
    # Read days since last weather event
    l daysSince station 1141  # DaysSinceLastWeatherEvent
```

### Predict weather patterns

```
alias station d0
alias daysSince r0

define TypicalStormInterval 3  # Storms typically every 3 days

# Check days since last weather
l daysSince station 1141  # DaysSinceLastWeatherEvent

# Warn if storm might be due soon
sgt r1 daysSince TypicalStormInterval
beqz r1 all_clear

# Storm may be imminent - prepare
# (activate warning lights, etc.)

all_clear:
    # Normal operations
```

---

## DeepMiner

The DeepMiner receives new LogicTypes for mining progress and ore type.

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| MiningProgress | 1150 | read | float | Mining cycle progress 0-100%. |
| CurrentOreHash | 1151 | read | int | Hash of current ore type in export slot (0 if empty). |

### Example IC10 Code

```
alias miner d0
alias progress r0
alias oreHash r1

# Read mining progress (0-100%)
l progress miner 1150  # MiningProgress

# Check if mining cycle is complete
blt progress 100 still_mining

# Mining complete - check what ore was produced
l oreHash miner 1151  # CurrentOreHash

still_mining:
    # Wait for mining to complete
```

### Monitor ore production

```
alias miner d0
alias display d1
alias lastHash r0
alias currentHash r1

loop:
    # Check current ore in export slot
    l currentHash miner 1151  # CurrentOreHash

    # If ore changed (new ore produced)
    beq currentHash lastHash no_change
    move lastHash currentHash

    # Display the ore hash on a console
    s display Setting currentHash

no_change:
    yield
    j loop
```

---

## Hydroponics Device

The Hydroponics Device (HydroponicsTrayDevice) receives new LogicTypes for light, water, and plant status monitoring.

The device already supports vanilla LogicTypes for atmosphere and slot data. SLE adds:

### Environment Data

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| LightExposure | 1160 | read | float | Current light exposure level (grow light 0.8 + solar contribution). |
| IsLitByGrowLight | 1161 | read | bool | 1 if lit by a powered grow light, 0 if not. |
| WaterMoles | 1162 | read | float | Water amount in internal atmosphere (moles). |

### Plant Status

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| PlantIsFertilized | 1163 | read | bool | 1 if plant has been fertilized, 0 if not. |
| PlantGrowthEfficiency | 1164 | read | int | Plant overall growth efficiency 0-100%. |
| BreathingEfficiency | 1165 | read | int | Plant breathing/gas efficiency 0-100%. |
| TemperatureEfficiency | 1166 | read | int | Plant temperature efficiency 0-100%. |
| PlantLightEfficiency | 1167 | read | int | Plant light efficiency 0-100%. |
| PlantPressureEfficiency | 1168 | read | int | Plant pressure efficiency 0-100%. |
| HydrationEfficiency | 1169 | read | int | Plant hydration/water efficiency 0-100%. |

### Example IC10 Code

```
alias hydro d0
alias light r0
alias hasGrowLight r1
alias water r2

# Read current light exposure
l light hydro 1160  # LightExposure

# Check if grow light is providing light
l hasGrowLight hydro 1161  # IsLitByGrowLight

# Read water level
l water hydro 1162  # WaterMoles
```

### Monitor light levels for plant health

```
alias hydro d0
alias growLight d1
alias light r0

define MinLight 0.5  # Minimum light needed

loop:
    # Check current light exposure
    l light hydro 1160  # LightExposure

    # Turn on grow light if natural light insufficient
    slt r1 light MinLight
    s growLight On r1

    yield
    j loop
```

### Water level monitoring

```
alias hydro d0
alias waterPump d1
alias water r0

define MinWater 10  # Minimum moles of water

loop:
    # Check water level
    l water hydro 1162  # WaterMoles

    # Enable water pump if water is low
    slt r1 water MinWater
    s waterPump On r1

    yield
    j loop
```

### Monitor plant efficiency breakdown

```
alias hydro d0
alias display d1
alias growthEff r0
alias breathEff r1
alias tempEff r2
alias lightEff r3

loop:
    # Read all efficiency values
    l growthEff hydro 1164  # PlantGrowthEfficiency
    l breathEff hydro 1165  # BreathingEfficiency
    l tempEff hydro 1166    # TemperatureEfficiency
    l lightEff hydro 1167   # PlantLightEfficiency

    # Find the lowest efficiency (limiting factor)
    min r4 breathEff tempEff
    min r4 r4 lightEff

    # Display limiting factor
    s display Setting r4

    yield
    j loop
```

---

## Harvester

The Harvester receives new LogicTypes for operational state, import slot, and fertilizer data from the tray below.

### Harvester State

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| HasTray | 1180 | read | bool | 1 if harvester is positioned over a hydroponics tray, 0 if not. |
| IsHarvesting | 1181 | read | bool | 1 if currently performing harvest operation, 0 if not. |
| IsPlanting | 1182 | read | bool | 1 if currently performing plant operation, 0 if not. |
| ArmState | 1183 | read | int | Arm state: 0=Idle, 1=Planting, 2=Harvesting. |

### Import Slot

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| HasImportPlant | 1184 | read | bool | 1 if plant/seed is ready in import slot, 0 if not. |
| ImportPlantHash | 1185 | read | int | PrefabHash of plant/seed in import slot. |

### Fertilizer Data (from tray below)

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| HasFertilizer | 1186 | read | bool | 1 if fertilizer is in tray's fertilizer slot, 0 if not. |
| FertilizerCycles | 1187 | read | float | Remaining fertilizer cycles. |
| FertilizerHarvestBoost | 1188 | read | float | Fertilizer harvest yield multiplier. |
| FertilizerGrowthSpeed | 1189 | read | float | Fertilizer growth speed multiplier. |

### Example IC10 Code

```
alias harvester d0
alias hasTray r0
alias armState r1
alias hasPlant r2

# Check if harvester is over a tray
l hasTray harvester 1180  # HasTray
beqz hasTray no_tray

# Check arm state
l armState harvester 1183  # ArmState
# 0 = Idle, 1 = Planting, 2 = Harvesting

# Check if plant in import slot
l hasPlant harvester 1184  # HasImportPlant

no_tray:
    # Handle no tray case
```

### Auto-plant when seed arrives

```
alias harvester d0
alias hasPlant r0
alias armState r1

loop:
    # Wait for plant in import slot
    l hasPlant harvester 1184  # HasImportPlant
    beqz hasPlant loop

    # Wait for arm to be idle
    l armState harvester 1183  # ArmState
    bnez armState loop

    # Trigger planting
    s harvester Plant 1

    yield
    j loop
```

### Monitor fertilizer levels

```
alias harvester d0
alias display d1
alias hasFert r0
alias cycles r1
alias boost r2
alias speed r3

# Check if fertilizer present
l hasFert harvester 1186   # HasFertilizer
beqz hasFert no_fertilizer

# Read fertilizer stats
l cycles harvester 1187    # FertilizerCycles
l boost harvester 1188     # FertilizerHarvestBoost
l speed harvester 1189     # FertilizerGrowthSpeed

# Display remaining cycles
s display Setting cycles
j done

no_fertilizer:
    s display Setting 0

done:
```

---

## GasFuelGenerator

The GasFuelGenerator (pipe-based gas-burning generator) receives new LogicTypes for combustion and operational monitoring.

The GasFuelGenerator already supports vanilla LogicTypes like PowerGeneration. SLE adds:

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| CombustionEnergy | 1200 | read | float | Combustion energy produced (before 17% efficiency conversion). |
| IsValidAtmosphere | 1201 | read | bool | 1 if atmosphere meets pressure/temp requirements, 0 if not. |
| DoShutdown | 1202 | read | bool | 1 if conditions will trigger shutdown, 0 if not. |
| MinTemperature | 1203 | read | float | Minimum operating temperature in Kelvin. |
| MaxTemperature | 1204 | read | float | Maximum operating temperature in Kelvin. |
| MinPressure | 1205 | read | float | Minimum operating pressure in Pa. |

### Understanding Efficiency

The GasFuelGenerator has a fixed 17% efficiency. The actual power output is:
`PowerGeneration = CombustionEnergy × 0.17`

While you can't change this efficiency, exposing `CombustionEnergy` lets you:
- Optimize fuel mixtures to maximize combustion energy
- Monitor combustion efficiency before the 17% conversion
- Compare different gas ratios for best output

### Example IC10 Code

```
alias generator d0
alias combustion r0
alias power r1
alias valid r2

# Read combustion energy (pre-efficiency)
l combustion generator 1200  # CombustionEnergy

# Read actual power output
l power generator PowerGeneration

# Check if atmosphere is valid
l valid generator 1201  # IsValidAtmosphere
beqz valid atmosphere_error

# Generator running normally

atmosphere_error:
    # Handle invalid atmosphere
```

### Monitor operating conditions

```
alias generator d0
alias minTemp r0
alias maxTemp r1
alias minPressure r2
alias doShutdown r3

# Read operating limits
l minTemp generator 1203     # MinTemperature (Kelvin)
l maxTemp generator 1204     # MaxTemperature (Kelvin)
l minPressure generator 1205 # MinPressure (Pa)

# Check if shutdown will occur
l doShutdown generator 1202  # DoShutdown
bgtz doShutdown shutdown_warning

# Normal operation

shutdown_warning:
    # Generator is about to shut down
    # (atmosphere out of range)
```

### Optimize fuel mixture

```
alias generator d0
alias display d1
alias combustion r0
alias lastCombustion r1
alias improvement r2

loop:
    # Read current combustion energy
    l combustion generator 1200  # CombustionEnergy

    # Calculate improvement from last reading
    sub improvement combustion lastCombustion
    move lastCombustion combustion

    # Display combustion energy for tuning
    s display Setting combustion

    yield
    j loop
```

---

## PipeAnalyzer

The PipeAnalyzer (StructurePipeAnalysizer) receives new LogicTypes for detailed gas analysis, partial pressures, combustion data, and stress monitoring.

### Partial Pressures

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| PartialPressureO2 | 1250 | read | float | Partial pressure of Oxygen in kPa. |
| PartialPressureCO2 | 1251 | read | float | Partial pressure of Carbon Dioxide in kPa. |
| PartialPressureN2 | 1252 | read | float | Partial pressure of Nitrogen in kPa. |
| PartialPressureVolatiles | 1253 | read | float | Partial pressure of Volatiles in kPa. |
| PartialPressureN2O | 1254 | read | float | Partial pressure of Nitrous Oxide in kPa. |
| PartialPressurePollutant | 1255 | read | float | Partial pressure of Pollutant in kPa. |
| PartialPressureH2 | 1256 | read | float | Partial pressure of Hydrogen in kPa. |
| PartialPressureSteam | 1257 | read | float | Partial pressure of Steam/Water in kPa. |
| PartialPressureToxins | 1258 | read | float | Partial pressure of Toxins in kPa. |

### Gas/Liquid Separation

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| TotalMolesGasses | 1260 | read | float | Total moles of gases only (excludes liquids). |
| TotalMolesLiquids | 1261 | read | float | Total moles of liquids only (excludes gases). |
| LiquidVolumeRatio | 1262 | read | float | Ratio of liquid volume to total volume 0-1. |
| PressureGasses | 1263 | read | float | Gas-only pressure in kPa (excludes liquid pressure). |
| LiquidPressureOffset | 1264 | read | float | Liquid pressure offset in kPa. |

### Combustion & Reactions

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| PipeCombustionEnergy | 1270 | read | float | Energy from combustion reactions in joules. |
| CleanBurnRate | 1271 | read | float | Clean burn rate/efficiency 0-1. |
| Inflamed | 1272 | read | bool | 1 if atmosphere is inflamed/on fire, 0 if not. |
| Suppressed | 1273 | read | int | Fire suppression ticks remaining. |

### Network & State

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| EnergyConvected | 1280 | read | float | Heat energy being convected in watts. |
| EnergyRadiated | 1281 | read | float | Heat energy being radiated in watts. |
| IsAboveArmstrong | 1282 | read | bool | 1 if pressure is above Armstrong limit (6.3kPa), 0 if not. |
| Condensation | 1283 | read | bool | 1 if condensation is occurring, 0 if not. |
| NetworkContentType | 1284 | read | int | Network content type: 0=Gas, 1=Liquid. |

### Stress Monitoring

| LogicType | Value | Access | Data Type | Description |
|-----------|-------|--------|-----------|-------------|
| PipeMaxPressure | 1285 | read | float | Maximum burst pressure for connected pipe type in kPa. |
| PipeStressRatio | 1286 | read | float | Current pressure / max pressure ratio (0-1+, >=0.8 = stressed). |
| PipeIsStressed | 1287 | read | bool | 1 if pipe is stressed (high pressure OR liquid in gas pipe), 0 if not. |

### Understanding Pipe Stress

Pipes become "stressed" in two scenarios:
1. **High Pressure**: When pressure reaches 80% of maximum burst pressure
   - Gas pipes: Max 60795 kPa, stressed at ~48636 kPa
   - Liquid pipes: Max 6079.5 kPa, stressed at ~4864 kPa
2. **Wrong Contents**: When liquid enters a gas-only pipe network

`PipeIsStressed` uses the game's actual stress detection, so it correctly identifies both conditions. Use `PipeStressRatio` to see how close you are to pressure-based stress (0.8 = threshold).

### Example IC10 Code

```
alias analyzer d0
alias stressRatio r0
alias isStressed r1
alias maxP r2

# Read stress monitoring values
l stressRatio analyzer 1286  # PipeStressRatio
l isStressed analyzer 1287   # PipeIsStressed
l maxP analyzer 1285         # PipeMaxPressure

# Warning if stressed
bgtz isStressed pipe_danger
j pipe_safe

pipe_danger:
    # Activate warning light or reduce pressure
    # PipeStressRatio tells you how close to burst (1.0 = burst point)

pipe_safe:
    # Normal operations
```

### Prevent pipe bursts with pressure monitoring

```
alias analyzer d0
alias pump d1
alias stressRatio r0

define SafeThreshold 0.7  # Stop pumping at 70% capacity

loop:
    # Check stress ratio
    l stressRatio analyzer 1286  # PipeStressRatio

    # Disable pump if approaching stress
    slt r1 stressRatio SafeThreshold
    s pump On r1

    yield
    j loop
```

### Monitor partial pressures

```
alias analyzer d0
alias o2 r0
alias n2 r1
alias co2 r2

# Read partial pressures
l o2 analyzer 1250   # PartialPressureO2
l n2 analyzer 1252   # PartialPressureN2
l co2 analyzer 1251  # PartialPressureCO2

# Calculate total from components
add r3 o2 n2
add r3 r3 co2
# r3 now has sum of main gas partial pressures
```

---

## Value Ranges

| Range | Purpose |
|-------|---------|
| 1000-1009 | Contact Selection & Filtering (SatelliteDish) |
| 1010-1029 | Selected Contact Properties (SatelliteDish) |
| 1030-1039 | Dish State (SatelliteDish) |
| 1040-1099 | Reserved for future SatelliteDish features |
| 1100-1109 | Centrifuge |
| 1110-1119 | DaylightSensor / Realtime Data |
| 1120-1129 | WindTurbineGenerator |
| 1130-1139 | SolidFuelGenerator |
| 1140-1149 | WeatherStation |
| 1150-1159 | DeepMiner |
| 1160-1169 | HydroponicsDevice |
| 1180-1189 | Harvester |
| 1200-1209 | GasFuelGenerator |
| 1250-1289 | PipeAnalyzer |
| 1300+ | Reserved for other devices |

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 2.5.0 | 2025 | Added PipeAnalyzer stress monitoring (3 LogicTypes): PipeMaxPressure, PipeStressRatio, PipeIsStressed. Complete PipeAnalyzer section with all 21 LogicTypes documented. |
| 2.0.0 | 2025 | Added Harvester (10 LogicTypes): HasTray, IsHarvesting, IsPlanting, ArmState, HasImportPlant, ImportPlantHash, HasFertilizer, FertilizerCycles, FertilizerHarvestBoost, FertilizerGrowthSpeed. Added HydroponicsDevice plant status (7 LogicTypes): PlantIsFertilized, PlantGrowthEfficiency, BreathingEfficiency, TemperatureEfficiency, PlantLightEfficiency, PlantPressureEfficiency, HydrationEfficiency. GasFuelGenerator values moved to 1200-1205. |
| 1.9.0 | 2025 | Added GasFuelGenerator: CombustionEnergy, IsValidAtmosphere, DoShutdown, MinTemperature, MaxTemperature, MinPressure |
| 1.8.0 | 2025 | Added HydroponicsDevice: LightExposure, IsLitByGrowLight, WaterMoles |
| 1.7.0 | 2025 | Added DeepMiner: MiningProgress, CurrentOreHash |
| 1.6.0 | 2025 | Added WeatherStation: WeatherWindStrength, DaysSinceLastWeatherEvent |
| 1.5.0 | 2025 | Added DaylightSensor: DayLengthSeconds, Latitude, Longitude, WeatherSolarRatio |
| 1.4.0 | 2025 | Added SolidFuelGenerator: FuelTicks for fuel buffer monitoring |
| 1.3.0 | 2025 | Added WindTurbineGenerator: WindSpeed, MaxPower (storm-aware), TurbineSpeed, AtmosphericPressure |
| 1.2.0 | 2025 | Added DaylightSensor realtime data: TimeOfDay, IsEclipse, EclipseRatio, DaysPast |
| 1.1.0 | 2025 | Added Centrifuge support: Rpm for regular Centrifuge, CentrifugeProcessing for both |
| 1.0.0 | 2024 | Initial release with SatelliteDish support (19 LogicTypes) |
