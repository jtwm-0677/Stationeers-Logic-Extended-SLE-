using System;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches
{
    /// <summary>
    /// Patches to provide names for custom LogicTypes.
    /// Only affects the Logic Reader/Writer UI display, not LogicMemory numeric values.
    /// </summary>
    public static class LocalizationPatches
    {
        /// <summary>
        /// Try to get the name for a custom LogicType value.
        /// Returns null if not a custom type.
        /// </summary>
        public static string GetCustomLogicTypeName(int value)
        {
            if (LogicTypeRegistry.TryGet((ushort)value, out var info))
            {
                return info.Name;
            }
            return null;
        }
    }

    /// <summary>
    /// Patch Enum.GetName when called with LogicType to return our custom names.
    /// This is used by the game to display LogicType names in Logic Reader/Writer UI.
    /// </summary>
    [HarmonyPatch(typeof(Enum), nameof(Enum.GetName), typeof(Type), typeof(object))]
    public static class EnumGetNamePatch
    {
        public static void Postfix(Type enumType, object value, ref string __result)
        {
            // Only handle LogicType enum
            if (enumType != typeof(LogicType))
                return;

            // Only handle if result is null/empty (custom types not in enum definition)
            if (!string.IsNullOrEmpty(__result))
                return;

            // Check if this is one of our custom LogicTypes
            try
            {
                int intValue;
                if (value is LogicType logicType)
                {
                    intValue = (int)logicType;
                }
                else if (value is int i)
                {
                    intValue = i;
                }
                else if (value is ushort u)
                {
                    intValue = u;
                }
                else
                {
                    return;
                }

                if (LogicTypeRegistry.TryGet((ushort)intValue, out var info))
                {
                    __result = info.Name;
                }
            }
            catch
            {
                // Ignore conversion errors
            }
        }
    }
}
