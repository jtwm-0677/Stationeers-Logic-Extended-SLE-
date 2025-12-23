using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for Quarry (Autominer Small) to add custom LogicTypes.
    /// Exposes drill state, ore count, depth, and operational status.
    /// </summary>
    public static class QuarryPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading Quarry custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(Quarry), nameof(Quarry.CanLogicRead))]
    public static class QuarryCanLogicReadPatch
    {
        public static void Postfix(Quarry __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: QuarryDrillState (1720) through QuarryIsDelivering (1726)
            if (value >= (ushort)SLELogicType.QuarryDrillState && value <= (ushort)SLELogicType.QuarryIsDelivering)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return Quarry custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(Quarry), nameof(Quarry.GetLogicValue))]
    public static class QuarryGetLogicValuePatch
    {
        // Cache reflection for private fields
        private static readonly FieldInfo DrillStateField = typeof(Quarry).GetField("_drillState", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo OreQueueField = typeof(Quarry).GetField("_oreQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo DepthField = typeof(Quarry).GetField("_depth", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo MaxDepthField = typeof(Quarry).GetField("_maxDepth", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(Quarry __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.QuarryDrillState || value > (ushort)SLELogicType.QuarryIsDelivering)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.QuarryDrillState:
                    var drillState = DrillStateField?.GetValue(__instance);
                    __result = drillState != null ? (int)drillState : 0;
                    return false;

                case SLELogicType.QuarryOreCount:
                    var oreQueue = OreQueueField?.GetValue(__instance) as Queue<int>;
                    __result = oreQueue?.Count ?? 0;
                    return false;

                case SLELogicType.QuarryDepth:
                    var depth = DepthField?.GetValue(__instance);
                    __result = depth != null ? (float)depth : 0;
                    return false;

                case SLELogicType.QuarryMaxDepth:
                    var maxDepth = MaxDepthField?.GetValue(__instance);
                    __result = maxDepth != null ? (float)maxDepth : 0;
                    return false;

                case SLELogicType.QuarryIsDrillFinished:
                    var finishedState = DrillStateField?.GetValue(__instance);
                    // State 0 = Idle (drilling complete)
                    __result = (finishedState != null && (int)finishedState == 0) ? 1 : 0;
                    return false;

                case SLELogicType.QuarryIsTransporting:
                    var transportState = DrillStateField?.GetValue(__instance);
                    // State 2 = Transporting
                    __result = (transportState != null && (int)transportState == 2) ? 1 : 0;
                    return false;

                case SLELogicType.QuarryIsDelivering:
                    var deliverState = DrillStateField?.GetValue(__instance);
                    // State 3 = Delivering
                    __result = (deliverState != null && (int)deliverState == 3) ? 1 : 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
