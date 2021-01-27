using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.USBSTOR
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string manufacturer, string title, string version, string serialNumber, DateTimeOffset? timestamp)
        {
            Manufacturer = manufacturer;
            Title = title;
            Version = version;
            SerialNumber = serialNumber;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Manufacturer { get; }
        public string Title { get; }
        public string Version { get; }
        public string SerialNumber { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Manufacture: {Manufacturer} Title: {Title} Version: {Version} SerialNumber: {SerialNumber}";
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} ";
        public string BatchValueData3 => string.Empty;
    }
}