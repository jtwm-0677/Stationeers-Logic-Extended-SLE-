using System.Reflection;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for ActiveVent to add custom LogicTypes.
    /// Exposes flow status, connection state, and power usage.
    /// Note: ActiveVent already exposes PressureExternal/PressureInternal via vanilla LogicTypes.
    /// </summary>
    public static class ActiveVentPatches
    {
        // No private fields to cache - ActiveVent uses inherited UsedPower for constant power draw
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading ActiveVent custom LogicTypes.
    /// </summary>
    [HarmonyPatch]
    public static class ActiveVentCanLogicReadPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ActiveVent), "CanLogicRead", new[] { typeof(LogicType) });
        }

        public static void Postfix(ActiveVent __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to ActiveVent itself, not derived types (fixes LarreDoc bug)
            if (__instance.GetType() != typeof(ActiveVent))
                return;

            ushort value = (ushort)logicType;
            // Check range: VentFlowStatus (1540) through VentPowerUsed (1542)
            if (value >= (ushort)SLELogicType.VentFlowStatus && value <= (ushort)SLELogicType.VentPowerUsed)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return ActiveVent custom LogicType values.
    /// </summary>
    [HarmonyPatch]
    public static class ActiveVentGetLogicValuePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ActiveVent), "GetLogicValue", new[] { typeof(LogicType) });
        }

        public static bool Prefix(ActiveVent __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1540-1542)
            if (value < (ushort)SLELogicType.VentFlowStatus || value > (ushort)SLELogicType.VentPowerUsed)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.VentFlowStatus:
                    // FlowIndicatorState enum: None=0, Max=1, InwardsLimited=2, InwardsVeryLimited=3,
                    // OutwardsLimited=4, OutwardsVeryLimited=5, Idle=6
                    __result = (int)__instance.FlowIndicatorStatus;
                    break;

                case SLELogicType.VentIsConnected:
                    // Check if vent has valid pipe network and atmosphere connection
                    __result = __instance.HasPipeNetwork ? 1 : 0;
                    break;

                case SLELogicType.VentPowerUsed:
                    // ActiveVent uses constant power when on - use inherited UsedPower
                    __result = __instance.OnOff && __instance.Powered ? __instance.UsedPower : 0;
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
