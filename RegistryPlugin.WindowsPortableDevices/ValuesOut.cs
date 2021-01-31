using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.WindowsPortableDevices
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string device, string serialnumber, string guid, string friendlyname, DateTimeOffset? timestamp)
        {
            Device = device;
            SerialNumber = serialnumber;
            Guid = guid;
            FriendlyName = friendlyname;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Device { get; }
        public string SerialNumber { get; }
        public string Guid { get; }
        public string FriendlyName { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Device: {Device} SerialNumber: {SerialNumber} Guid: {Guid}";
        public string BatchValueData2 => $"FriendlyName: {FriendlyName}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} ";
    }
}
