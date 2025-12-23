using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for HydroponicsTrayDevice to add plant/light data LogicTypes.
    /// Adds LightExposure, IsLitByGrowLight, WaterMoles, and plant status data.
    /// Note: HydroponicsTrayDevice inherits CanLogicRead/GetLogicValue from Device,
    /// so we patch the parent class and check for HydroponicsTrayDevice instance.
    /// </summary>
    public static class HydroponicsPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead on parent class to allow reading HydroponicsTrayDevice LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(Device), nameof(Device.CanLogicRead), new[] { typeof(LogicType) })]
    public static class HydroponicsCanLogicReadPatch
    {
        public static void Postfix(Device __instance, ref bool __result, LogicType logicType)
        {
            // Only handle HydroponicsTrayDevice instances
            if (!(__instance is HydroponicsTrayDevice))
                return;

            ushort value = (ushort)logicType;
            // Check full range: LightExposure (1160) through HydrationEfficiency (1169)
            if (value >= (ushort)SLELogicType.LightExposure && value <= (ushort)SLELogicType.HydrationEfficiency)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue on parent class to return HydroponicsTrayDevice data values.
    /// </summary>
    [HarmonyPatch(typeof(Device), nameof(Device.GetLogicValue), new[] { typeof(LogicType) })]
    public static class HydroponicsGetLogicValuePatch
    {
        public static bool Prefix(Device __instance, LogicType logicType, ref double __result)
        {
            // Only handle HydroponicsTrayDevice instances
            if (!(__instance is HydroponicsTrayDevice hydro))
                return true;

            ushort value = (ushort)logicType;

            switch ((SLELogicType)value)
            {
                case SLELogicType.LightExposure:
                    // Current light exposure (grow light + solar contribution)
                    __result = hydro.CurrentLightExposure;
                    return false;

                case SLELogicType.IsLitByGrowLight:
                    // 1 if lit by powered grow light, 0 if not
                    __result = hydro.IsLitByGrowLight ? 1 : 0;
                    return false;

                case SLELogicType.WaterMoles:
                    // Water amount in internal atmosphere (total moles)
                    var waterAtmo = hydro.WaterAtmosphere;
                    if (waterAtmo != null)
                    {
                        __result = waterAtmo.TotalMoles.ToDouble();
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                case SLELogicType.PlantIsFertilized:
                    // 1 if plant has been fertilized, 0 if not
                    __result = (hydro.Plant != null && hydro.Plant.IsFertilized) ? 1 : 0;
                    return false;

                case SLELogicType.PlantGrowthEfficiency:
                    // Plant overall growth efficiency 0-100%
                    __result = hydro.Plant != null ? hydro.Plant.GrowthEfficiencyPercent : 0;
                    return false;

                case SLELogicType.BreathingEfficiency:
                    // Plant breathing/gas efficiency 0-100%
                    __result = (hydro.Plant != null && hydro.Plant.PlantStatus != null)
                        ? hydro.Plant.PlantStatus.BreathingEfficiencyPercent : 0;
                    return false;

                case SLELogicType.TemperatureEfficiency:
                    // Plant temperature efficiency 0-100%
                    __result = (hydro.Plant != null && hydro.Plant.PlantStatus != null)
                        ? hydro.Plant.PlantStatus.TemperatureEfficiencyPercent : 0;
                    return false;

                case SLELogicType.PlantLightEfficiency:
                    // Plant light efficiency 0-100%
                    __result = (hydro.Plant != null && hydro.Plant.PlantStatus != null)
                        ? hydro.Plant.PlantStatus.LightEfficiencyPercent : 0;
                    return false;

                case SLELogicType.PlantPressureEfficiency:
                    // Plant pressure efficiency 0-100%
                    __result = (hydro.Plant != null && hydro.Plant.PlantStatus != null)
                        ? hydro.Plant.PlantStatus.PressureEfficiencyPercent : 0;
                    return false;

                case SLELogicType.HydrationEfficiency:
                    // Plant hydration/water efficiency 0-100%
                    __result = (hydro.Plant != null && hydro.Plant.PlantStatus != null)
                        ? hydro.Plant.PlantStatus.HydrationEfficiencyPercent : 0;
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
