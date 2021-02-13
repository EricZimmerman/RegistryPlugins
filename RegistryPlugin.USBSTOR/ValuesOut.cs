using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.USBSTOR
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string manufacturer, string title, string version, string serialNumber, DateTimeOffset? timestamp,
            string deviceName, DateTimeOffset? installed, DateTimeOffset? firstinstalled, DateTimeOffset? lastconnected, DateTimeOffset? lastremoved)
        {
            Manufacturer = manufacturer;
            Title = title;
            Version = version;
            SerialNumber = serialNumber;
            Timestamp = timestamp;
            DeviceName = deviceName;
            Installed = installed;
            FirstInstalled = firstinstalled;
            LastConnected = lastconnected;
            LastRemoved = lastremoved;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Manufacturer { get; }
        public string Title { get; }
        public string Version { get; }
        public string SerialNumber { get; }
        public string DeviceName { get; }
        public DateTimeOffset? Installed { get; }
        public DateTimeOffset? FirstInstalled { get; }
        public DateTimeOffset? LastConnected { get; }
        public DateTimeOffset? LastRemoved { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Manufacturer: {Manufacturer} Title: {Title} Version: {Version} SerialNumber: {SerialNumber}";
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => 
            $"Installed: {Installed?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}" +
            $"FirstInstalled: {FirstInstalled?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}" +
            $"LastConnected: {LastConnected?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}" +
            $"LastRemoved: {LastRemoved?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
