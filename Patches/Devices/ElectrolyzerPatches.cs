using System.Reflection;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for ElectrolysisMachine (Electrolyzer) to expose hidden LogicTypes.
    /// Reveals processed moles, power usage, operability, and IC error state.
    /// Only applies to ElectrolysisMachine itself, not derived types like H2Combustor.
    /// </summary>
    public static class ElectrolyzerPatches
    {
        // Cached reflection for protected properties/fields
        private static readonly PropertyInfo IsOperableProperty =
            typeof(ElectrolysisMachine).GetProperty("IsOperable", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo CodeErrorStateField =
            typeof(DeviceInputOutputCircuit).GetField("CodeErrorState", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool GetIsOperable(ElectrolysisMachine machine)
        {
            return (bool?)IsOperableProperty?.GetValue(machine) ?? false;
        }

        public static int GetCodeErrorState(ElectrolysisMachine machine)
        {
            return (int?)CodeErrorStateField?.GetValue(machine) ?? 0;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading Electrolyzer custom LogicTypes.
    /// Must patch DeviceInputOutput since ElectrolysisMachine doesn't override CanLogicRead.
    /// </summary>
    [HarmonyPatch(typeof(DeviceInputOutput), nameof(DeviceInputOutput.CanLogicRead))]
    public static class ElectrolyzerCanLogicReadPatch
    {
        public static void Postfix(DeviceInputOutput __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to ElectrolysisMachine itself, not derived types like H2Combustor
            if (__instance.GetType() != typeof(ElectrolysisMachine))
                return;

            ushort value = (ushort)logicType;

            // Check range: ElectrolyzerProcessedMoles (1240) through ElectrolyzerCodeError (1243)
            if (value >= (ushort)SLELogicType.ElectrolyzerProcessedMoles && value <= (ushort)SLELogicType.ElectrolyzerCodeError)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return Electrolyzer custom LogicType values.
    /// Must patch DeviceInputOutput since ElectrolysisMachine doesn't override GetLogicValue.
    /// </summary>
    [HarmonyPatch(typeof(DeviceInputOutput), nameof(DeviceInputOutput.GetLogicValue))]
    public static class ElectrolyzerGetLogicValuePatch
    {
        public static bool Prefix(DeviceInputOutput __instance, LogicType logicType, ref double __result)
        {
            // Only apply to ElectrolysisMachine itself, not derived types like H2Combustor
            if (__instance.GetType() != typeof(ElectrolysisMachine))
                return true;

            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1240-1243)
            if (value < (ushort)SLELogicType.ElectrolyzerProcessedMoles || value > (ushort)SLELogicType.ElectrolyzerCodeError)
                return true;

            var electrolyzer = (ElectrolysisMachine)__instance;

            switch ((SLELogicType)value)
            {
                case SLELogicType.ElectrolyzerProcessedMoles:
                    // Moles processed this tick
                    __result = electrolyzer.ProcessedMoles.ToFloat();
                    break;

                case SLELogicType.ElectrolyzerUsedPower:
                    // Power consumed this tick
                    __result = electrolyzer.UsedPower;
                    break;

                case SLELogicType.ElectrolyzerIsOperable:
                    // Whether device is operable
                    __result = ElectrolyzerPatches.GetIsOperable(electrolyzer) ? 1 : 0;
                    break;

                case SLELogicType.ElectrolyzerCodeError:
                    // IC chip error state
                    __result = ElectrolyzerPatches.GetCodeErrorState(electrolyzer);
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
