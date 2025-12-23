using System;
using Assets.Scripts.Atmospherics;
using Assets.Scripts.Networks;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using Networks;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for PipeAnalysizer to add advanced atmospheric data LogicTypes.
    /// Adds partial pressures, gas/liquid separation, combustion data, and network state.
    /// </summary>
    public static class PipeAnalyzerPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading PipeAnalyzer custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(PipeAnalysizer), nameof(PipeAnalysizer.CanLogicRead))]
    public static class PipeAnalyzerCanLogicReadPatch
    {
        public static void Postfix(PipeAnalysizer __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to PipeAnalysizer itself, not derived types
            if (__instance.GetType() != typeof(PipeAnalysizer))
                return;

            ushort value = (ushort)logicType;
            // Check range: PartialPressureO2 (1250) through NetworkWorstDamage (1292)
            if (value >= (ushort)SLELogicType.PartialPressureO2 && value <= (ushort)SLELogicType.NetworkWorstDamage)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return PipeAnalyzer custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(PipeAnalysizer), nameof(PipeAnalysizer.GetLogicValue))]
    public static class PipeAnalyzerGetLogicValuePatch
    {
        public static bool Prefix(PipeAnalysizer __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.PartialPressureO2 || value > (ushort)SLELogicType.NetworkWorstDamage)
                return true;

            // Check if analyzer is operational
            if (!__instance.OnOff || !__instance.Powered || __instance.Error == 1)
            {
                __result = -1;
                return false;
            }

            // Get the network atmosphere
            var atmosphere = __instance.NetworkAtmosphere;
            if (atmosphere == null)
            {
                __result = -1;
                return false;
            }

            // Get the pipe network for network-level data
            var pipeNetwork = __instance.SmallCell?.Pipe?.PipeNetwork;

            switch ((SLELogicType)value)
            {
                // Partial Pressures
                case SLELogicType.PartialPressureO2:
                    __result = atmosphere.PartialPressureO2.ToDouble();
                    return false;

                case SLELogicType.PartialPressureCO2:
                    __result = atmosphere.PartialPressureCarbonDioxide.ToDouble();
                    return false;

                case SLELogicType.PartialPressureN2:
                    __result = atmosphere.PartialPressureNitrogen.ToDouble();
                    return false;

                case SLELogicType.PartialPressureVolatiles:
                    __result = atmosphere.PartialPressureVolatiles.ToDouble();
                    return false;

                case SLELogicType.PartialPressureN2O:
                    __result = atmosphere.PartialPressureNitrousOxide.ToDouble();
                    return false;

                case SLELogicType.PartialPressurePollutant:
                    __result = atmosphere.PartialPressurePollutant.ToDouble();
                    return false;

                case SLELogicType.PartialPressureH2:
                    __result = atmosphere.PartialPressureHydrogen.ToDouble();
                    return false;

                case SLELogicType.PartialPressureSteam:
                    __result = atmosphere.PartialPressureSteam.ToDouble();
                    return false;

                case SLELogicType.PartialPressureToxins:
                    __result = atmosphere.PartialPressureToxins.ToDouble();
                    return false;

                // Gas/Liquid Separation
                case SLELogicType.TotalMolesGasses:
                    __result = atmosphere.TotalMolesGases.ToDouble();
                    return false;

                case SLELogicType.TotalMolesLiquids:
                    __result = atmosphere.TotalMolesLiquids.ToDouble();
                    return false;

                case SLELogicType.LiquidVolumeRatio:
                    __result = atmosphere.LiquidVolumeRatio;
                    return false;

                case SLELogicType.PressureGasses:
                    __result = atmosphere.PressureGasses.ToDouble();
                    return false;

                case SLELogicType.LiquidPressureOffset:
                    __result = atmosphere.LiquidPressureOffset.ToDouble();
                    return false;

                // Combustion & Reactions
                case SLELogicType.PipeCombustionEnergy:
                    __result = atmosphere.CombustionEnergy.ToDouble();
                    return false;

                case SLELogicType.CleanBurnRate:
                    __result = atmosphere.CleanBurnRate;
                    return false;

                case SLELogicType.Inflamed:
                    __result = atmosphere.Inflamed ? 1 : 0;
                    return false;

                case SLELogicType.Suppressed:
                    __result = atmosphere.Suppressed;
                    return false;

                // Network & State
                case SLELogicType.EnergyConvected:
                    __result = pipeNetwork?.EnergyConvected ?? 0;
                    return false;

                case SLELogicType.EnergyRadiated:
                    __result = pipeNetwork?.EnergyRadiated ?? 0;
                    return false;

                case SLELogicType.IsAboveArmstrong:
                    __result = atmosphere.IsAboveArmstrong() ? 1 : 0;
                    return false;

                case SLELogicType.Condensation:
                    __result = atmosphere.Condensation ? 1 : 0;
                    return false;

                case SLELogicType.NetworkContentType:
                    __result = pipeNetwork != null ? (int)pipeNetwork.NetworkContentType : -1;
                    return false;

                // Stress Monitoring
                case SLELogicType.PipeMaxPressure:
                    var pipe = __instance.SmallCell?.Pipe;
                    __result = pipe?.MaxPressure.ToDouble() ?? Chemistry.Limits.MAXPressureGasPipe.ToDouble();
                    return false;

                case SLELogicType.PipeStressRatio:
                    // Return the HIGHER of pressure stress OR liquid stress
                    // Pressure stress: currentPressure / maxPressure (triggers at 0.8)
                    // Liquid stress: LiquidVolumeRatio / 0.02 (game scales so 2% liquid = 100% stress)
                    var stressPipe = __instance.SmallCell?.Pipe;
                    var maxPressure = stressPipe?.MaxPressure.ToDouble() ?? Chemistry.Limits.MAXPressureGasPipe.ToDouble();
                    var currentPressure = atmosphere.PressureGassesAndLiquids.ToDouble();
                    var pressureStress = maxPressure > 0 ? currentPressure / maxPressure : 0;

                    // Liquid stress only applies to gas pipes with liquid in them
                    // Game uses 0.02 (2%) as the 100% stress threshold
                    var liquidStress = 0.0;
                    if (pipeNetwork?.NetworkContentType == Pipe.ContentType.Gas)
                    {
                        // Scale to match game display: 2% liquid = 1.0 (100% stress)
                        liquidStress = atmosphere.LiquidVolumeRatio / 0.02;
                    }

                    __result = Math.Max(pressureStress, liquidStress);
                    return false;

                case SLELogicType.PipeIsStressed:
                    // Use the actual pipe's Stressed property - this correctly detects:
                    // - High pressure stress (>=80% of max)
                    // - Liquid in gas pipe stress
                    // - Network-propagated stress
                    var stressedPipe = __instance.SmallCell?.Pipe;
                    __result = stressedPipe?.Stressed == true ? 1 : 0;
                    return false;

                // Damage Monitoring
                case SLELogicType.PipeDamageRatio:
                    var damagePipe = __instance.SmallCell?.Pipe;
                    __result = damagePipe?.DamageState?.TotalRatioClamped ?? 0;
                    return false;

                case SLELogicType.PipeIsBurst:
                    var burstPipe = __instance.SmallCell?.Pipe;
                    __result = burstPipe?.IsBurst != PipeBurst.None ? 1 : 0;
                    return false;

                case SLELogicType.PipeDamageType:
                    // Returns bitmask: 1=Pressure, 2=Liquid, 4=Frozen
                    var damageTypePipe = __instance.SmallCell?.Pipe;
                    __result = (int)(damageTypePipe?.DamageRecord ?? PipeBurst.None);
                    return false;

                case SLELogicType.NetworkHasFault:
                    __result = pipeNetwork?.HasNetworkFault == true ? 1 : 0;
                    return false;

                case SLELogicType.NetworkWorstDamage:
                    // Get the weakest (most damaged) member in the network
                    var weakest = pipeNetwork?.GetWeakestMember();
                    __result = weakest?.GetAsThing?.DamageState?.TotalRatioClamped ?? 0;
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
