using System.Reflection;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using Objects.Electrical;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for AdvancedComposter to expose hidden LogicTypes.
    /// Reveals grinding progress, batch progress, compost type counts, power usage, and status.
    /// </summary>
    public static class AdvancedComposterPatches
    {
        // Cached reflection for private fields
        private static readonly FieldInfo CurrentProgressField =
            typeof(AdvancedComposter).GetField("_currentProgress", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo TotalProcessedSecondsField =
            typeof(AdvancedComposter).GetField("_totalFertilizerProcessedSeconds", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo PowerUsedField =
            typeof(AdvancedComposter).GetField("_powerUsedDuringTick", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly PropertyInfo IsOperableProperty =
            typeof(AdvancedComposter).GetProperty("IsOperable", BindingFlags.NonPublic | BindingFlags.Instance);

        public static float GetCurrentProgress(AdvancedComposter composter)
        {
            return (float?)CurrentProgressField?.GetValue(composter) ?? 0f;
        }

        public static float GetTotalProcessedSeconds(AdvancedComposter composter)
        {
            return (float?)TotalProcessedSecondsField?.GetValue(composter) ?? 0f;
        }

        public static float GetPowerUsed(AdvancedComposter composter)
        {
            return (float?)PowerUsedField?.GetValue(composter) ?? 0f;
        }

        public static bool GetIsOperable(AdvancedComposter composter)
        {
            return (bool?)IsOperableProperty?.GetValue(composter) ?? false;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading AdvancedComposter custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(AdvancedComposter), nameof(AdvancedComposter.CanLogicRead))]
    public static class AdvancedComposterCanLogicReadPatch
    {
        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;

            // Check range: ComposterGrindProgress (1620) through ComposterIsOperable (1627)
            if (value >= (ushort)SLELogicType.ComposterGrindProgress && value <= (ushort)SLELogicType.ComposterIsOperable)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return AdvancedComposter custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(AdvancedComposter), nameof(AdvancedComposter.GetLogicValue))]
    public static class AdvancedComposterGetLogicValuePatch
    {
        public static bool Prefix(AdvancedComposter __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1620-1627)
            if (value < (ushort)SLELogicType.ComposterGrindProgress || value > (ushort)SLELogicType.ComposterIsOperable)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.ComposterGrindProgress:
                    // Grinding progress for current item (0 to 1.5 seconds)
                    __result = AdvancedComposterPatches.GetCurrentProgress(__instance);
                    break;

                case SLELogicType.ComposterBatchProgress:
                    // Batch processing time (0 to 60 seconds)
                    __result = AdvancedComposterPatches.GetTotalProcessedSeconds(__instance);
                    break;

                case SLELogicType.ComposterDecayCount:
                    // Count of decay food items (boost GrowthSpeed)
                    __result = __instance.DecayFoodQuantity;
                    break;

                case SLELogicType.ComposterNormalCount:
                    // Count of normal food items (boost HarvestQuantity)
                    __result = __instance.NormalFoodQuantity;
                    break;

                case SLELogicType.ComposterBiomassCount:
                    // Count of biomass items (boost GrowthCycles)
                    __result = __instance.BiomassQuantity;
                    break;

                case SLELogicType.ComposterPowerUsed:
                    // Power consumed this tick
                    __result = AdvancedComposterPatches.GetPowerUsed(__instance);
                    break;

                case SLELogicType.ComposterCanProcess:
                    // Ready to process (has 3+ items)
                    __result = __instance.CanDoProcessing ? 1 : 0;
                    break;

                case SLELogicType.ComposterIsOperable:
                    // Device is operable (input connected)
                    __result = AdvancedComposterPatches.GetIsOperable(__instance) ? 1 : 0;
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
