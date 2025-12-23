using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for AirConditioner to add custom LogicTypes.
    /// Exposes energy transfer, connection state, and efficiency data.
    /// </summary>
    public static class AirConditionerPatches
    {
        // Cached reflection for private fields
        private static FieldInfo _powerUsedDuringTickField;

        static AirConditionerPatches()
        {
            _powerUsedDuringTickField = typeof(AirConditioner)
                .GetField("_powerUsedDuringTick", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get power used during the current tick.
        /// </summary>
        public static float GetPowerUsed(AirConditioner ac)
        {
            return (float?)_powerUsedDuringTickField?.GetValue(ac) ?? 0f;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading AirConditioner custom LogicTypes.
    /// </summary>
    [HarmonyPatch]
    public static class AirConditionerCanLogicReadPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(AirConditioner), "CanLogicRead", new[] { typeof(LogicType) });
        }

        public static void Postfix(AirConditioner __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to AirConditioner itself, not derived types
            if (__instance.GetType() != typeof(AirConditioner))
                return;

            ushort value = (ushort)logicType;
            // Check range: ACEnergyMoved (1500) through ACEfficiency (1503)
            if (value >= (ushort)SLELogicType.ACEnergyMoved && value <= (ushort)SLELogicType.ACEfficiency)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return AirConditioner custom LogicType values.
    /// </summary>
    [HarmonyPatch]
    public static class AirConditionerGetLogicValuePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(AirConditioner), "GetLogicValue", new[] { typeof(LogicType) });
        }

        public static bool Prefix(AirConditioner __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1500-1503)
            if (value < (ushort)SLELogicType.ACEnergyMoved || value > (ushort)SLELogicType.ACEfficiency)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.ACEnergyMoved:
                    __result = __instance.EnergyMoved.ToDouble();
                    break;

                case SLELogicType.ACIsFullyConnected:
                    __result = __instance.IsFullyConnected ? 1 : 0;
                    break;

                case SLELogicType.ACPowerUsed:
                    __result = AirConditionerPatches.GetPowerUsed(__instance);
                    break;

                case SLELogicType.ACEfficiency:
                    // Combined efficiency = TempDiff * OpTemp * Pressure (all 0-1 range)
                    __result = __instance.TemperatureDifferentialEfficiency *
                               __instance.OperationalTemperatureLimitor *
                               __instance.OptimalPressureScalar;
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
