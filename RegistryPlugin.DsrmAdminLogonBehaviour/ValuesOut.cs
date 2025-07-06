using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.DsrmAdminLogonBehaviour
{

    public class ValuesOut : IValueOut
    {
        public ValuesOut(string valueName, string valueData, DateTimeOffset? lastWriteOffset)
        {
            ValueName = valueName;
            ValueData = valueData;
            LastWriteTime = lastWriteOffset?.UtcDateTime;
            
        }

        public string ValueName { get; }

        public string ValueData { get; }
 
        public DateTime? LastWriteTime { get; }

        public bool PersistenceDetection { get; set; }

        public string BatchKeyPath { get; set; }

        public string BatchValueName { get; set; }

        public string BatchValueData1 => $"Value: {ValueData}";

        public string BatchValueData2 => $"Last write: {LastWriteTime?.ToUniversalTime():yyyy-MM-dd HH:mm:ss}";

        public string BatchValueData3 => PersistenceDetection
            ? "Potential Persistence: DsrmAdminLogonBehaviour value found (1 or 2)"
            : "DsrmAdminLogonBehaviour value found";
    }
}
