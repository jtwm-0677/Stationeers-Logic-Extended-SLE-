using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for SolarPanel to expose hidden LogicTypes.
    /// Works for all adjustable solar panel variants (excludes basic non-movable panels).
    /// </summary>
    public static class SolarPanelPatches
    {
        // Cached reflection for private/protected properties
        private static readonly PropertyInfo HealthProperty =
            typeof(SolarPanel).GetProperty("Health", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly PropertyInfo EfficiencyProperty =
            typeof(SolarPanel).GetProperty("Efficiency", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly PropertyInfo IsOperableProperty =
            typeof(SolarPanel).GetProperty("IsOperable", BindingFlags.NonPublic | BindingFlags.Instance);

        public static int GetHealth(SolarPanel panel)
        {
            return (int?)HealthProperty?.GetValue(panel) ?? 0;
        }

        public static int GetEfficiency(SolarPanel panel)
        {
            return (int?)EfficiencyProperty?.GetValue(panel) ?? 0;
        }

        public static bool GetIsOperable(SolarPanel panel)
        {
            return (bool?)IsOperableProperty?.GetValue(panel) ?? false;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading SolarPanel custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(SolarPanel), nameof(SolarPanel.CanLogicRead))]
    public static class SolarPanelCanLogicReadPatch
    {
        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;

            // Check range: SolarVisibility (1220) through SolarMovementSpeedV (1228)
            if (value >= (ushort)SLELogicType.SolarVisibility && value <= (ushort)SLELogicType.SolarMovementSpeedV)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return SolarPanel custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(SolarPanel), nameof(SolarPanel.GetLogicValue))]
    public static class SolarPanelGetLogicValuePatch
    {
        public static bool Prefix(SolarPanel __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1220-1228)
            if (value < (ushort)SLELogicType.SolarVisibility || value > (ushort)SLELogicType.SolarMovementSpeedV)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.SolarVisibility:
                    // Sun visibility factor 0-1 (affected by obstructions)
                    __result = __instance.SolarVisibility;
                    break;

                case SLELogicType.SolarDamageRatio:
                    // Damage ratio from DamageState (0=undamaged, 1=fully damaged)
                    __result = __instance.DamageState?.TotalRatio ?? 0;
                    break;

                case SLELogicType.SolarDamageTotal:
                    // Total damage points
                    __result = __instance.DamageState?.Total ?? 0;
                    break;

                case SLELogicType.SolarHealth:
                    // Health as percentage 0-100
                    __result = SolarPanelPatches.GetHealth(__instance);
                    break;

                case SLELogicType.SolarEfficiency:
                    // Efficiency as percentage 0-100 (includes damage)
                    __result = SolarPanelPatches.GetEfficiency(__instance);
                    break;

                case SLELogicType.SolarIsOperable:
                    // Panel is operable (not broken, powered, etc.)
                    __result = SolarPanelPatches.GetIsOperable(__instance) ? 1 : 0;
                    break;

                case SLELogicType.SolarIsBroken:
                    // Panel is broken
                    __result = __instance.IsBroken ? 1 : 0;
                    break;

                case SLELogicType.SolarMovementSpeedH:
                    // Horizontal rotation speed
                    __result = __instance.MovementSpeedHorizontal;
                    break;

                case SLELogicType.SolarMovementSpeedV:
                    // Vertical rotation speed
                    __result = __instance.MovementSpeedVertical;
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
