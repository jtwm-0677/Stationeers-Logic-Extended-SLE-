using System;
using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for StirlingEngine to add custom LogicTypes.
    /// Exposes hot/cold temperatures, efficiency, and power output.
    /// </summary>
    public static class StirlingEnginePatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading StirlingEngine custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(StirlingEngine), nameof(StirlingEngine.CanLogicRead))]
    public static class StirlingEngineCanLogicReadPatch
    {
        public static void Postfix(StirlingEngine __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: StirlingHotTemperature (1780) through StirlingIsConnected (1785)
            if (value >= (ushort)SLELogicType.StirlingHotTemperature && value <= (ushort)SLELogicType.StirlingIsConnected)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return StirlingEngine custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(StirlingEngine), nameof(StirlingEngine.GetLogicValue))]
    public static class StirlingEngineGetLogicValuePatch
    {
        public static bool Prefix(StirlingEngine __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.StirlingHotTemperature || value > (ushort)SLELogicType.StirlingIsConnected)
                return true;

            // Get atmospheres
            var hotAtmosphere = __instance.InputNetwork?.Atmosphere;
            var coldAtmosphere = __instance.OutputNetwork?.Atmosphere;

            switch ((SLELogicType)value)
            {
                case SLELogicType.StirlingHotTemperature:
                    __result = hotAtmosphere?.Temperature.ToDouble() ?? 0;
                    return false;

                case SLELogicType.StirlingColdTemperature:
                    __result = coldAtmosphere?.Temperature.ToDouble() ?? 0;
                    return false;

                case SLELogicType.StirlingTemperatureDelta:
                    var hotTemp = hotAtmosphere?.Temperature.ToDouble() ?? 0;
                    var coldTemp = coldAtmosphere?.Temperature.ToDouble() ?? 0;
                    __result = Math.Abs(hotTemp - coldTemp);
                    return false;

                case SLELogicType.StirlingEfficiency:
                    // Stirling efficiency depends on temperature differential
                    if (hotAtmosphere == null || coldAtmosphere == null)
                    {
                        __result = 0;
                    }
                    else
                    {
                        var hot = hotAtmosphere.Temperature.ToDouble();
                        var cold = coldAtmosphere.Temperature.ToDouble();
                        if (hot <= 0 || cold <= 0 || hot <= cold)
                        {
                            __result = 0;
                        }
                        else
                        {
                            // Carnot efficiency approximation
                            __result = Math.Min(1.0, (hot - cold) / hot);
                        }
                    }
                    return false;

                case SLELogicType.StirlingMaxPower:
                    // Return max power based on current conditions
                    __result = __instance.MaxPower.ToDouble();
                    return false;

                case SLELogicType.StirlingIsConnected:
                    __result = (hotAtmosphere != null && coldAtmosphere != null) ? 1 : 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
