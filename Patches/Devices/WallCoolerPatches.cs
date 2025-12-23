using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for WallCooler to add custom LogicTypes.
    /// Exposes environment validity, power usage, and energy transfer.
    /// </summary>
    public static class WallCoolerPatches
    {
        // Cached reflection for private fields
        private static FieldInfo _powerUsedDuringTickField;

        static WallCoolerPatches()
        {
            _powerUsedDuringTickField = typeof(WallCooler)
                .GetField("_powerUsedDuringTick", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get power used during the current tick.
        /// </summary>
        public static float GetPowerUsed(WallCooler cooler)
        {
            return (float?)_powerUsedDuringTickField?.GetValue(cooler) ?? 0f;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading WallCooler custom LogicTypes.
    /// </summary>
    [HarmonyPatch]
    public static class WallCoolerCanLogicReadPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(WallCooler), "CanLogicRead", new[] { typeof(LogicType) });
        }

        public static void Postfix(WallCooler __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to WallCooler itself, not derived types (fixes LarreDoc bug)
            if (__instance.GetType() != typeof(WallCooler))
                return;

            ushort value = (ushort)logicType;
            // Check range: CoolerIsEnvironmentOkay (1520) through CoolerEnergyMoved (1523)
            if (value >= (ushort)SLELogicType.CoolerIsEnvironmentOkay && value <= (ushort)SLELogicType.CoolerEnergyMoved)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return WallCooler custom LogicType values.
    /// </summary>
    [HarmonyPatch]
    public static class WallCoolerGetLogicValuePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(WallCooler), "GetLogicValue", new[] { typeof(LogicType) });
        }

        public static bool Prefix(WallCooler __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1520-1523)
            if (value < (ushort)SLELogicType.CoolerIsEnvironmentOkay || value > (ushort)SLELogicType.CoolerEnergyMoved)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.CoolerIsEnvironmentOkay:
                    __result = __instance.IsEnvironmentOkay ? 1 : 0;
                    break;

                case SLELogicType.CoolerIsPipeOkay:
                    __result = __instance.IsPipeEnvironmentOkay ? 1 : 0;
                    break;

                case SLELogicType.CoolerPowerUsed:
                    __result = WallCoolerPatches.GetPowerUsed(__instance);
                    break;

                case SLELogicType.CoolerEnergyMoved:
                    // WallCooler doesn't have EnergyMoved property - _powerUsedDuringTick equals energy transferred
                    __result = WallCoolerPatches.GetPowerUsed(__instance);
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
