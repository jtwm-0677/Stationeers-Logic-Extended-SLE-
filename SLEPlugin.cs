using HarmonyLib;
using SLE.Core;
using SLE.Patches;
using StationeersMods.Interface;
using UnityEngine;

namespace SLE
{
    public class SLEPlugin : ModBehaviour
    {
        public static SLEPlugin Instance { get; private set; }

        private Harmony _harmony;

        public override void OnLoaded(ContentHandler contentHandler)
        {
            Instance = this;

            Debug.Log($"[SLE] {PluginInfo.NAME} v{PluginInfo.VERSION} loading...");

            // Initialize the LogicType registry
            LogicTypeRegistry.Initialize();

            // Initialize device state manager
            DeviceStateManager.Initialize();

            // Apply Harmony patches
            _harmony = new Harmony(PluginInfo.GUID);
            _harmony.PatchAll();

            // Register custom LogicType names as IC10 constants
            // This allows IC10 to use names like "ContactIndex" instead of 1000
            IC10ConstantsPatches.RegisterCustomConstants();

            Debug.Log($"[SLE] {PluginInfo.NAME} loaded successfully!");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            DeviceStateManager.Cleanup();
            Debug.Log($"[SLE] {PluginInfo.NAME} unloaded");
        }
    }

    public static class PluginInfo
    {
        public const string GUID = "com.stationeers.sle";
        public const string NAME = "Stationeers Logic Extended";
        public const string VERSION = "1.0.0";
    }
}
