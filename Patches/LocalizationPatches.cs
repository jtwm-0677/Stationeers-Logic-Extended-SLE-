using System;
using Assets.Scripts;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches
{
    /// <summary>
    /// Patches to provide names for custom LogicTypes.
    /// Patches EnumCollection methods used by Logic Reader UI and Stationpedia.
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
    /// This is used by some game code to display LogicType names.
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

    /// <summary>
    /// Patch EnumCollection&lt;LogicType, ushort&gt;.GetName to return custom LogicType names.
    /// This is the primary method used by Logic Reader hover UI.
    /// </summary>
    [HarmonyPatch(typeof(EnumCollection<LogicType, ushort>), nameof(EnumCollection<LogicType, ushort>.GetName))]
    public static class EnumCollectionGetNamePatch
    {
        public static void Postfix(LogicType value, ref string __result)
        {
            // Only handle if result is empty (custom types not in enum definition)
            if (!string.IsNullOrEmpty(__result))
                return;

            // Check if this is one of our custom LogicTypes
            ushort intValue = (ushort)value;
            if (LogicTypeRegistry.TryGet(intValue, out var info))
            {
                __result = info.Name;
            }
        }
    }

    /// <summary>
    /// Patch EnumCollection&lt;LogicType, ushort&gt;.GetNameFromValue to return custom LogicType names.
    /// Used when looking up by int value directly.
    /// </summary>
    [HarmonyPatch(typeof(EnumCollection<LogicType, ushort>), nameof(EnumCollection<LogicType, ushort>.GetNameFromValue))]
    public static class EnumCollectionGetNameFromValuePatch
    {
        public static void Postfix(int value, ref string __result)
        {
            // Only handle if result is empty (custom types not in enum definition)
            if (!string.IsNullOrEmpty(__result))
                return;

            // Check if this is one of our custom LogicTypes
            if (LogicTypeRegistry.TryGet((ushort)value, out var info))
            {
                __result = info.Name;
            }
        }
    }
}
