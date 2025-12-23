using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Objects.Electrical;
using HarmonyLib;
using SLE.Core;
using UnityEngine;

namespace SLE.Patches
{
    /// <summary>
    /// Patches to register custom LogicType names as IC10 constants.
    /// This allows IC10 code to use friendly names like "ContactIndex" instead of numeric values like 1000.
    /// </summary>
    public static class IC10ConstantsPatches
    {
        private static bool _registered = false;

        /// <summary>
        /// Register all custom LogicType names as IC10 constants.
        /// Called during mod initialization after LogicTypeRegistry is populated.
        /// </summary>
        public static void RegisterCustomConstants()
        {
            if (_registered)
                return;

            try
            {
                // Get the AllConstants field
                var allConstantsField = typeof(ProgrammableChip).GetField("AllConstants",
                    BindingFlags.Public | BindingFlags.Static);

                if (allConstantsField == null)
                {
                    Debug.LogError("[SLE] Could not find ProgrammableChip.AllConstants field");
                    return;
                }

                // Get current constants array
                var currentConstants = (ProgrammableChip.Constant[])allConstantsField.GetValue(null);
                if (currentConstants == null)
                {
                    Debug.LogError("[SLE] ProgrammableChip.AllConstants is null");
                    return;
                }

                // Build list of new constants from our registry
                var newConstants = new List<ProgrammableChip.Constant>();
                foreach (var info in LogicTypeRegistry.All)
                {
                    // Create constant with name, description, and numeric value
                    var constant = new ProgrammableChip.Constant(
                        info.Name,
                        info.Description,
                        info.Value,
                        true  // Add value to description
                    );
                    newConstants.Add(constant);
                }

                if (newConstants.Count == 0)
                {
                    Debug.LogWarning("[SLE] No custom LogicTypes found to register as constants");
                    return;
                }

                // Create extended array with original + new constants
                var extendedConstants = new ProgrammableChip.Constant[currentConstants.Length + newConstants.Count];

                // Copy original constants
                currentConstants.CopyTo(extendedConstants, 0);

                // Add our custom constants
                for (int i = 0; i < newConstants.Count; i++)
                {
                    extendedConstants[currentConstants.Length + i] = newConstants[i];
                }

                // Replace the static field with extended array
                allConstantsField.SetValue(null, extendedConstants);

                _registered = true;
                Debug.Log($"[SLE] Registered {newConstants.Count} custom LogicType names as IC10 constants");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SLE] Failed to register IC10 constants: {ex.Message}");
            }
        }
    }
}
