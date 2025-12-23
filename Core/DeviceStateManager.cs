using System.Collections.Generic;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Pipes;

namespace SLE.Core
{
    /// <summary>
    /// Manages custom state for devices.
    /// Stores per-device values for custom LogicTypes like ContactIndex, FilterMode, etc.
    /// </summary>
    public static class DeviceStateManager
    {
        private static readonly Dictionary<long, SatelliteDishState> _satelliteDishStates
            = new Dictionary<long, SatelliteDishState>();

        private static readonly Dictionary<long, FiltrationMachineState> _filtrationStates
            = new Dictionary<long, FiltrationMachineState>();

        // TODO: Add AirConditioner state once we determine the correct class name
        // private static readonly Dictionary<long, AirConditionerState> _airConditionerStates
        //     = new Dictionary<long, AirConditionerState>();

        private static readonly object _lock = new object();

        public static void Initialize()
        {
            lock (_lock)
            {
                _satelliteDishStates.Clear();
                _filtrationStates.Clear();
            }
            UnityEngine.Debug.Log("[SLE] DeviceStateManager initialized");
        }

        public static void Cleanup()
        {
            lock (_lock)
            {
                _satelliteDishStates.Clear();
                _filtrationStates.Clear();
            }
        }

        /// <summary>
        /// Get or create state for a SatelliteDish.
        /// </summary>
        public static SatelliteDishState GetOrCreate(SatelliteDish dish)
        {
            if (dish == null) return null;

            lock (_lock)
            {
                long id = dish.ReferenceId;
                if (!_satelliteDishStates.TryGetValue(id, out var state))
                {
                    state = new SatelliteDishState();
                    _satelliteDishStates[id] = state;
                }
                return state;
            }
        }

        /// <summary>
        /// Get or create state for a FiltrationMachineBase.
        /// </summary>
        public static FiltrationMachineState GetOrCreate(FiltrationMachineBase machine)
        {
            if (machine == null) return null;

            lock (_lock)
            {
                long id = machine.ReferenceId;
                if (!_filtrationStates.TryGetValue(id, out var state))
                {
                    state = new FiltrationMachineState();
                    _filtrationStates[id] = state;
                }
                return state;
            }
        }

        // TODO: Add AirConditioner GetOrCreate once we determine the correct class name
        // public static AirConditionerState GetOrCreate(AirConditioner ac) { ... }

        /// <summary>
        /// Remove state for a destroyed device.
        /// </summary>
        public static void Remove(long referenceId)
        {
            lock (_lock)
            {
                _satelliteDishStates.Remove(referenceId);
                _filtrationStates.Remove(referenceId);
            }
        }
    }

    /// <summary>
    /// Custom state for a SatelliteDish instance.
    /// </summary>
    public class SatelliteDishState
    {
        /// <summary>Currently selected contact index (0-based).</summary>
        public int ContactIndex { get; set; } = 0;

        /// <summary>Current filter mode.</summary>
        public SLEFilterMode FilterMode { get; set; } = SLEFilterMode.All;

        /// <summary>Filter parameter value.</summary>
        public int FilterValue { get; set; } = 0;
    }

    /// <summary>
    /// Custom state for a FiltrationMachineBase instance.
    /// </summary>
    public class FiltrationMachineState
    {
        /// <summary>Currently selected filter slot index (0-based).</summary>
        public int FilterSlotIndex { get; set; } = 0;

        /// <summary>Maximum output pressure limit in kPa. 0 = unlimited (default).</summary>
        public float OutputPressureLimit { get; set; } = 0f;

        /// <summary>Minimum input pressure required in kPa. 0 = no minimum (default).</summary>
        public float InputPressureLimit { get; set; } = 0f;
    }

    // TODO: Add AirConditionerState once we implement HVAC patches
    // public class AirConditionerState { ... }
}
