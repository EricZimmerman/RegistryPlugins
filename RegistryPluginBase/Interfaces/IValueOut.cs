namespace RegistryPluginBase.Interfaces
{
    public interface IValueOut
    {
        /// <summary>
        /// Holds the key name where this data came from
        /// </summary>
        string BatchKeyPath { get; set; }
        /// <summary>
        /// Holds the value name where this data came from
        /// </summary>
        string BatchValueName { get; set; }
        /// <summary>
        ///     In batch mode, this property must return a string representing parsed data from a key/value. This essentially normalizes binary or other values so they can be used in standardized reporting
        /// <remarks>This property should be the most important data returned and should always be present. In CSV output, it becomes 'ValueData'</remarks>
        /// </summary>
        string BatchValueData1 { get; }

        /// <summary>
        ///     In batch mode, this property must return a string representing parsed data from a key/value. This essentially normalizes binary or other values so they can be used in standardized reporting
        /// <remarks>This property should be secondary data to return, like a timestamp or something else that is useful for a given plugin to report.  In CSV output, it becomes 'ValueData2'</remarks>
        /// </summary>
        string BatchValueData2 { get; }

        /// <summary>
        ///     In batch mode, this property must return a string representing parsed data from a key/value. This essentially normalizes binary or other values so they can be used in standardized reporting
        /// <remarks>This property should be any remaining data to return.  In CSV output, it becomes 'ValueData3'</remarks>
        /// </summary>
        string BatchValueData3 { get; }
    }
}