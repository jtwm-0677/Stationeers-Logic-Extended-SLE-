using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;
using UnityEngine;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for FiltrationMachineBase to add custom LogicTypes.
    /// Exposes filter slot data, machine state, and per-filter properties.
    /// </summary>
    public static class FiltrationPatches
    {
        // Cached reflection for private fields
        private static FieldInfo _powerUsedDuringTickField;
        private static FieldInfo _usedTicksField;

        // Tick constants for filter degradation by life tier
        private static readonly int[] TicksBeforeDegrade = { 144, 720, 2880, 11520 };

        static FiltrationPatches()
        {
            _powerUsedDuringTickField = typeof(FiltrationMachineBase)
                .GetField("_powerUsedDuringTick", BindingFlags.NonPublic | BindingFlags.Instance);

            _usedTicksField = typeof(GasFilter)
                .GetField("_usedTicks", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get power used during the current tick.
        /// </summary>
        public static float GetPowerUsed(FiltrationMachineBase machine)
        {
            return (float?)_powerUsedDuringTickField?.GetValue(machine) ?? 0f;
        }

        /// <summary>
        /// Get the used ticks for a filter (ticks since last degradation).
        /// </summary>
        public static int GetUsedTicks(GasFilter filter)
        {
            return (int?)_usedTicksField?.GetValue(filter) ?? 0;
        }

        /// <summary>
        /// Get max ticks before degradation based on filter life tier.
        /// </summary>
        public static int GetMaxTicks(GasFilterLife life)
        {
            int index = (int)life;
            if (index >= 0 && index < TicksBeforeDegrade.Length)
                return TicksBeforeDegrade[index];
            return TicksBeforeDegrade[0]; // Default to Normal
        }

        /// <summary>
        /// Count the number of gas filter slots in the machine.
        /// </summary>
        public static int CountFilterSlots(FiltrationMachineBase machine)
        {
            int count = 0;
            foreach (Slot slot in machine.Slots)
            {
                if (slot.Type == Slot.Class.GasFilter)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Get the filter at the specified slot index (0-based among filter slots only).
        /// </summary>
        public static GasFilter GetFilterAtIndex(FiltrationMachineBase machine, int index)
        {
            int filterIndex = 0;
            foreach (Slot slot in machine.Slots)
            {
                if (slot.Type == Slot.Class.GasFilter)
                {
                    if (filterIndex == index)
                    {
                        GasFilter filter;
                        if (slot.Contains<GasFilter>(out filter))
                            return filter;
                        return null; // Slot exists but is empty
                    }
                    filterIndex++;
                }
            }
            return null; // Index out of range
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading FiltrationMachineBase custom LogicTypes.
    /// Uses TargetMethod for proper resolution of inherited methods.
    /// </summary>
    [HarmonyPatch]
    public static class FiltrationCanLogicReadPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(FiltrationMachineBase), "CanLogicRead", new[] { typeof(LogicType) });
        }

        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: FilterSlotIndex (1400) through FilterMaxTicks (1416)
            if (value >= (ushort)SLELogicType.FilterSlotIndex && value <= (ushort)SLELogicType.FilterMaxTicks)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch CanLogicWrite to allow writing FiltrationMachineBase custom LogicTypes.
    /// Uses TargetMethod for proper resolution of inherited methods.
    /// </summary>
    [HarmonyPatch]
    public static class FiltrationCanLogicWritePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(FiltrationMachineBase), "CanLogicWrite", new[] { typeof(LogicType) });
        }

        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Only FilterSlotIndex is writable
            if (value == (ushort)SLELogicType.FilterSlotIndex)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return FiltrationMachineBase custom LogicType values.
    /// Uses TargetMethod for proper resolution of inherited methods.
    /// </summary>
    [HarmonyPatch]
    public static class FiltrationGetLogicValuePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(FiltrationMachineBase), "GetLogicValue", new[] { typeof(LogicType) });
        }

        public static bool Prefix(FiltrationMachineBase __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1400-1416)
            if (value < (ushort)SLELogicType.FilterSlotIndex || value > (ushort)SLELogicType.FilterMaxTicks)
                return true;

            var state = DeviceStateManager.GetOrCreate(__instance);
            if (state == null)
            {
                __result = 0;
                return false;
            }

            var sleType = (SLELogicType)value;
            __result = GetCustomLogicValue(__instance, state, sleType);
            return false; // Skip original method
        }

        private static double GetCustomLogicValue(FiltrationMachineBase machine, FiltrationMachineState state, SLELogicType logicType)
        {
            switch (logicType)
            {
                // Machine State
                case SLELogicType.FilterSlotIndex:
                    return state.FilterSlotIndex;

                case SLELogicType.FilterSlotCount:
                    return FiltrationPatches.CountFilterSlots(machine);

                case SLELogicType.HasEmptyFilter:
                    return machine.HasEmptyFilter() ? 1 : 0;

                case SLELogicType.IsFullyConnected:
                    return machine.IsFullyConnected ? 1 : 0;

                case SLELogicType.FilterPowerUsed:
                    return FiltrationPatches.GetPowerUsed(machine);

                case SLELogicType.FiltrationProcessedMoles:
                    return machine.ProcessedMoles.ToDouble();

                // Per-Filter Properties
                case SLELogicType.FilterQuantity:
                case SLELogicType.FilterIsLow:
                case SLELogicType.FilterIsEmpty:
                case SLELogicType.FilterTypeHash:
                case SLELogicType.FilterLife:
                case SLELogicType.FilterUsedTicks:
                case SLELogicType.FilterMaxTicks:
                    return GetFilterPropertyValue(machine, state, logicType);

                default:
                    return 0;
            }
        }

        private static double GetFilterPropertyValue(FiltrationMachineBase machine, FiltrationMachineState state, SLELogicType logicType)
        {
            var filter = FiltrationPatches.GetFilterAtIndex(machine, state.FilterSlotIndex);

            // Return -1 if no filter at selected index
            if (filter == null)
                return -1;

            switch (logicType)
            {
                case SLELogicType.FilterQuantity:
                    return filter.Quantity;

                case SLELogicType.FilterIsLow:
                    return filter.IsLow ? 1 : 0;

                case SLELogicType.FilterIsEmpty:
                    return filter.IsEmpty ? 1 : 0;

                case SLELogicType.FilterTypeHash:
                    return (int)filter.FilterType;

                case SLELogicType.FilterLife:
                    return (int)filter.FilterLife;

                case SLELogicType.FilterUsedTicks:
                    return FiltrationPatches.GetUsedTicks(filter);

                case SLELogicType.FilterMaxTicks:
                    return FiltrationPatches.GetMaxTicks(filter.FilterLife);

                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// Patch SetLogicValue to handle writing FiltrationMachineBase custom LogicTypes.
    /// Uses TargetMethod for proper resolution of inherited methods.
    /// </summary>
    [HarmonyPatch]
    public static class FiltrationSetLogicValuePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(FiltrationMachineBase), "SetLogicValue", new[] { typeof(LogicType), typeof(double) });
        }

        public static bool Prefix(FiltrationMachineBase __instance, LogicType logicType, double value)
        {
            ushort typeValue = (ushort)logicType;

            // Only handle FilterSlotIndex
            if (typeValue != (ushort)SLELogicType.FilterSlotIndex)
                return true;

            var state = DeviceStateManager.GetOrCreate(__instance);
            if (state == null)
                return false;

            int filterCount = FiltrationPatches.CountFilterSlots(__instance);
            state.FilterSlotIndex = Mathf.Clamp((int)value, 0, Mathf.Max(0, filterCount - 1));

            return false; // Skip original method
        }
    }
}
