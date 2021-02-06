using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.DeviceClasses
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string guidfolder, string type, string name, string serialNumber, DateTimeOffset? timestamp)
        {
            GuidFolder = guidfolder;
            Type = type;
            Name = name;
            SerialNumber = serialNumber;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string GuidFolder { get; }
        public string Type { get; }
        public string Name { get; }
        public string SerialNumber { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Type: {Type} Name: {Name} SerialNumber: {SerialNumber}";
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} ";
        public string BatchValueData3 => string.Empty;
    }
}
