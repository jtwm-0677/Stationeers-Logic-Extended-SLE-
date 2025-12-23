using System.Reflection;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for RocketMiner to add custom LogicTypes.
    /// Exposes mining progress, ore type, quantity, and operational status.
    /// </summary>
    public static class RocketMinerPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading RocketMiner custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(RocketMiner), nameof(RocketMiner.CanLogicRead))]
    public static class RocketMinerCanLogicReadPatch
    {
        public static void Postfix(RocketMiner __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: RocketMiningProgress (1800) through RocketIsMining (1803)
            if (value >= (ushort)SLELogicType.RocketMiningProgress && value <= (ushort)SLELogicType.RocketIsMining)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return RocketMiner custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(RocketMiner), nameof(RocketMiner.GetLogicValue))]
    public static class RocketMinerGetLogicValuePatch
    {
        // Cache reflection for private fields
        private static readonly FieldInfo MiningProgressField = typeof(RocketMiner).GetField("_miningProgress", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo NextOreHashField = typeof(RocketMiner).GetField("_nextOreHash", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo MiningQuantityField = typeof(RocketMiner).GetField("_miningQuantity", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IsMiningField = typeof(RocketMiner).GetField("_isMining", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(RocketMiner __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.RocketMiningProgress || value > (ushort)SLELogicType.RocketIsMining)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.RocketMiningProgress:
                    var miningProgress = MiningProgressField?.GetValue(__instance);
                    __result = miningProgress != null ? (float)miningProgress : 0;
                    return false;

                case SLELogicType.RocketNextOreHash:
                    var nextOreHash = NextOreHashField?.GetValue(__instance);
                    __result = nextOreHash != null ? (int)nextOreHash : 0;
                    return false;

                case SLELogicType.RocketMiningQuantity:
                    var miningQuantity = MiningQuantityField?.GetValue(__instance);
                    __result = miningQuantity != null ? (int)miningQuantity : 0;
                    return false;

                case SLELogicType.RocketIsMining:
                    var isMining = IsMiningField?.GetValue(__instance);
                    __result = (isMining is bool mining && mining) ? 1 : 0;
                    return false;

                default:
                    return true;
            }
        }
    }
}
