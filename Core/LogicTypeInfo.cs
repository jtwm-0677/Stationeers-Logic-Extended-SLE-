namespace SLE.Core
{
    /// <summary>
    /// Indicates whether a LogicType exposes existing hidden data or adds new functionality.
    /// </summary>
    public enum LogicTypeKind
    {
        /// <summary>Exposes existing game data that was hidden from IC10. Shown in GREEN.</summary>
        Revealed,

        /// <summary>Adds new controllable functionality that doesn't exist in vanilla. Shown in BLUE.</summary>
        Added
    }

    /// <summary>
    /// Metadata for a custom LogicType.
    /// Used for documentation and compiler integration.
    /// </summary>
    public class LogicTypeInfo
    {
        /// <summary>Internal name (e.g., "ContactIndex").</summary>
        public string Name { get; set; }

        /// <summary>Display name (e.g., "Contact Index").</summary>
        public string DisplayName { get; set; }

        /// <summary>Human-readable description.</summary>
        public string Description { get; set; }

        /// <summary>Numeric value of the LogicType.</summary>
        public ushort Value { get; set; }

        /// <summary>CRC32 hash for Stationeers compatibility.</summary>
        public int Hash { get; set; }

        /// <summary>Data type: "int", "float", "bool", "long".</summary>
        public string DataType { get; set; }

        /// <summary>Access mode: "read", "write", "read-write".</summary>
        public string Access { get; set; }

        /// <summary>Category for grouping.</summary>
        public string Category { get; set; }

        /// <summary>Devices that support this LogicType.</summary>
        public string[] Devices { get; set; }

        /// <summary>Whether this is revealed (existing hidden data) or added (new functionality).</summary>
        public LogicTypeKind Kind { get; set; }

        public LogicTypeInfo(
            string name,
            string displayName,
            string description,
            ushort value,
            string dataType = "float",
            string access = "read",
            string category = "Custom",
            string[] devices = null,
            LogicTypeKind kind = LogicTypeKind.Revealed)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            Value = value;
            Hash = CalculateHash(name);
            DataType = dataType;
            Access = access;
            Category = category;
            Devices = devices ?? new[] { "SatelliteDish" };
            Kind = kind;
        }

        /// <summary>
        /// Calculate CRC32 hash matching Stationeers' algorithm.
        /// </summary>
        public static int CalculateHash(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            // CRC32 implementation matching Stationeers
            uint crc = 0xFFFFFFFF;
            foreach (char c in value)
            {
                crc ^= (uint)c;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
            }
            return (int)(crc ^ 0xFFFFFFFF);
        }
    }
}
