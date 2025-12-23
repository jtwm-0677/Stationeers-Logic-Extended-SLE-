using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for AreaPowerControl (APC) to add custom LogicTypes.
    /// Exposes MaximumPower which includes both battery capacity AND network potential.
    /// Vanilla only exposes Maximum = Battery.PowerMaximum (excludes network).
    /// </summary>
    public static class AreaPowerControlPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading APC custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(AreaPowerControl), nameof(AreaPowerControl.CanLogicRead))]
    public static class AreaPowerControlCanLogicReadPatch
    {
        public static void Postfix(AreaPowerControl __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check for APCMaximumPower (1830)
            if (value == (ushort)SLELogicType.APCMaximumPower)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return APC custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(AreaPowerControl), nameof(AreaPowerControl.GetLogicValue))]
    public static class AreaPowerControlGetLogicValuePatch
    {
        public static bool Prefix(AreaPowerControl __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle APCMaximumPower
            if (value != (ushort)SLELogicType.APCMaximumPower)
                return true;

            // MaximumPower = Battery.PowerMaximum + InputNetwork.PotentialLoad
            // This is the total system capacity, not just the battery
            __result = __instance.MaximumPower;
            return false;
        }
    }
}
