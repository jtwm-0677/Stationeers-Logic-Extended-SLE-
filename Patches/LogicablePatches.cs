using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;
using UnityEngine;

namespace SLE.Patches
{
    /// <summary>
    /// Patches to add custom LogicTypes to the game's iteration arrays,
    /// making them appear in Logic Reader/Writer dropdowns.
    /// </summary>
    public static class LogicablePatches
    {
        private static bool _initialized = false;

        /// <summary>
        /// Inject custom LogicTypes into the Logicable.LogicTypes array.
        /// Called after game initialization.
        /// </summary>
        public static void InjectCustomLogicTypes()
        {
            if (_initialized) return;

            try
            {
                // Get the original array
                var originalTypes = Logicable.LogicTypes;
                var originalNames = GetLogicTypeNames();

                // Create expanded arrays
                var customTypes = LogicTypeRegistry.All.ToList();
                var newLength = originalTypes.Length + customTypes.Count;

                var expandedTypes = new LogicType[newLength];
                var expandedNames = new string[newLength];

                // Copy original values
                Array.Copy(originalTypes, expandedTypes, originalTypes.Length);
                if (originalNames != null)
                {
                    Array.Copy(originalNames, expandedNames, originalNames.Length);
                }

                // Add custom values
                for (int i = 0; i < customTypes.Count; i++)
                {
                    int index = originalTypes.Length + i;
                    // Cast our custom ushort value to LogicType enum
                    expandedTypes[index] = (LogicType)customTypes[i].Value;
                    expandedNames[index] = customTypes[i].Name;
                }

                // Replace the static arrays using reflection
                var logicTypesField = AccessTools.Field(typeof(Logicable), "LogicTypes");
                var logicTypeNamesField = AccessTools.Field(typeof(Logicable), "LogicTypeNames");

                logicTypesField.SetValue(null, expandedTypes);
                if (logicTypeNamesField != null)
                {
                    logicTypeNamesField.SetValue(null, expandedNames);
                }

                // Re-run Initialize to rebuild the redirect arrays
                RebuildRedirectArrays(expandedNames);

                _initialized = true;
                Debug.Log($"[SLE] Injected {customTypes.Count} custom LogicTypes into Logicable.LogicTypes");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SLE] Failed to inject custom LogicTypes: {ex}");
            }
        }

        private static string[] GetLogicTypeNames()
        {
            var field = AccessTools.Field(typeof(Logicable), "LogicTypeNames");
            return field?.GetValue(null) as string[];
        }

        private static void RebuildRedirectArrays(string[] names)
        {
            try
            {
                // Rebuild LogicTypeNamesRedirects (sorted index array)
                var redirectsField = AccessTools.Field(typeof(Logicable), "LogicTypeNamesRedirects");
                if (redirectsField != null && names != null)
                {
                    var redirects = new int[names.Length];
                    for (int i = 0; i < names.Length; i++)
                    {
                        redirects[i] = i;
                    }
                    Array.Sort(redirects, (a, b) => string.Compare(names[a], names[b], StringComparison.Ordinal));
                    redirectsField.SetValue(null, redirects);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SLE] Failed to rebuild redirect arrays: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Patch Logicable.Initialize to inject custom types after game initialization.
    /// </summary>
    [HarmonyPatch(typeof(Logicable), nameof(Logicable.Initialize))]
    public static class LogicableInitializePatch
    {
        public static void Postfix()
        {
            LogicablePatches.InjectCustomLogicTypes();
        }
    }
}
