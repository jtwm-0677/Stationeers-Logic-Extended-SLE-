using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for Quarry (Autominer Small) to add custom LogicTypes.
    /// Exposes drill state, ore count, depth, and operational status.
    /// Note: Quarry inherits from DeviceImportExport which defines CanLogicRead/GetLogicValue,
    /// so we patch that class and check for Quarry instance.
    /// </summary>
    public static class QuarryPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead on DeviceImportExport to allow reading Quarry LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(DeviceImportExport), nameof(DeviceImportExport.CanLogicRead))]
    public static class QuarryCanLogicReadPatch
    {
        public static void Postfix(DeviceImportExport __instance, ref bool __result, LogicType logicType)
        {
            // Only handle Quarry instances
            if (!(__instance is Quarry))
                return;

            ushort value = (ushort)logicType;
            // Check range: QuarryDrillState (1720) through QuarryIsDelivering (1726)
            if (value >= (ushort)SLELogicType.QuarryDrillState && value <= (ushort)SLELogicType.QuarryIsDelivering)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue on DeviceImportExport to return Quarry data values.
    /// </summary>
    [HarmonyPatch(typeof(DeviceImportExport), nameof(DeviceImportExport.GetLogicValue))]
    public static class QuarryGetLogicValuePatch
    {
        // Cache reflection for private fields
        private static readonly FieldInfo DrillStateField = typeof(Quarry).GetField("_drillState", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo OreQueueField = typeof(Quarry).GetField("_oreQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo DepthField = typeof(Quarry).GetField("_depth", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo MaxDepthField = typeof(Quarry).GetField("_maxDepth", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(DeviceImportExport __instance, LogicType logicType, ref double __result)
        {
            // Only handle Quarry instances
            if (!(__instance is Quarry quarry))
                return true;

            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.QuarryDrillState || value > (ushort)SLELogicType.QuarryIsDelivering)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.QuarryDrillState:
                    var drillState = DrillStateField?.GetValue(quarry);
                    __result = drillState != null ? (int)drillState : 0;
                    return false;

                case SLELogicType.QuarryOreCount:
                    var oreQueue = OreQueueField?.GetValue(quarry) as Queue<int>;
                    __result = oreQueue?.Count ?? 0;
                    return false;

                case SLELogicType.QuarryDepth:
                    var depth = DepthField?.GetValue(quarry);
                    __result = depth != null ? (float)depth : 0;
                    return false;

                case SLELogicType.QuarryMaxDepth:
                    var maxDepth = MaxDepthField?.GetValue(quarry);
                    __result = maxDepth != null ? (float)maxDepth : 0;
                    return false;

                case SLELogicType.QuarryIsDrillFinished:
                    var finishedState = DrillStateField?.GetValue(quarry);
                    // State 0 = Idle (drilling complete)
                    __result = (finishedState != null && (int)finishedState == 0) ? 1 : 0;
                    return false;

                case SLELogicType.QuarryIsTransporting:
                    var transportState = DrillStateField?.GetValue(quarry);
                    // State 2 = Transporting
                    __result = (transportState != null && (int)transportState == 2) ? 1 : 0;
                    return false;

                case SLELogicType.QuarryIsDelivering:
                    var deliverState = DrillStateField?.GetValue(quarry);
                    // State 3 = Delivering
                    __result = (deliverState != null && (int)deliverState == 3) ? 1 : 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
