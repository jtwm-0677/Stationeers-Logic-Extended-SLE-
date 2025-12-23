using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using Objects.Electrical;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for HorizontalQuarry (Ogre) to add custom LogicTypes.
    /// Exposes mining state, ore count, position, and operational status.
    /// Note: HorizontalQuarry inherits from DeviceImportExport which defines CanLogicRead/GetLogicValue,
    /// so we patch that class and check for HorizontalQuarry instance.
    /// </summary>
    public static class HorizontalQuarryPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead on DeviceImportExport to allow reading HorizontalQuarry LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(DeviceImportExport), nameof(DeviceImportExport.CanLogicRead))]
    public static class HorizontalQuarryCanLogicReadPatch
    {
        public static void Postfix(DeviceImportExport __instance, ref bool __result, LogicType logicType)
        {
            // Only handle HorizontalQuarry instances
            if (!(__instance is HorizontalQuarry))
                return;

            ushort value = (ushort)logicType;
            // Check range: OgreState (1740) through OgreQueueFull (1745)
            if (value >= (ushort)SLELogicType.OgreState && value <= (ushort)SLELogicType.OgreQueueFull)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue on DeviceImportExport to return HorizontalQuarry data values.
    /// </summary>
    [HarmonyPatch(typeof(DeviceImportExport), nameof(DeviceImportExport.GetLogicValue))]
    public static class HorizontalQuarryGetLogicValuePatch
    {
        // Cache reflection for private fields
        private static readonly FieldInfo StateField = typeof(HorizontalQuarry).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo OreQueueField = typeof(HorizontalQuarry).GetField("_oreQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo PositionField = typeof(HorizontalQuarry).GetField("_position", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo MiningCompleteField = typeof(HorizontalQuarry).GetField("_miningComplete", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IsReturningField = typeof(HorizontalQuarry).GetField("_isReturning", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo QueueFullField = typeof(HorizontalQuarry).GetField("_queueFull", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(DeviceImportExport __instance, LogicType logicType, ref double __result)
        {
            // Only handle HorizontalQuarry instances
            if (!(__instance is HorizontalQuarry quarry))
                return true;

            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.OgreState || value > (ushort)SLELogicType.OgreQueueFull)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.OgreState:
                    var state = StateField?.GetValue(quarry);
                    __result = state != null ? (int)state : 0;
                    return false;

                case SLELogicType.OgreOreCount:
                    var oreQueue = OreQueueField?.GetValue(quarry) as Queue<int>;
                    __result = oreQueue?.Count ?? 0;
                    return false;

                case SLELogicType.OgrePosition:
                    var position = PositionField?.GetValue(quarry);
                    __result = position != null ? (float)position : 0;
                    return false;

                case SLELogicType.OgreMiningComplete:
                    var miningComplete = MiningCompleteField?.GetValue(quarry);
                    __result = (miningComplete is bool complete && complete) ? 1 : 0;
                    return false;

                case SLELogicType.OgreIsReturning:
                    var isReturning = IsReturningField?.GetValue(quarry);
                    __result = (isReturning is bool returning && returning) ? 1 : 0;
                    return false;

                case SLELogicType.OgreQueueFull:
                    var queueFull = QueueFullField?.GetValue(quarry);
                    __result = (queueFull is bool full && full) ? 1 : 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
