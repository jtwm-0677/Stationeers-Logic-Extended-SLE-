using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for SolidFuelGenerator to add fuel status LogicTypes.
    /// </summary>
    public static class SolidFuelGeneratorPatches
    {
        // PoweredTicks is a protected field in PowerGeneratorSlot
        internal static readonly FieldInfo PoweredTicksField =
            typeof(PowerGeneratorSlot).GetField("PoweredTicks", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading SolidFuelGenerator LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(SolidFuelGenerator), nameof(SolidFuelGenerator.CanLogicRead))]
    public static class SolidFuelGeneratorCanLogicReadPatch
    {
        public static void Postfix(SolidFuelGenerator __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to SolidFuelGenerator itself, not derived types
            if (__instance.GetType() != typeof(SolidFuelGenerator))
                return;

            if ((ushort)logicType == (ushort)SLELogicType.FuelTicks)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return SolidFuelGenerator data values.
    /// </summary>
    [HarmonyPatch(typeof(SolidFuelGenerator), nameof(SolidFuelGenerator.GetLogicValue))]
    public static class SolidFuelGeneratorGetLogicValuePatch
    {
        public static bool Prefix(SolidFuelGenerator __instance, LogicType logicType, ref double __result)
        {
            if ((ushort)logicType == (ushort)SLELogicType.FuelTicks)
            {
                __result = (int)SolidFuelGeneratorPatches.PoweredTicksField.GetValue(__instance);
                return false;
            }

            return true; // Let original method handle
        }
    }
}
