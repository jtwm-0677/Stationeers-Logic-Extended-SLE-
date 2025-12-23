using System.Reflection;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for ArcFurnace to add smelting state and power LogicTypes.
    /// </summary>
    public static class ArcFurnacePatches
    {
        // Cached reflection for private fields
        private static FieldInfo _smeltingTaskField;
        private static FieldInfo _powerUsedField;
        private static PropertyInfo _taskStatusProperty;

        static ArcFurnacePatches()
        {
            var arcFurnaceType = typeof(ArcFurnace);
            _smeltingTaskField = arcFurnaceType.GetField("_smeltingTask", BindingFlags.NonPublic | BindingFlags.Instance);
            _powerUsedField = arcFurnaceType.GetField("_powerUsedDuringTick", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static int GetSmeltingTaskStatus(ArcFurnace instance)
        {
            var task = _smeltingTaskField?.GetValue(instance);
            if (task == null) return 0;

            // Get Status property via reflection (UniTask.Status)
            if (_taskStatusProperty == null)
            {
                _taskStatusProperty = task.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
            }

            var status = _taskStatusProperty?.GetValue(task);
            return status != null ? (int)status : 0;
        }

        public static float GetPowerUsed(ArcFurnace instance)
        {
            return (float?)_powerUsedField?.GetValue(instance) ?? 0f;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading ArcFurnace custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(ArcFurnace), nameof(ArcFurnace.CanLogicRead))]
    public static class ArcFurnaceCanLogicReadPatch
    {
        public static void Postfix(ArcFurnace __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to ArcFurnace itself, not derived types
            if (__instance.GetType() != typeof(ArcFurnace))
                return;

            ushort value = (ushort)logicType;
            // Check range: ArcFurnaceActivate (1330) through ArcFurnaceIsSmelting (1333)
            if (value >= (ushort)SLELogicType.ArcFurnaceActivate && value <= (ushort)SLELogicType.ArcFurnaceIsSmelting)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return ArcFurnace custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(ArcFurnace), nameof(ArcFurnace.GetLogicValue))]
    public static class ArcFurnaceGetLogicValuePatch
    {
        public static bool Prefix(ArcFurnace __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.ArcFurnaceActivate || value > (ushort)SLELogicType.ArcFurnaceIsSmelting)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.ArcFurnaceActivate:
                    // Activate state - 0=idle, 1=smelting
                    __result = __instance.Activate;
                    return false;

                case SLELogicType.ImportStackSize:
                    // CurrentStackSize property - quantity in import slot
                    __result = __instance.CurrentStackSize;
                    return false;

                case SLELogicType.SmeltingPower:
                    // Power being consumed for smelting this tick
                    __result = ArcFurnacePatches.GetPowerUsed(__instance);
                    return false;

                case SLELogicType.ArcFurnaceIsSmelting:
                    // Check if smelting task is running (Status > 0 means running)
                    var taskStatus = ArcFurnacePatches.GetSmeltingTaskStatus(__instance);
                    __result = taskStatus > 0 ? 1 : 0;
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
