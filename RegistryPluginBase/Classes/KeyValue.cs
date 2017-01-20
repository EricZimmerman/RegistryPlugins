using System.Text;

// namespaces...

namespace RegistryPluginBase.Classes
{
    /// <summary>
    ///     Represents the data structure being passed to a plugin. One KeyValue will be passed to the plugin per value found
    ///     in a key
    /// </summary>
    public class KeyValue
    {
        public KeyValue(string valueData, byte[] valueDataRaw, string valueName, byte[] valueSlackRaw, string valueType)
        {
            ValueName = valueName;
            ValueData = valueData;
            ValueDataRaw = valueDataRaw;
            ValueSlackRaw = valueSlackRaw;
            ValueType = valueType;
        }

        /// <summary>
        ///     The normalized representation of the value's value.
        /// </summary>
        public string ValueData { get; }

        /// <summary>
        ///     The value as stored in the hive as a series of bytes
        /// </summary>
        public byte[] ValueDataRaw { get; }

        public string ValueName { get; }

        /// <summary>
        ///     The value slack as stored in the hive as a series of bytes
        /// </summary>
        public byte[] ValueSlackRaw { get; }

        /// <summary>
        ///     The values type
        /// </summary>
        public string ValueType { get; }

        // public methods...
        public override string ToString()
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }
    }
}