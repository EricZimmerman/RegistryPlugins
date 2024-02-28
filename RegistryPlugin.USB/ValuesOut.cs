using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.USB
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string keyName, string serialNumber, string parentIdPrefix, string service, string deviceDesc, string friendlyName, string deviceName, string locationinformation, DateTimeOffset? installed, DateTimeOffset? firstinstalled, DateTimeOffset? lastconnected)
        {
            KeyName = keyName;
            SerialNumber = serialNumber;
            ParentidPrefix = parentIdPrefix;
            Service = service;
            DeviceDesc = deviceDesc;
            FriendlyName = friendlyName; 
            DeviceName = deviceName;
            LocationInformation = locationinformation;
            Installed = installed;
            FirstInstalled = firstinstalled;
            LastConnected = lastconnected;
        }

        public string KeyName { get; }
        public string SerialNumber { get; }
        public string ParentidPrefix { get; }
        public string Service { get; }
        public string DeviceDesc { get; }
        public string FriendlyName { get; }
        public string DeviceName { get; }
        public string LocationInformation { get; }
        public DateTimeOffset? Installed { get; }
        public DateTimeOffset? FirstInstalled { get; }
        public DateTimeOffset? LastConnected { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"KeyName: {KeyName} SerialNumber: {SerialNumber} ParentIdPrefix: {ParentidPrefix} Service: {Service}";
        public string BatchValueData2 => $"DeviceDesc: {DeviceDesc} FriendlyName: {FriendlyName} DeviceName: {DeviceName} Location Information: {LocationInformation}";
        public string BatchValueData3 => $"Installed: {Installed?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} FirstInstalled: {FirstInstalled?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} LastConnected: {LastConnected?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
