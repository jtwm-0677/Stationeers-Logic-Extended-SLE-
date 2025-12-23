using Assets.Scripts.Inventory;
using Assets.Scripts.Objects.Chutes;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using SLE.Core;

namespace SLE.Patches.Devices
{
    /// <summary>
    /// Harmony patches for Harvester to add operational and tray data LogicTypes.
    /// Adds HasTray, IsHarvesting, IsPlanting, ArmState, HasImportPlant, ImportPlantHash,
    /// and fertilizer data from the tray below.
    /// Note: Harvester inherits from DeviceImportExport which has its own CanLogicRead/GetLogicValue,
    /// so we patch that class and check for Harvester instance.
    /// </summary>
    public static class HarvesterPatches
    {
    }

    /// <summary>
    /// Patch CanLogicRead on DeviceImportExport to allow reading Harvester LogicTypes.
    /// </summary>
    [HarmonyPatch(typeof(DeviceImportExport), nameof(DeviceImportExport.CanLogicRead))]
    public static class HarvesterCanLogicReadPatch
    {
        public static void Postfix(DeviceImportExport __instance, ref bool __result, LogicType logicType)
        {
            // Only handle Harvester instances
            if (!(__instance is Harvester))
                return;

            ushort value = (ushort)logicType;
            // Check range: HasTray (1180) through FertilizerGrowthSpeed (1189)
            if (value >= (ushort)SLELogicType.HasTray && value <= (ushort)SLELogicType.FertilizerGrowthSpeed)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch GetLogicValue on DeviceImportExport to return Harvester data values.
    /// </summary>
    [HarmonyPatch(typeof(DeviceImportExport), nameof(DeviceImportExport.GetLogicValue))]
    public static class HarvesterGetLogicValuePatch
    {
        public static bool Prefix(DeviceImportExport __instance, LogicType logicType, ref double __result)
        {
            // Only handle Harvester instances
            if (!(__instance is Harvester harvester))
                return true;

            ushort value = (ushort)logicType;

            // Get the HydroponicTray via reflection (it's a private property)
            var trayProp = typeof(Harvester).GetProperty("HydroponicTray",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tray = trayProp?.GetValue(harvester) as IHarvestable;

            // Get _isHarvesting and _isPlanting fields via reflection
            var isHarvestingField = typeof(Harvester).GetField("_isHarvesting",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isPlantingField = typeof(Harvester).GetField("_isPlanting",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            bool isHarvesting = isHarvestingField != null && (bool)isHarvestingField.GetValue(harvester);
            bool isPlanting = isPlantingField != null && (bool)isPlantingField.GetValue(harvester);

            switch ((SLELogicType)value)
            {
                case SLELogicType.HasTray:
                    // 1 if positioned over a hydroponics tray, 0 if not
                    __result = (tray != null && tray.GetThing != null) ? 1 : 0;
                    return false;

                case SLELogicType.IsHarvesting:
                    // 1 if currently performing harvest operation
                    __result = isHarvesting ? 1 : 0;
                    return false;

                case SLELogicType.IsPlanting:
                    // 1 if currently performing plant operation
                    __result = isPlanting ? 1 : 0;
                    return false;

                case SLELogicType.ArmState:
                    // Arm state: 0=Idle, 1=Planting, 2=Harvesting
                    // Based on the Activate property which maps to ArmControl enum
                    __result = harvester.Activate;
                    return false;

                case SLELogicType.HasImportPlant:
                    // 1 if plant/seed is in import slot
                    // ImportingThing is inherited from DeviceImport
                    __result = (harvester.ImportingThing != null) ? 1 : 0;
                    return false;

                case SLELogicType.ImportPlantHash:
                    // PrefabHash of plant in import slot
                    __result = harvester.ImportingThing?.PrefabHash ?? 0;
                    return false;

                case SLELogicType.HasFertilizer:
                    // 1 if fertilizer is in tray's fertilizer slot
                    if (tray != null && tray.FertilizerSlot != null)
                    {
                        var fertilizer = tray.FertilizerSlot.Get<Fertiliser>();
                        __result = (fertilizer != null) ? 1 : 0;
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                case SLELogicType.FertilizerCycles:
                    // Remaining fertilizer cycles
                    if (tray != null && tray.FertilizerSlot != null)
                    {
                        var fertilizer = tray.FertilizerSlot.Get<Fertiliser>();
                        __result = fertilizer?.Cycles ?? 0;
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                case SLELogicType.FertilizerHarvestBoost:
                    // Fertilizer harvest yield multiplier
                    if (tray != null && tray.FertilizerSlot != null)
                    {
                        var fertilizer = tray.FertilizerSlot.Get<Fertiliser>();
                        __result = fertilizer?.HarvestBoost ?? 0;
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                case SLELogicType.FertilizerGrowthSpeed:
                    // Fertilizer growth speed multiplier
                    if (tray != null && tray.FertilizerSlot != null)
                    {
                        var fertilizer = tray.FertilizerSlot.Get<Fertiliser>();
                        __result = fertilizer?.GrowthSpeed ?? 0;
                    }
                    else
                    {
                        __result = 0;
                    }
                    return false;

                default:
                    return true; // Let original method handle
            }
        }
    }
}
