using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for AdvancedFurnace to add setting bounds LogicTypes.
    /// </summary>
    public static class AdvancedFurnacePatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading AdvancedFurnace custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(AdvancedFurnace), nameof(AdvancedFurnace.CanLogicRead))]
    public static class AdvancedFurnaceCanLogicReadPatch
    {
        public static void Postfix(AdvancedFurnace __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to AdvancedFurnace itself, not derived types
            if (__instance.GetType() != typeof(AdvancedFurnace))
                return;

            ushort value = (ushort)logicType;
            // Check range: MinSettingInput (1320) through MaxSettingOutput (1323)
            if (value >= (ushort)SLELogicType.MinSettingInput && value <= (ushort)SLELogicType.MaxSettingOutput)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return AdvancedFurnace custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(AdvancedFurnace), nameof(AdvancedFurnace.GetLogicValue))]
    public static class AdvancedFurnaceGetLogicValuePatch
    {
        public static bool Prefix(AdvancedFurnace __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.MinSettingInput || value > (ushort)SLELogicType.MaxSettingOutput)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.MinSettingInput:
                    __result = __instance.MinSetting2;
                    return false;

                case SLELogicType.MaxSettingInput:
                    __result = __instance.MaxSetting2;
                    return false;

                case SLELogicType.MinSettingOutput:
                    __result = __instance.MinSetting;
                    return false;

                case SLELogicType.MaxSettingOutput:
                    __result = __instance.MaxSetting;
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
