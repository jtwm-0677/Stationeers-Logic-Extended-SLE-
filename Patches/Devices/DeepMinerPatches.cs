using System.Reflection;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for DeepMiner to add mining data LogicTypes.
    /// Adds MiningProgress, CurrentOreHash.
    /// Note: DeepMiner inherits CanLogicRead/GetLogicValue from DeviceInputOutputImportExport,
    /// so we patch the parent class and check for DeepMiner instance.
    /// </summary>
    public static class DeepMinerPatches
    {
        // Try to find the mining progress field via reflection
        internal static readonly FieldInfo MiningTimeField =
            typeof(DeepMiner).GetField("_miningTime", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo MiningDurationField =
            typeof(DeepMiner).GetField("_miningDuration", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    /// <summary>
    /// Patch CanLogicRead on parent class to allow reading DeepMiner LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(DeviceInputOutputImportExport), nameof(DeviceInputOutputImportExport.CanLogicRead))]
    public static class DeepMinerCanLogicReadPatch
    {
        public static void Postfix(DeviceInputOutputImportExport __instance, ref bool __result, LogicType logicType)
        {
            // Only handle DeepMiner instances
            if (!(__instance is DeepMiner))
                return;

            ushort value = (ushort)logicType;
            if (value >= (ushort)SLELogicType.MiningProgress && value <= (ushort)SLELogicType.CurrentOreHash)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue on parent class to return DeepMiner data values.
    /// </summary>
    [HarmonyPatch(typeof(DeviceInputOutputImportExport), nameof(DeviceInputOutputImportExport.GetLogicValue))]
    public static class DeepMinerGetLogicValuePatch
    {
        public static bool Prefix(DeviceInputOutputImportExport __instance, LogicType logicType, ref double __result)
        {
            // Only handle DeepMiner instances
            if (!(__instance is DeepMiner deepMiner))
                return true;

            ushort value = (ushort)logicType;

            switch ((SLELogicType)value)
            {
                case SLELogicType.MiningProgress:
                    // Mining cycle progress 0-100%
                    if (DeepMinerPatches.MiningTimeField != null && DeepMinerPatches.MiningDurationField != null)
                    {
                        float time = (float)DeepMinerPatches.MiningTimeField.GetValue(deepMiner);
                        float duration = (float)DeepMinerPatches.MiningDurationField.GetValue(deepMiner);
                        if (duration > 0)
                        {
                            __result = (time / duration) * 100.0;
                        }
                        else
                        {
                            __result = 0;
                        }
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                case SLELogicType.CurrentOreHash:
                    // Hash of current ore type in export slot
                    var exportSlot = deepMiner.ExportSlot;
                    var ore = exportSlot?.Get();
                    if (ore != null)
                    {
                        __result = ore.PrefabHash;
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
