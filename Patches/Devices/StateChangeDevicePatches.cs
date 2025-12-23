using System.Reflection;
using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for StateChangeDevice (Condensation/Evaporation Chambers) to expose hidden LogicTypes.
    /// Reveals energy transfer, operability, volume, heat exchange ratio, and liquid ratio.
    /// </summary>
    public static class StateChangeDevicePatches
    {
        // Cached reflection for protected/private members
        private static readonly FieldInfo EnergyTransferField =
            typeof(StateChangeDevice).GetField("_energyTransfer", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly PropertyInfo IsOperableProperty =
            typeof(StateChangeDevice).GetProperty("IsOperable", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo HeatExchangeRatioMethod =
            typeof(StateChangeDevice).GetMethod("HeatExchangeRatio", BindingFlags.NonPublic | BindingFlags.Instance);

        public static float GetEnergyTransfer(StateChangeDevice device)
        {
            var value = EnergyTransferField?.GetValue(device);
            if (value is MoleEnergy energy)
                return energy.ToFloat();
            return 0f;
        }

        public static bool GetIsOperable(StateChangeDevice device)
        {
            return (bool?)IsOperableProperty?.GetValue(device) ?? false;
        }

        public static float GetHeatExchangeRatio(StateChangeDevice device)
        {
            return (float?)HeatExchangeRatioMethod?.Invoke(device, null) ?? 0f;
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading StateChangeDevice custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(StateChangeDevice), nameof(StateChangeDevice.CanLogicRead))]
    public static class StateChangeDeviceCanLogicReadPatch
    {
        public static void Postfix(ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;

            // Check range: ChamberEnergyTransfer (1600) through ChamberLiquidRatio (1604)
            if (value >= (ushort)SLELogicType.ChamberEnergyTransfer && value <= (ushort)SLELogicType.ChamberLiquidRatio)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return StateChangeDevice custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(StateChangeDevice), nameof(StateChangeDevice.GetLogicValue))]
    public static class StateChangeDeviceGetLogicValuePatch
    {
        public static bool Prefix(StateChangeDevice __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes (1600-1604)
            if (value < (ushort)SLELogicType.ChamberEnergyTransfer || value > (ushort)SLELogicType.ChamberLiquidRatio)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.ChamberEnergyTransfer:
                    // Heat energy transfer rate in joules/tick
                    __result = StateChangeDevicePatches.GetEnergyTransfer(__instance);
                    break;

                case SLELogicType.ChamberIsOperable:
                    // Device is operable
                    __result = StateChangeDevicePatches.GetIsOperable(__instance) ? 1 : 0;
                    break;

                case SLELogicType.ChamberVolume:
                    // Internal chamber volume
                    __result = __instance.Volume.ToDouble();
                    break;

                case SLELogicType.ChamberHeatExchangeRatio:
                    // Heat exchange efficiency
                    __result = StateChangeDevicePatches.GetHeatExchangeRatio(__instance);
                    break;

                case SLELogicType.ChamberLiquidRatio:
                    // Liquid volume ratio in internal atmosphere
                    __result = __instance.InternalAtmosphere?.LiquidVolumeRatio ?? 0;
                    break;

                default:
                    __result = 0;
                    break;
            }

            return false; // Skip original method
        }
    }
}
