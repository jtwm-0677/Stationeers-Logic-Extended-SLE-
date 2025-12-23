using System.Reflection;
using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for FurnaceBase to add internal atmosphere and state LogicTypes.
    /// Applies to Furnace, AdvancedFurnace, and any other FurnaceBase derivatives.
    /// </summary>
    public static class FurnacePatches
    {
        // Cached reflection for protected AtmosphericsController property
        private static PropertyInfo _atmosphericsControllerProperty;
        private static MethodInfo _sampleGlobalAtmosphereMethod;

        static FurnacePatches()
        {
            // AtmosphericsController is a protected property on Structure
            _atmosphericsControllerProperty = typeof(Assets.Scripts.Objects.Structure)
                .GetProperty("AtmosphericsController", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static Atmosphere GetExternalAtmosphere(FurnaceBase instance)
        {
            var controller = _atmosphericsControllerProperty?.GetValue(instance);
            if (controller == null) return null;

            // Cache the SampleGlobalAtmosphere method
            if (_sampleGlobalAtmosphereMethod == null)
            {
                _sampleGlobalAtmosphereMethod = controller.GetType()
                    .GetMethod("SampleGlobalAtmosphere", BindingFlags.Public | BindingFlags.Instance);
            }

            return _sampleGlobalAtmosphereMethod?.Invoke(controller, new object[] { instance.WorldGrid }) as Atmosphere;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading FurnaceBase custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(FurnaceBase), nameof(FurnaceBase.CanLogicRead))]
    public static class FurnaceBaseCanLogicReadPatch
    {
        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: FurnaceTemperature (1300) through FurnaceVolume (1311)
            if (value >= (ushort)SLELogicType.FurnaceTemperature && value <= (ushort)SLELogicType.FurnaceVolume)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return FurnaceBase custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(FurnaceBase), nameof(FurnaceBase.GetLogicValue))]
    public static class FurnaceBaseGetLogicValuePatch
    {
        // Maximum pressure differential constant from FurnaceBase
        private static readonly double MAXPressureDelta = 60795.0;

        public static bool Prefix(FurnaceBase __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.FurnaceTemperature || value > (ushort)SLELogicType.FurnaceVolume)
                return true;

            // Get the internal atmosphere
            var atmosphere = __instance.InternalAtmosphere;

            switch ((SLELogicType)value)
            {
                case SLELogicType.FurnaceTemperature:
                    __result = atmosphere?.Temperature.ToDouble() ?? 0;
                    return false;

                case SLELogicType.FurnacePressure:
                    __result = atmosphere?.PressureGassesAndLiquids.ToDouble() ?? 0;
                    return false;

                case SLELogicType.FurnaceTotalMoles:
                    __result = atmosphere?.TotalMoles.ToDouble() ?? 0;
                    return false;

                case SLELogicType.FurnaceInflamed:
                    __result = __instance.Inflamed ? 1 : 0;
                    return false;

                case SLELogicType.FurnaceMode:
                    __result = __instance.Mode;
                    return false;

                case SLELogicType.ReagentQuantity:
                    __result = __instance.ReagentMixture?.TotalReagents ?? 0;
                    return false;

                case SLELogicType.FurnaceOverpressure:
                    // Check if pressure differential exceeds max
                    if (atmosphere == null)
                    {
                        __result = 0;
                        return false;
                    }
                    var externalAtmo = FurnacePatches.GetExternalAtmosphere(__instance);
                    double externalPressure = externalAtmo?.PressureGassesAndLiquids.ToDouble() ?? 0;
                    double internalPressure = atmosphere.PressureGassesAndLiquids.ToDouble();
                    double pressureDiff = System.Math.Abs(externalPressure - internalPressure);
                    __result = pressureDiff > MAXPressureDelta ? 1 : 0;
                    return false;

                case SLELogicType.CurrentRecipeEnergy:
                    __result = __instance.CurrentRecipe.Energy;
                    return false;

                case SLELogicType.FurnaceStressed:
                    // Stressed when pressure > 66% of max
                    if (atmosphere == null)
                    {
                        __result = 0;
                        return false;
                    }
                    var extAtmo = FurnacePatches.GetExternalAtmosphere(__instance);
                    double extPressure = extAtmo?.PressureGassesAndLiquids.ToDouble() ?? 0;
                    double intPressure = atmosphere.PressureGassesAndLiquids.ToDouble();
                    double pDiff = System.Math.Abs(extPressure - intPressure);
                    __result = pDiff > (MAXPressureDelta * 0.66) ? 1 : 0;
                    return false;

                case SLELogicType.FurnaceHasBlown:
                    __result = __instance.HasBlown ? 1 : 0;
                    return false;

                case SLELogicType.FurnaceMaxPressure:
                    __result = MAXPressureDelta;
                    return false;

                case SLELogicType.FurnaceVolume:
                    __result = __instance.Volume.ToDouble();
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
