using System.Reflection;
using Assets.Scripts;
using Assets.Scripts.Localization2;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.UI;
using HarmonyLib;
using SLE.Core;
using UnityEngine;

namespace SLE.Patches
{
    /// <summary>
    /// Patches to add custom LogicTypes to the Stationpedia device info pages.
    /// </summary>
    public static class StationpediaPatches
    {
        private static bool _pagesRegistered = false;

        /// <summary>
        /// Register Stationpedia pages for all custom LogicTypes.
        /// Called once during game initialization.
        /// </summary>
        public static void RegisterCustomLogicTypePages()
        {
            if (_pagesRegistered) return;

            foreach (var info in LogicTypeRegistry.All)
            {
                // Create a page for this LogicType
                // Key format: "LogicType" + Name (e.g., "LogicTypeContactIndex")
                string pageKey = "LogicType" + info.Name;
                string title = info.Name;

                // Format description with access info
                string accessText = info.Access == "read-write" ? "Read/Write" :
                                   info.Access == "read" ? "Read Only" : "Write Only";

                // Choose color and label based on LogicTypeKind
                string kindColor = info.Kind == LogicTypeKind.Added ? "#88ccff" : "#88ff88";
                string kindLabel = info.Kind == LogicTypeKind.Added
                    ? "Added by Stationeers Logic Extended (New Functionality)"
                    : "Revealed by Stationeers Logic Extended (Hidden Data)";

                string description = $"<b>{info.Name}</b>\n\n" +
                                    $"{info.Description}\n\n" +
                                    $"<color=#888888>Access:</color> {accessText}\n" +
                                    $"<color=#888888>Data Type:</color> {info.DataType}\n" +
                                    $"<color=#888888>Value:</color> {info.Value}\n" +
                                    $"<color=#888888>Category:</color> {info.Category}\n\n" +
                                    $"<color={kindColor}>{kindLabel}</color>";

                var page = new StationpediaPage(pageKey, title, description);
                page.Description = description;

                // Register the page
                Stationpedia.Register(page, false);
            }

            _pagesRegistered = true;
            Debug.Log($"[SLE] Registered {LogicTypeRegistry.Count} custom LogicType Stationpedia pages");
        }
    }

    /// <summary>
    /// Patch Stationpedia.PopulateLogicVariables to also register our custom LogicType pages.
    /// </summary>
    [HarmonyPatch(typeof(Stationpedia), "PopulateLogicVariables")]
    public static class StationpediaPopulateLogicVariablesPatch
    {
        public static void Postfix()
        {
            StationpediaPatches.RegisterCustomLogicTypePages();
        }
    }

    /// <summary>
    /// Patch AddLogicTypeInfo to add our custom LogicTypes with clickable links.
    /// </summary>
    [HarmonyPatch]
    public static class StationpediaAddLogicTypeInfoPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Stationpedia), "AddLogicTypeInfo");
        }

        /// <summary>
        /// After the game adds vanilla LogicTypes, add our custom ones with proper links.
        /// Works for ALL devices - uses reflection to call CanLogicRead/CanLogicWrite
        /// since different device types have these methods on different parent classes.
        /// </summary>
        public static void Postfix(object prefab, ref StationpediaPage page)
        {
            if (prefab == null) return;

            // Use reflection to find CanLogicRead/CanLogicWrite methods
            var prefabType = prefab.GetType();
            var canLogicReadMethod = prefabType.GetMethod("CanLogicRead",
                BindingFlags.Public | BindingFlags.Instance,
                null, new[] { typeof(LogicType) }, null);
            var canLogicWriteMethod = prefabType.GetMethod("CanLogicWrite",
                BindingFlags.Public | BindingFlags.Instance,
                null, new[] { typeof(LogicType) }, null);

            // If this object doesn't support logic, skip it
            if (canLogicReadMethod == null && canLogicWriteMethod == null) return;

            // Add our custom LogicTypes - each device's CanLogicRead patch will
            // return true only for its own LogicTypes
            foreach (var info in LogicTypeRegistry.All)
            {
                var logicType = (LogicType)info.Value;

                bool canRead = false;
                bool canWrite = false;

                try
                {
                    canRead = canLogicReadMethod != null &&
                        (bool)canLogicReadMethod.Invoke(prefab, new object[] { logicType });
                    canWrite = canLogicWriteMethod != null &&
                        (bool)canLogicWriteMethod.Invoke(prefab, new object[] { logicType });
                }
                catch
                {
                    // Some devices may throw on invalid LogicType values - skip them
                    continue;
                }

                if (canRead || canWrite)
                {
                    var insert = new StationLogicInsert();

                    // Set access type
                    if (canWrite && !canRead)
                    {
                        insert.LogicAccessTypes = GameStrings.LogicWrite;
                    }
                    else if (!canWrite && canRead)
                    {
                        insert.LogicAccessTypes = GameStrings.LogicRead;
                    }
                    else
                    {
                        insert.LogicAccessTypes = GameStrings.LogicReadWrite;
                    }

                    // Format as clickable link (same format as vanilla)
                    // <link=LogicType{Name}><color=...>{Name}</color></link>
                    // GREEN (#88ff88) for Revealed (exposing hidden game data)
                    // BLUE (#88ccff) for Added (new controllable functionality)
                    string linkColor = info.Kind == LogicTypeKind.Added ? "#88ccff" : "#88ff88";
                    insert.LogicName = $"<link=LogicType{info.Name}><color={linkColor}>{info.Name}</color></link>";

                    page.LogicInsert.Add(insert);
                }
            }
        }
    }
}
