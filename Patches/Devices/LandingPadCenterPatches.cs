using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using Objects.Electrical;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for LandingPadCenter to add custom LogicTypes.
    /// Fixes vanilla oversight where CanLogicRead returns false for all types
    /// despite GetLogicValue supporting Mode, Activate, and Vertical.
    /// Also adds IsTraderReady, HasContact, and Locked status.
    /// </summary>
    public static class LandingPadCenterPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading LandingPadCenter custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(LandingPadCenter), nameof(LandingPadCenter.CanLogicRead), new[] { typeof(LogicType) })]
    public static class LandingPadCenterCanLogicReadPatch
    {
        public static void Postfix(LandingPadCenter __instance, ref bool __result, LogicType logicType)
        {
            ushort value = (ushort)logicType;
            // Check range: PadContactStatus (1820) through PadWaypointHeight (1824)
            if (value >= (ushort)SLELogicType.PadContactStatus && value <= (ushort)SLELogicType.PadWaypointHeight)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return LandingPadCenter custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(LandingPadCenter), nameof(LandingPadCenter.GetLogicValue), new[] { typeof(LogicType) })]
    public static class LandingPadCenterGetLogicValuePatch
    {
        public static bool Prefix(LandingPadCenter __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;

            // Only handle our custom LogicTypes
            if (value < (ushort)SLELogicType.PadContactStatus || value > (ushort)SLELogicType.PadWaypointHeight)
                return true;

            switch ((SLELogicType)value)
            {
                case SLELogicType.PadContactStatus:
                    // Mode is ContactStatus enum: 0=NoContact, 1=Approaching, 2=WaitingApproach, 3=WaitingDoors, 4=Landed
                    __result = __instance.Mode;
                    return false;

                case SLELogicType.PadIsTraderReady:
                    __result = __instance.IsTraderReady ? 1 : 0;
                    return false;

                case SLELogicType.PadHasContact:
                    __result = __instance.CurrentTradingContact != null ? 1 : 0;
                    return false;

                case SLELogicType.PadIsLocked:
                    __result = __instance.Locked ? 1 : 0;
                    return false;

                case SLELogicType.PadWaypointHeight:
                    __result = __instance.VirtualWaypointHeight;
                    return false;

                default:
                    return true;
            }
        }
    }
}
