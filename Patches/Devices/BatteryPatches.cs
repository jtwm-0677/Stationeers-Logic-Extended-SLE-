using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for Battery to expose hidden LogicTypes.
    /// Reveals power delta, submersion state, and charge status.
    /// </summary>
    public static class BatteryPatches
    {
        // Cached reflection for private fields from ElectricalInputOutput
        private static readonly FieldInfo InputSubmergedField =
            typeof(ElectricalInputOutput).GetField("_inputSubmerged", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo OutputSubmergedField =
            typeof(ElectricalInputOutput).GetField("_outputSubmerged", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Get input submerged ticks count.
        /// </summary>
        public static uint GetInputSubmergedTicks(Battery battery)
        {
            return (uint?)InputSubmergedField?.GetValue(battery) ?? 0;
        }

        /// <summary>
        /// Get output submerged ticks count.
        /// </summary>
        public static uint GetOutputSubmergedTicks(Battery battery)
        {
            return (uint?)OutputSubmergedField?.GetValue(battery) ?? 0;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading Battery custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(Battery), nameof(Battery.CanLogicRead))]
    public static class BatteryCanLogicReadPatch
    {
        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;

            // Check range: PowerDelta (1210) through BatteryIsCharged (1215)
            if (value >= (ushort)SLELogicType.PowerDelta && value <= (ushort)SLELogicType.BatteryIsCharged)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return Battery custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(Battery), nameof(Battery.GetLogicValue))]
    public static class BatteryGetLogicValuePatch
    {
        public static bool Prefix(Battery __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1210-1215)
            if (value < (ushort)SLELogicType.PowerDelta || value > (ushort)SLELogicType.BatteryIsCharged)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.PowerDelta:
                    // PowerStored - PowerMaximum (negative when not full)
                    __result = __instance.PowerDelta;
                    break;

                case SLELogicType.BatteryIsSubmerged:
                    // From ElectricalInputOutput base class
                    __result = __instance.IsSubmerged ? 1 : 0;
                    break;

                case SLELogicType.InputSubmergedTicks:
                    __result = BatteryPatches.GetInputSubmergedTicks(__instance);
                    break;

                case SLELogicType.OutputSubmergedTicks:
                    __result = BatteryPatches.GetOutputSubmergedTicks(__instance);
                    break;

                case SLELogicType.BatteryIsEmpty:
                    __result = __instance.IsEmpty ? 1 : 0;
                    break;

                case SLELogicType.BatteryIsCharged:
                    __result = __instance.IsCharged ? 1 : 0;
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
