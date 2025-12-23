using System.Reflection;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for CombustionCentrifuge to add Centrifuge LogicTypes.
    /// CombustionCentrifuge already has Rpm, Stress, Throttle, CombustionLimiter.
    /// We add Processing, ReagentTotal, and LidClosed.
    /// </summary>
    public static class CombustionCentrifugePatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading Centrifuge custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(CombustionCentrifuge), nameof(CombustionCentrifuge.CanLogicRead))]
    public static class CombustionCentrifugeCanLogicReadPatch
    {
        public static void Postfix(CombustionCentrifuge __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to CombustionCentrifuge itself, not derived types
            if (__instance.GetType() != typeof(CombustionCentrifuge))
                return;

            ushort value = (ushort)logicType;
            // Check range: CentrifugeProcessing (1100) through CentrifugeLidClosed (1103)
            if (value >= (ushort)SLELogicType.CentrifugeProcessing && value <= (ushort)SLELogicType.CentrifugeLidClosed)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return Centrifuge custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(CombustionCentrifuge), nameof(CombustionCentrifuge.GetLogicValue))]
    public static class CombustionCentrifugeGetLogicValuePatch
    {
        // Cache reflection for private fields
        private static readonly FieldInfo ReagentTotalField = typeof(CombustionCentrifuge).GetField("_reagentTotal", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo LidClosedField = typeof(CombustionCentrifuge).GetField("_lidClosed", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(CombustionCentrifuge __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.CentrifugeProcessing || value > (ushort)SLELogicType.CentrifugeLidClosed)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.CentrifugeProcessing:
                    __result = __instance.Processing;
                    return false;

                case SLELogicType.CentrifugeRPM:
                    // Rpm is already a vanilla LogicType, but we expose it under our naming for consistency
                    __result = __instance.Rpm;
                    return false;

                case SLELogicType.CentrifugeReagentTotal:
                    var reagentTotal = ReagentTotalField?.GetValue(__instance);
                    __result = reagentTotal != null ? (float)reagentTotal : 0;
                    return false;

                case SLELogicType.CentrifugeLidClosed:
                    var lidClosed = LidClosedField?.GetValue(__instance);
                    __result = (lidClosed is bool closed && closed) ? 1 : 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
