using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for WallHeater to add custom LogicTypes.
    /// Exposes environment validity, power usage, and energy transfer.
    /// WallHeater heats room atmosphere directly (no pipe connection).
    /// </summary>
    public static class WallHeaterPatches
    {
        // Cached reflection for private fields
        private static FieldInfo _powerUsedDuringTickField;
        private static FieldInfo _heatTransferField;

        static WallHeaterPatches()
        {
            _powerUsedDuringTickField = typeof(WallHeater)
                .GetField("_powerUsedDuringTick", BindingFlags.NonPublic | BindingFlags.Instance);

            _heatTransferField = typeof(WallHeater)
                .GetField("heatTransferJoulesPerTick", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get power used during the current tick.
        /// </summary>
        public static float GetPowerUsed(WallHeater heater)
        {
            return (float?)_powerUsedDuringTickField?.GetValue(heater) ?? 0f;
        }

        /// <summary>
        /// Get heat transfer joules per tick (constant value).
        /// </summary>
        public static float GetHeatTransfer(WallHeater heater)
        {
            return (float?)_heatTransferField?.GetValue(heater) ?? 1000f;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading WallHeater custom LogicTypes.
    /// </summary>
    [HarmonyPatch]
    public static class WallHeaterCanLogicReadPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(WallHeater), "CanLogicRead", new[] { typeof(LogicType) });
        }

        public static void Postfix(WallHeater __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to WallHeater itself, not derived types (fixes LarreDoc bug)
            if (__instance.GetType() != typeof(WallHeater))
                return;

            ushort value = (ushort)logicType;
            // Check range: HeaterIsEnvironmentOkay (1530) through HeaterMaxTemperature (1534)
            if (value >= (ushort)SLELogicType.HeaterIsEnvironmentOkay && value <= (ushort)SLELogicType.HeaterMaxTemperature)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return WallHeater custom LogicType values.
    /// </summary>
    [HarmonyPatch]
    public static class WallHeaterGetLogicValuePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(WallHeater), "GetLogicValue", new[] { typeof(LogicType) });
        }

        public static bool Prefix(WallHeater __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1530-1534)
            if (value < (ushort)SLELogicType.HeaterIsEnvironmentOkay || value > (ushort)SLELogicType.HeaterMaxTemperature)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.HeaterIsEnvironmentOkay:
                    // WallHeater needs open grid (room atmosphere) to operate
                    __result = __instance.HasOpenGrid ? 1 : 0;
                    break;

                case SLELogicType.HeaterIsPipeOkay:
                    // WallHeater has no pipe connection - return -1 to indicate N/A
                    __result = -1;
                    break;

                case SLELogicType.HeaterPowerUsed:
                    __result = WallHeaterPatches.GetPowerUsed(__instance);
                    break;

                case SLELogicType.HeaterEnergyTransfer:
                    // Returns joules per tick when operating
                    __result = WallHeaterPatches.GetHeatTransfer(__instance);
                    break;

                case SLELogicType.HeaterMaxTemperature:
                    // WallHeater stops adding heat at 2500K
                    __result = 2500;
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
