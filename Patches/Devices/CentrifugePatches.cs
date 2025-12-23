namespace SLE.Patches.Devices
{
    /// <summary>
    /// Placeholder for regular Centrifuge patches.
    /// Note: The regular Centrifuge has no logic support (no CanLogicRead/GetLogicValue methods).
    /// Only CombustionCentrifuge supports logic - see CombustionCentrifugePatches.cs.
    /// </summary>
    public static class CentrifugePatches
    {
        // Regular Centrifuge cannot be patched for logic support without more invasive changes.
        // The CombustionCentrifuge already has full logic support and is patched separately.
    }
}
