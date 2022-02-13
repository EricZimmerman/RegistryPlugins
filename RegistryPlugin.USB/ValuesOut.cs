using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.USB
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string keyName, string serialNumber, string parentIdPrefix, string service, string deviceName, string friendlyName, string locationinformation, DateTimeOffset? timestamp)
        {
            KeyName = keyName;
            SerialNumber = serialNumber;
            ParentidPrefix = parentIdPrefix;
            Service = service;
            DeviceName = deviceName;
            FriendlyName = friendlyName;
            LocationInformation = locationinformation;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string KeyName { get; }
        public string SerialNumber { get; }
        public string ParentidPrefix { get; }
        public string Service { get; }
        public string DeviceName { get; }
        public string FriendlyName { get; }
        public string LocationInformation { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"KeyName: {KeyName} SerialNumber: {SerialNumber} ParentIdPrefix: {ParentidPrefix} Service: {Service}";
        public string BatchValueData2 => $"DeviceName: {DeviceName} FriendlyName: {FriendlyName} Location Information: {LocationInformation}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
