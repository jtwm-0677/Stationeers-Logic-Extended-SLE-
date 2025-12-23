using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for RobotMining (AIMeE Bot) to add custom LogicTypes.
    /// Exposes storage state, battery, damage, and operational status.
    /// </summary>
    public static class RobotMiningPatches
    {
        // Cache reflection for private fields
        private static readonly FieldInfo StorageSlotsField = typeof(RobotMining).GetField("_storageSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IsBusyField = typeof(RobotMining).GetField("IsBusy", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading RobotMining custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(RobotMining), nameof(RobotMining.CanLogicRead))]
    public static class RobotMiningCanLogicReadPatch
    {
        public static void Postfix(RobotMining __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: RobotIsStorageEmpty (1700) through RobotStorageCapacity (1707)
            if (value >= (ushort)SLELogicType.RobotIsStorageEmpty && value <= (ushort)SLELogicType.RobotStorageCapacity)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return RobotMining custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(RobotMining), nameof(RobotMining.GetLogicValue))]
    public static class RobotMiningGetLogicValuePatch
    {
        // Cache reflection for private fields
        private static readonly FieldInfo StorageSlotsField = typeof(RobotMining).GetField("_storageSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IsBusyField = typeof(RobotMining).GetField("IsBusy", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(RobotMining __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.RobotIsStorageEmpty || value > (ushort)SLELogicType.RobotStorageCapacity)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.RobotIsStorageEmpty:
                    __result = __instance.IsStorageEmpty ? 1 : 0;
                    return false;

                case SLELogicType.RobotIsStorageFull:
                    __result = __instance.IsStorageFull ? 1 : 0;
                    return false;

                case SLELogicType.RobotIsOperable:
                    __result = __instance.IsOperable ? 1 : 0;
                    return false;

                case SLELogicType.RobotBatteryRatio:
                    var battery = __instance.Battery;
                    if (battery == null || battery.PowerMaximum <= 0)
                    {
                        __result = 0;
                    }
                    else
                    {
                        __result = battery.PowerStored / battery.PowerMaximum;
                    }
                    return false;

                case SLELogicType.RobotIsBusy:
                    var isBusy = IsBusyField?.GetValue(__instance);
                    __result = (isBusy is bool busy && busy) ? 1 : 0;
                    return false;

                case SLELogicType.RobotDamageRatio:
                    __result = __instance.DamageState?.TotalRatioClamped ?? 0;
                    return false;

                case SLELogicType.RobotStorageCount:
                    var storageSlots = StorageSlotsField?.GetValue(__instance) as List<Slot>;
                    if (storageSlots == null)
                    {
                        __result = 0;
                    }
                    else
                    {
                        int count = 0;
                        foreach (var slot in storageSlots)
                        {
                            if (slot.Occupant != null)
                                count++;
                        }
                        __result = count;
                    }
                    return false;

                case SLELogicType.RobotStorageCapacity:
                    var capacitySlots = StorageSlotsField?.GetValue(__instance) as List<Slot>;
                    __result = capacitySlots?.Count ?? 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
