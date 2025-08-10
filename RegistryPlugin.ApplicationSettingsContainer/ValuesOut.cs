using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.ApplicationSettingsContainer
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string valueName, string keyPath, string dataType, string value, DateTime timeStamp, string notes = "")
        {
            BatchValueName = valueName;
            BatchKeyPath = keyPath;
            ValueName = valueName;
            ValueType = dataType;
            Value = value;
            Notes = notes;
            UTCTimestamp = timeStamp;
        }

        public string ValueName { get; set; }
        public string ValueType { get; }
        public string Value { get; set; }
        public string Notes { get; set; }
        public DateTime UTCTimestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"ValueType: {ValueType}";
        public string BatchValueData2 => "";
        public string BatchValueData3 => "";
    }
}