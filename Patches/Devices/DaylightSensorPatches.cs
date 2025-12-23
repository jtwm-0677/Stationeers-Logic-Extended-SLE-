using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;
using Weather;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for DaylightSensor to add realtime world data LogicTypes.
    /// Adds TimeOfDay, IsEclipse, EclipseRatio, DaysPast, DayLengthSeconds, Latitude, Longitude, WeatherSolarRatio.
    /// </summary>
    public static class DaylightSensorPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading realtime data LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(DaylightSensor), nameof(DaylightSensor.CanLogicRead))]
    public static class DaylightSensorCanLogicReadPatch
    {
        public static void Postfix(DaylightSensor __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to DaylightSensor itself, not derived types
            if (__instance.GetType() != typeof(DaylightSensor))
                return;

            ushort value = (ushort)logicType;
            if (value >= (ushort)SLELogicType.TimeOfDay && value <= (ushort)SLELogicType.WeatherSolarRatio)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return realtime data values.
    /// </summary>
    [HarmonyPatch(typeof(DaylightSensor), nameof(DaylightSensor.GetLogicValue))]
    public static class DaylightSensorGetLogicValuePatch
    {
        public static bool Prefix(LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            switch ((SLELogicType)value)
            {
                case SLELogicType.TimeOfDay:
                    __result = OrbitalSimulation.TimeOfDay;
                    return false;

                case SLELogicType.IsEclipse:
                    __result = OrbitalSimulation.IsEclipse ? 1 : 0;
                    return false;

                case SLELogicType.EclipseRatio:
                    __result = OrbitalSimulation.EclipseRatio;
                    return false;

                case SLELogicType.DaysPast:
                    __result = WorldManager.DaysPast;
                    return false;

                case SLELogicType.DayLengthSeconds:
                    __result = OrbitalSimulation.GetDayLengthSeconds();
                    return false;

                case SLELogicType.Latitude:
                    __result = OrbitalSimulation.Latitude;
                    return false;

                case SLELogicType.Longitude:
                    __result = OrbitalSimulation.Longitude;
                    return false;

                case SLELogicType.WeatherSolarRatio:
                    // Return 1.0 if no weather event, otherwise return the event's solar ratio
                    if (WeatherManager.CurrentWeatherEvent != null && WeatherManager.IsWeatherEventRunning)
                    {
                        __result = WeatherManager.CurrentWeatherEvent.SolarRatio;
                    }
                    else
                    {
                        __result = 1.0;
                    }
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
