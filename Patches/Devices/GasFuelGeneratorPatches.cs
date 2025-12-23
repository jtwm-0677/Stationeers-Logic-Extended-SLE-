using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for GasFuelGenerator to add combustion/operational data LogicTypes.
    /// Adds CombustionEnergy, IsValidAtmosphere, DoShutdown, MinTemperature, MaxTemperature, MinPressure.
    /// Note: GasFuelGenerator inherits CanLogicRead/GetLogicValue from PowerGeneratorPipe,
    /// so we patch the parent class and check for GasFuelGenerator instance.
    /// </summary>
    public static class GasFuelGeneratorPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead on parent class to allow reading GasFuelGenerator LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(PowerGeneratorPipe), nameof(PowerGeneratorPipe.CanLogicRead))]
    public static class GasFuelGeneratorCanLogicReadPatch
    {
        public static void Postfix(PowerGeneratorPipe __instance, ref bool __result, LogicType logicType)
        {
            // Only handle GasFuelGenerator instances
            if (!(__instance is GasFuelGenerator))
                return;

            ushort value = (ushort)logicType;
            if (value >= (ushort)SLELogicType.CombustionEnergy && value <= (ushort)SLELogicType.MinPressure)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue on parent class to return GasFuelGenerator data values.
    /// </summary>
    [HarmonyPatch(typeof(PowerGeneratorPipe), nameof(PowerGeneratorPipe.GetLogicValue))]
    public static class GasFuelGeneratorGetLogicValuePatch
    {
        public static bool Prefix(PowerGeneratorPipe __instance, LogicType logicType, ref double __result)
        {
            // Only handle GasFuelGenerator instances
            if (!(__instance is GasFuelGenerator))
                return true;

            ushort value = (ushort)logicType;

            switch ((SLELogicType)value)
            {
                case SLELogicType.CombustionEnergy:
                    // Combustion energy produced (before 17% efficiency conversion)
                    var atmo = __instance.InternalAtmosphere;
                    if (atmo != null)
                    {
                        __result = atmo.CombustionEnergy.ToDouble();
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                case SLELogicType.IsValidAtmosphere:
                    // 1 if atmosphere meets pressure/temp requirements, 0 if not
                    __result = __instance.IsValidAtmosphere ? 1 : 0;
                    return false;

                case SLELogicType.DoShutdown:
                    // 1 if conditions will trigger shutdown, 0 if not
                    __result = __instance.DoShutdown ? 1 : 0;
                    return false;

                case SLELogicType.MinTemperature:
                    // Minimum operating temperature in Kelvin
                    __result = __instance.MinimumTemperature.ToDouble();
                    return false;

                case SLELogicType.MaxTemperature:
                    // Maximum operating temperature in Kelvin
                    __result = __instance.MaximumTemperature.ToDouble();
                    return false;

                case SLELogicType.MinPressure:
                    // Minimum operating pressure in Pa
                    __result = __instance.minimumPressure;
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
