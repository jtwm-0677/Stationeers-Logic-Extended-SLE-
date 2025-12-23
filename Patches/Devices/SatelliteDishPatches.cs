using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;
using UnityEngine;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for SatelliteDish to add custom LogicTypes.
    /// </summary>
    public static class SatelliteDishPatches
    {
        /// <summary>
        /// Get filtered contacts based on current filter settings.
        /// </summary>
        public static List<ScannedContactData> GetFilteredContacts(SatelliteDish dish, SatelliteDishState state)
        {
            var contacts = dish.DishScannedContacts.ScannedContactData;
            if (contacts == null || contacts.Count == 0)
                return new List<ScannedContactData>();

            var filtered = new List<ScannedContactData>();

            foreach (var data in contacts)
            {
                if (data?.Contact == null) continue;

                bool include = true;
                switch (state.FilterMode)
                {
                    case SLEFilterMode.All:
                        include = true;
                        break;

                    case SLEFilterMode.ShuttleType:
                        include = (int)data.Contact.ShuttleType == state.FilterValue;
                        break;

                    case SLEFilterMode.Resolved:
                        include = data.CurrentTimeTillResolve <= 0f;
                        break;

                    case SLEFilterMode.Unresolved:
                        include = data.CurrentTimeTillResolve > 0f;
                        break;

                    case SLEFilterMode.Contacted:
                        include = data.Contact.Contacted;
                        break;

                    case SLEFilterMode.NotContacted:
                        include = !data.Contact.Contacted;
                        break;
                }

                if (include)
                    filtered.Add(data);
            }

            return filtered;
        }

        /// <summary>
        /// Get the selected contact based on ContactIndex.
        /// </summary>
        public static ScannedContactData GetSelectedContact(SatelliteDish dish, SatelliteDishState state)
        {
            var filtered = GetFilteredContacts(dish, state);
            if (filtered.Count == 0)
                return null;

            int index = Mathf.Clamp(state.ContactIndex, 0, filtered.Count - 1);
            return filtered[index];
        }
    }

    /// <summary>
    /// Patch CanLogicRead to allow reading custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(SatelliteDish), nameof(SatelliteDish.CanLogicRead))]
    public static class CanLogicReadPatch
    {
        public static void Postfix(SatelliteDish __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to SatelliteDish itself, not derived types
            if (__instance.GetType() != typeof(SatelliteDish))
                return;

            ushort value = (ushort)logicType;
            // Only accept SatelliteDish LogicTypes: 1000-1039
            if (value >= (ushort)SLELogicType.ContactIndex && value <= (ushort)SLELogicType.DishInterrogatingId)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch CanLogicWrite to allow writing custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(SatelliteDish), nameof(SatelliteDish.CanLogicWrite))]
    public static class CanLogicWritePatch
    {
        private static readonly HashSet<ushort> WritableTypes = new HashSet<ushort>
        {
            (ushort)SLELogicType.ContactIndex,
            (ushort)SLELogicType.FilterMode,
            (ushort)SLELogicType.FilterValue,
        };

        public static void Postfix(SatelliteDish __instance, ref bool __result, LogicType logicType)
        {
            // Only apply to SatelliteDish itself, not derived types
            if (__instance.GetType() != typeof(SatelliteDish))
                return;

            ushort value = (ushort)logicType;
            if (WritableTypes.Contains(value))
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue to return custom LogicType values.
    /// </summary>
    [HarmonyPatch(typeof(SatelliteDish), nameof(SatelliteDish.GetLogicValue))]
    public static class GetLogicValuePatch
    {
        public static bool Prefix(SatelliteDish __instance, LogicType logicType, ref double __result)
        {
            ushort value = (ushort)logicType;
            if (!LogicTypeRegistry.IsCustomLogicType(value))
                return true; // Let original method handle it

            var state = DeviceStateManager.GetOrCreate(__instance);
            if (state == null)
            {
                __result = 0;
                return false;
            }

            var sleType = (SLELogicType)value;
            __result = GetCustomLogicValue(__instance, state, sleType);
            return false; // Skip original method
        }

        private static double GetCustomLogicValue(SatelliteDish dish, SatelliteDishState state, SLELogicType logicType)
        {
            switch (logicType)
            {
                // Contact Selection & Filtering
                case SLELogicType.ContactIndex:
                    return state.ContactIndex;

                case SLELogicType.ContactCount:
                    return dish.DishScannedContacts.ScannedContactData.Count;

                case SLELogicType.FilterMode:
                    return (int)state.FilterMode;

                case SLELogicType.FilterValue:
                    return state.FilterValue;

                case SLELogicType.FilteredCount:
                    return SatelliteDishPatches.GetFilteredContacts(dish, state).Count;

                // Selected Contact Properties
                case SLELogicType.ContactShuttleType:
                case SLELogicType.ContactLifetime:
                case SLELogicType.ContactDegreeOffset:
                case SLELogicType.ContactResolved:
                case SLELogicType.ContactContacted:
                case SLELogicType.ContactResolutionProgress:
                case SLELogicType.ContactMinWattsResolve:
                case SLELogicType.ContactMinWattsContact:
                case SLELogicType.ContactSecondsToContact:
                case SLELogicType.ContactTraderHash:
                case SLELogicType.ContactReferenceId:
                    return GetContactPropertyValue(dish, state, logicType);

                // Dish State
                case SLELogicType.DishWattageOnContact:
                    var selected = SatelliteDishPatches.GetSelectedContact(dish, state);
                    if (selected?.Contact == null) return -1;
                    return dish.GetWattageOnContact(selected.Contact);

                case SLELogicType.DishIsInterrogating:
                    return dish.InterrogatingContact != null ? 1 : 0;

                case SLELogicType.DishInterrogatingId:
                    return dish.InterrogatingContact?.ReferenceId ?? 0;

                default:
                    return 0;
            }
        }

        private static double GetContactPropertyValue(SatelliteDish dish, SatelliteDishState state, SLELogicType logicType)
        {
            var contactData = SatelliteDishPatches.GetSelectedContact(dish, state);
            if (contactData?.Contact == null)
                return -1; // No contact selected

            var contact = contactData.Contact;

            switch (logicType)
            {
                case SLELogicType.ContactShuttleType:
                    return (int)contact.ShuttleType;

                case SLELogicType.ContactLifetime:
                    return contact.EndLifetime - Time.time;

                case SLELogicType.ContactDegreeOffset:
                    return contactData.LastScannedDegreeOffset;

                case SLELogicType.ContactResolved:
                    return contactData.CurrentTimeTillResolve <= 0f ? 1 : 0;

                case SLELogicType.ContactContacted:
                    return contact.Contacted ? 1 : 0;

                case SLELogicType.ContactResolutionProgress:
                    if (contactData.StartTimeTillResolve <= 0f) return 1;
                    return 1f - (contactData.CurrentTimeTillResolve / contactData.StartTimeTillResolve);

                case SLELogicType.ContactMinWattsResolve:
                    return contact.MinimumWattsToResolve;

                case SLELogicType.ContactMinWattsContact:
                    return contact.MinimumWattsToContact;

                case SLELogicType.ContactSecondsToContact:
                    return contact.SecondsRequiredToContact;

                case SLELogicType.ContactTraderHash:
                    return contact.DataInstance?.TraderData?.IdHash ?? 0;

                case SLELogicType.ContactReferenceId:
                    return contact.ReferenceId;

                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// Patch SetLogicValue to handle writing custom LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(SatelliteDish), nameof(SatelliteDish.SetLogicValue))]
    public static class SetLogicValuePatch
    {
        public static bool Prefix(SatelliteDish __instance, LogicType logicType, double value)
        {
            ushort typeValue = (ushort)logicType;
            if (!LogicTypeRegistry.IsCustomLogicType(typeValue))
                return true; // Let original method handle it

            var state = DeviceStateManager.GetOrCreate(__instance);
            if (state == null)
                return false;

            var sleType = (SLELogicType)typeValue;
            SetCustomLogicValue(__instance, state, sleType, value);
            return false; // Skip original method
        }

        private static void SetCustomLogicValue(SatelliteDish dish, SatelliteDishState state, SLELogicType logicType, double value)
        {
            switch (logicType)
            {
                case SLELogicType.ContactIndex:
                    var filteredCount = SatelliteDishPatches.GetFilteredContacts(dish, state).Count;
                    state.ContactIndex = Mathf.Clamp((int)value, 0, Mathf.Max(0, filteredCount - 1));
                    break;

                case SLELogicType.FilterMode:
                    int mode = (int)value;
                    if (mode >= 0 && mode <= (int)SLEFilterMode.NotContacted)
                    {
                        state.FilterMode = (SLEFilterMode)mode;
                        // Reset index when filter changes
                        state.ContactIndex = 0;
                    }
                    break;

                case SLELogicType.FilterValue:
                    state.FilterValue = (int)value;
                    // Reset index when filter changes
                    state.ContactIndex = 0;
                    break;
            }
        }
    }
}
