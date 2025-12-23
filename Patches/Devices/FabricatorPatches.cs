using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for SimpleFabricatorBase (all printers/fabricators) to expose hidden LogicTypes.
    /// Reveals recipe index, count, tier, time multiplier, power usage, and making index.
    /// </summary>
    public static class FabricatorPatches
    {
        // Cached reflection for private fields
        private static readonly FieldInfo PowerUsedField =
            typeof(SimpleFabricatorBase).GetField("_powerUsedDuringTick", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo MakingIndexField =
            typeof(SimpleFabricatorBase).GetField("_makingIndex", BindingFlags.NonPublic | BindingFlags.Instance);

        public static float GetPowerUsed(SimpleFabricatorBase fabricator)
        {
            return (float?)PowerUsedField?.GetValue(fabricator) ?? 0f;
        }

        public static int GetMakingIndex(SimpleFabricatorBase fabricator)
        {
            return (int?)MakingIndexField?.GetValue(fabricator) ?? -1;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading Fabricator custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(SimpleFabricatorBase), nameof(SimpleFabricatorBase.CanLogicRead))]
    public static class FabricatorCanLogicReadPatch
    {
        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;

            // Check range: FabricatorCurrentIndex (1610) through FabricatorMakingIndex (1615)
            if (value >= (ushort)SLELogicType.FabricatorCurrentIndex && value <= (ushort)SLELogicType.FabricatorMakingIndex)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return Fabricator custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(SimpleFabricatorBase), nameof(SimpleFabricatorBase.GetLogicValue))]
    public static class FabricatorGetLogicValuePatch
    {
        public static bool Prefix(SimpleFabricatorBase __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1610-1615)
            if (value < (ushort)SLELogicType.FabricatorCurrentIndex || value > (ushort)SLELogicType.FabricatorMakingIndex)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.FabricatorCurrentIndex:
                    // Index of currently selected recipe
                    __result = __instance.CurrentIndex;
                    break;

                case SLELogicType.FabricatorRecipeCount:
                    // Total number of available recipes at current tier
                    __result = __instance.ValidDynamicThings?.Count ?? 0;
                    break;

                case SLELogicType.FabricatorCurrentTier:
                    // Current machine tier (0-2)
                    __result = (int)__instance.CurrentTier;
                    break;

                case SLELogicType.FabricatorTimeMultiplier:
                    // Build time multiplier
                    __result = __instance.ManufactureTimeMultiplier;
                    break;

                case SLELogicType.FabricatorPowerUsed:
                    // Power consumed this tick
                    __result = FabricatorPatches.GetPowerUsed(__instance);
                    break;

                case SLELogicType.FabricatorMakingIndex:
                    // Index of recipe being fabricated
                    __result = FabricatorPatches.GetMakingIndex(__instance);
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
