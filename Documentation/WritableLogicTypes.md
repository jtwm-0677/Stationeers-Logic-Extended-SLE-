# Writable LogicTypes - Research Notes

**Date:** 2025-12-19
**Status:** Research complete, implementation pending

## Goal

Add IC10-writable LogicTypes that allow setting device parameters like:
- Maximum pressure for Filtration Machine
- Maximum/target pressure for Air Conditioner
- Other configurable limits currently only accessible via device UI

## Stationeers Tweaker Investigation

**Repository:** https://github.com/tsholmes/StationeersTweaker
**Workshop:** https://steamcommunity.com/sharedfiles/filedetails/?id=3560876115

### What Tweaker Does
- Modifies **prefabs at load time** (before game uses them)
- Uses reflection to set any field/property value on prefab definitions
- Hooks into `Prefab.Clear` postfix to apply changes
- Operates on static data, not live device instances

### Why It Doesn't Apply
Tweaker changes default values for all instances of a device type. We need:
- Per-instance runtime modifications
- IC10 programmable control
- Values that persist and sync in multiplayer

**Conclusion:** Tweaker's approach is not suitable for IC10-writable LogicTypes.

## Discovered Opportunities

### AirConditioner - Unused Field
```csharp
// File: Assets\Scripts\Objects\Electrical\AirConditioner.cs, Line 643
private float _goalPressure = 101.325f;
```
This field exists but is **never used** in the current implementation. It could potentially be:
1. Exposed as a writable LogicType
2. Patched into `OnAtmosphericTick` to affect behavior

### FiltrationMachine - No Pressure Limit
- Uses `PressurePerTick` from base class `DeviceInputOutputCircuit`
- Calculates pressure differential dynamically
- No configurable max pressure field exists
- Would need to add our own storage + behavior patch

## Implementation Approach

### Step 1: Patch CanLogicWrite
Return true for custom writable LogicTypes:
```csharp
[HarmonyPatch(typeof(AirConditioner), nameof(AirConditioner.CanLogicWrite))]
public static class AirConditionerCanLogicWritePatch
{
    public static void Postfix(AirConditioner __instance, ref bool __result, LogicType logicType)
    {
        if (logicType == (LogicType)SLELogicType.GoalPressure)
            __result = true;
    }
}
```

### Step 2: Patch SetLogicValue
Handle the actual value write:
```csharp
[HarmonyPatch(typeof(AirConditioner), nameof(AirConditioner.SetLogicValue))]
public static class AirConditionerSetLogicValuePatch
{
    public static bool Prefix(AirConditioner __instance, LogicType logicType, double value)
    {
        if (logicType == (LogicType)SLELogicType.GoalPressure)
        {
            // Option A: Use reflection to set _goalPressure
            // Option B: Store in our own dictionary keyed by device ReferenceId
            return false; // Skip original
        }
        return true;
    }
}
```

### Step 3: Value Storage Options

**Option A: Use Existing Fields (when available)**
```csharp
private static readonly FieldInfo GoalPressureField =
    typeof(AirConditioner).GetField("_goalPressure",
        BindingFlags.NonPublic | BindingFlags.Instance);

public static void SetGoalPressure(AirConditioner device, float value)
{
    GoalPressureField?.SetValue(device, value);
}
```

**Option B: External Dictionary (when no field exists)**
```csharp
// In a new storage class
public static class SLEDeviceStorage
{
    private static readonly Dictionary<long, Dictionary<SLELogicType, double>> _values = new();

    public static void SetValue(Thing device, SLELogicType type, double value)
    {
        if (!_values.TryGetValue(device.ReferenceId, out var dict))
        {
            dict = new Dictionary<SLELogicType, double>();
            _values[device.ReferenceId] = dict;
        }
        dict[type] = value;
    }

    public static double GetValue(Thing device, SLELogicType type, double defaultValue = 0)
    {
        if (_values.TryGetValue(device.ReferenceId, out var dict) &&
            dict.TryGetValue(type, out var value))
            return value;
        return defaultValue;
    }

    // Need to clean up when devices are destroyed
    public static void OnDeviceDestroyed(Thing device)
    {
        _values.Remove(device.ReferenceId);
    }
}
```

### Step 4: Patch Device Behavior
Make the device respect our stored value:
```csharp
[HarmonyPatch(typeof(FiltrationMachineBase), nameof(FiltrationMachineBase.OnAtmosphericTick))]
public static class FiltrationMaxPressurePatch
{
    public static bool Prefix(FiltrationMachineBase __instance)
    {
        var maxPressure = SLEDeviceStorage.GetValue(__instance, SLELogicType.MaxPressure, double.MaxValue);

        // Check if output would exceed max pressure
        if (__instance.OutputNetwork?.Atmosphere?.PressureGasses.ToDouble() >= maxPressure)
        {
            // Skip processing this tick
            return false;
        }
        return true;
    }
}
```

## Considerations

### Multiplayer Sync
- Values set via IC10 should sync automatically (IC10 commands are server-authoritative)
- May need to hook into save/load for persistence
- External dictionary values won't persist across save/load without additional work

### Save/Load Persistence
For Option B (external dictionary), would need to:
1. Hook into world save to serialize our dictionary
2. Hook into world load to deserialize
3. Consider using device's existing save data if possible

### Device Cleanup
Must remove dictionary entries when devices are destroyed to prevent memory leaks:
```csharp
[HarmonyPatch(typeof(Thing), nameof(Thing.OnDestroy))]
public static class ThingDestroyCleanupPatch
{
    public static void Postfix(Thing __instance)
    {
        SLEDeviceStorage.OnDeviceDestroyed(__instance);
    }
}
```

## Proposed LogicType Values

| Device | LogicType | Value Range | Description |
|--------|-----------|-------------|-------------|
| AirConditioner | GoalPressure (1550) | 0-50000 kPa | Target output pressure |
| FiltrationMachine | MaxOutputPressure (1551) | 0-50000 kPa | Stop when output exceeds |
| FiltrationMachine | MaxFilteredPressure (1552) | 0-50000 kPa | Stop when filtered output exceeds |
| ActiveVent | MaxPressure (1553) | 0-50000 kPa | Pressure limit for operation |

## Files to Create/Modify

1. **Core/SLEDeviceStorage.cs** - New file for value storage
2. **Core/SLELogicTypes.cs** - Add new writable LogicType enums
3. **Core/LogicTypeRegistry.cs** - Register with `canWrite: true`
4. **Patches/Devices/[Device]Patches.cs** - Add CanLogicWrite, SetLogicValue, behavior patches

## Next Steps

1. Examine existing `SetLogicValue` implementations in game for patterns
2. Decide on storage approach (existing fields vs external dictionary)
3. Implement for one device as proof of concept
4. Test multiplayer sync behavior
5. Add save/load persistence if needed
