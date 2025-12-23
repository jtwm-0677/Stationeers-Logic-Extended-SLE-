using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for Recycler to add custom LogicTypes.
    /// Exposes reagent total, export state, capacity, and idle time.
    /// </summary>
    public static class RecyclerPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading Recycler custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(Recycler), nameof(Recycler.CanLogicRead))]
    public static class RecyclerCanLogicReadPatch
    {
        public static void Postfix(Recycler __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: RecyclerReagentTotal (1760) through RecyclerIsProcessing (1764)
            if (value >= (ushort)SLELogicType.RecyclerReagentTotal && value <= (ushort)SLELogicType.RecyclerIsProcessing)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return Recycler custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(Recycler), nameof(Recycler.GetLogicValue))]
    public static class RecyclerGetLogicValuePatch
    {
        // Cache reflection for private fields
        private static readonly FieldInfo ReagentTotalField = typeof(Recycler).GetField("_reagentTotal", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IsExportingField = typeof(Recycler).GetField("_isExporting", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo AtCapacityField = typeof(Recycler).GetField("_atCapacity", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IdleTicksField = typeof(Recycler).GetField("_idleTicks", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IsProcessingField = typeof(Recycler).GetField("_isProcessing", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(Recycler __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.RecyclerReagentTotal || value > (ushort)SLELogicType.RecyclerIsProcessing)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.RecyclerReagentTotal:
                    var reagentTotal = ReagentTotalField?.GetValue(__instance);
                    __result = reagentTotal != null ? (float)reagentTotal : 0;
                    return false;

                case SLELogicType.RecyclerIsExporting:
                    var isExporting = IsExportingField?.GetValue(__instance);
                    __result = (isExporting is bool exporting && exporting) ? 1 : 0;
                    return false;

                case SLELogicType.RecyclerAtCapacity:
                    var atCapacity = AtCapacityField?.GetValue(__instance);
                    __result = (atCapacity is bool capacity && capacity) ? 1 : 0;
                    return false;

                case SLELogicType.RecyclerIdleTicks:
                    var idleTicks = IdleTicksField?.GetValue(__instance);
                    __result = idleTicks != null ? (int)idleTicks : 0;
                    return false;

                case SLELogicType.RecyclerIsProcessing:
                    var isProcessing = IsProcessingField?.GetValue(__instance);
                    __result = (isProcessing is bool processing && processing) ? 1 : 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
