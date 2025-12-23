using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;
using Weather;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for WeatherStation to add weather data LogicTypes.
    /// Adds WeatherWindStrength, DaysSinceLastWeatherEvent.
    /// </summary>
    public static class WeatherStationPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading weather data LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(WeatherStation), nameof(WeatherStation.CanLogicRead))]
    public static class WeatherStationCanLogicReadPatch
    {
        public static void Postfix(WeatherStation __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to WeatherStation itself, not derived types
            if (__instance.GetType() != typeof(WeatherStation))
                return;

            ushort value = (ushort)logicType;
            if (value >= (ushort)SLELogicType.WeatherWindStrength && value <= (ushort)SLELogicType.DaysSinceLastWeatherEvent)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return weather data values.
    /// </summary>
    [HarmonyPatch(typeof(WeatherStation), nameof(WeatherStation.GetLogicValue))]
    public static class WeatherStationGetLogicValuePatch
    {
        public static bool Prefix(LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            switch ((SLELogicType)value)
            {
                case SLELogicType.WeatherWindStrength:
                    // Current weather wind strength (0 if no weather event)
                    if (WeatherManager.CurrentWeatherEvent != null && WeatherManager.IsWeatherEventRunning)
                    {
                        __result = WeatherManager.CurrentWeatherEvent.WindStrength;
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                case SLELogicType.DaysSinceLastWeatherEvent:
                    __result = WeatherManager.DaysSinceLastWeatherEvent;
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
