using System.Reflection;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for H2CombustorMachine to expose hidden LogicTypes.
    /// Reveals processed moles, power usage, operability, and IC error state.
    /// </summary>
    public static class H2CombustorPatches
    {
        // Cached reflection for protected properties/fields
        private static readonly PropertyInfo IsOperableProperty =
            typeof(ElectrolysisMachine).GetProperty("IsOperable", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo CodeErrorStateField =
            typeof(DeviceInputOutputCircuit).GetField("CodeErrorState", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool GetIsOperable(H2CombustorMachine machine)
        {
            return (bool?)IsOperableProperty?.GetValue(machine) ?? false;
        }

        public static int GetCodeErrorState(H2CombustorMachine machine)
        {
            return (int?)CodeErrorStateField?.GetValue(machine) ?? 0;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading H2Combustor custom LogicTypes.
    /// Must patch DeviceInputOutput since H2CombustorMachine doesn't override CanLogicRead.
    /// </summary>
    [HarmonyPatch(typeof(DeviceInputOutput), nameof(DeviceInputOutput.CanLogicRead))]
    public static class H2CombustorCanLogicReadPatch
    {
        public static void Postfix(DeviceInputOutput __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to H2CombustorMachine
            if (!(__instance is H2CombustorMachine))
                return;

            ushort value = (ushort)logicType;

            // Check range: H2CombustorProcessedMoles (1230) through H2CombustorCodeError (1233)
            if (value >= (ushort)SLELogicType.H2CombustorProcessedMoles && value <= (ushort)SLELogicType.H2CombustorCodeError)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return H2Combustor custom LogicType values.
    /// Must patch DeviceInputOutput since H2CombustorMachine doesn't override GetLogicValue.
    /// </summary>
    [HarmonyPatch(typeof(DeviceInputOutput), nameof(DeviceInputOutput.GetLogicValue))]
    public static class H2CombustorGetLogicValuePatch
    {
        public static bool Prefix(DeviceInputOutput __instance, LogicType logicType, ref double __result)
        {
            // Only apply to H2CombustorMachine
            if (!(__instance is H2CombustorMachine combustor))
                return true;
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1230-1233)
            if (value < (ushort)SLELogicType.H2CombustorProcessedMoles || value > (ushort)SLELogicType.H2CombustorCodeError)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.H2CombustorProcessedMoles:
                    // Moles processed this tick (from DeviceInputOutputCircuit)
                    __result = combustor.ProcessedMoles.ToFloat();
                    break;

                case SLELogicType.H2CombustorUsedPower:
                    // Power consumed this tick
                    __result = combustor.UsedPower;
                    break;

                case SLELogicType.H2CombustorIsOperable:
                    // Whether device is operable (connections valid)
                    __result = H2CombustorPatches.GetIsOperable(combustor) ? 1 : 0;
                    break;

                case SLELogicType.H2CombustorCodeError:
                    // IC chip error state
                    __result = H2CombustorPatches.GetCodeErrorState(combustor);
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
