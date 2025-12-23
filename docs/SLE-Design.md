# Stationeers Logic Extended (SLE) - Framework Design

## Status: IMPLEMENTED

## Overview

SLE is a BepInEx/Harmony mod that extends Stationeers' logic system by adding new LogicTypes to devices. The SatelliteDish is the first device to receive extended logic capabilities.

## Architecture

```
StationeersLogicExtended/
├── SLEPlugin.cs                  # BepInEx plugin entry point
├── StationeersLogicExtended.csproj
├── Core/
│   ├── SLELogicTypes.cs          # New LogicType definitions (enum)
│   ├── LogicTypeRegistry.cs      # Registration and metadata
│   ├── LogicTypeInfo.cs          # Documentation/metadata class
│   └── DeviceStateManager.cs     # Per-device state storage
├── Patches/
│   └── Devices/
│       └── SatelliteDishPatches.cs
├── Data/
│   └── SLE_LogicTypes.json       # Export for compiler integration
├── docs/
│   └── SLE-Design.md             # This file
└── README.md                     # User documentation
```

## Key Design Decisions

### LogicType Values Start at 1000
- Game uses 0-271
- We use 1000+ to avoid conflicts with future game updates
- Leaves room for ~700 more game types before collision

### Per-Device State Storage
- Custom state (ContactIndex, FilterMode, FilterValue) stored in `DeviceStateManager`
- Keyed by device `ReferenceId`
- Thread-safe for multiplayer

### Harmony Patching Strategy
- Postfix on `CanLogicRead`/`CanLogicWrite` - additive, doesn't break vanilla
- Prefix on `GetLogicValue`/`SetLogicValue` - returns early for custom types only

### Filter System
- Filters applied dynamically when reading contacts
- `ContactIndex` clamped to valid range for current filter
- Changing filter resets index to 0

## Compiler Integration

`Data/SLE_LogicTypes.json` contains all LogicType definitions in a format compatible with the BASICtoMIPS compiler. Copy to compiler's `Data/CustomDevices/` folder.

## Future Expansion

To add more devices:
1. Create new state class in `Core/`
2. Add to `DeviceStateManager`
3. Create patch file in `Patches/Devices/`
4. Register new LogicTypes in `LogicTypeRegistry.Initialize()`
5. Update `SLE_LogicTypes.json`
