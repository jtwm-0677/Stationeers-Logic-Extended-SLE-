using System.Reflection;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using Objects;
using SLE.Core;
using Weather;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for WindTurbineGenerator to add wind and turbine data LogicTypes.
    /// Works for both WindTurbineGenerator and LargeWindTurbineGenerator.
    /// </summary>
    public static class WindTurbinePatches
    {
        internal static readonly FieldInfo TurbineRotationSpeedField =
            typeof(WindTurbineGenerator).GetField("_turbineRotationSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading wind turbine LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(WindTurbineGenerator), nameof(WindTurbineGenerator.CanLogicRead))]
    public static class WindTurbineCanLogicReadPatch
    {
        public static void Postfix(WindTurbineGenerator __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;

            // LogicTypes available on all wind turbines: WindSpeed (1120) through AtmosphericPressure (1123)
            if (value >= (ushort)SLELogicType.WindSpeed && value <= (ushort)SLELogicType.AtmosphericPressure)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return wind turbine data values.
    /// </summary>
    [HarmonyPatch(typeof(WindTurbineGenerator), nameof(WindTurbineGenerator.GetLogicValue))]
    public static class WindTurbineGetLogicValuePatch
    {
        public static bool Prefix(WindTurbineGenerator __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            switch ((SLELogicType)value)
            {
                case SLELogicType.WindSpeed:
                    // Global wind strength (static property)
                    __result = WindTurbineGenerator.WindStrength;
                    return false;

                case SLELogicType.MaxPower:
                    // Return storm max or normal max based on current weather
                    bool isStorm = WeatherManager.CurrentWeatherEvent != null
                                && WeatherManager.IsWeatherEventRunning
                                && WeatherManager.CurrentWeatherEvent.StormEffect != null;
                    __result = isStorm ? __instance.MaxPowerOutputStorm : __instance.MAXPowerOutput;
                    return false;

                case SLELogicType.TurbineSpeed:
                    // Access private field via reflection
                    __result = (float)WindTurbinePatches.TurbineRotationSpeedField.GetValue(__instance);
                    return false;

                case SLELogicType.AtmosphericPressure:
                    // Clamped atmospheric pressure used in power calculation
                    __result = __instance.GetWorldAtmospherePressureClamped().ToDouble();
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
