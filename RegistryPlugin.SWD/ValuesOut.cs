using RegistryPluginBase.Interfaces;
using System;

namespace RegistryPlugin.SWD
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string type, string keyName, string service, string deviceDesc, string friendlyName, DateTimeOffset? installed, DateTimeOffset? firstinstalled, DateTimeOffset? lastconnected, DateTimeOffset? lastremoved)
        {
            Type = type;
            KeyName = keyName;
            Service = service;
            DeviceDesc = deviceDesc;
            FriendlyName = friendlyName;
            Installed = installed;
            FirstInstalled = firstinstalled;
            LastConnected = lastconnected;
            LastRemoved = lastremoved;
        }
        
        public string Type { get; }
        public string KeyName { get; }
        public string Service { get; }
        public string DeviceDesc { get; }
        public string FriendlyName { get; }
        public DateTimeOffset? Installed { get; }
        public DateTimeOffset? FirstInstalled { get; }
        public DateTimeOffset? LastConnected { get; }
        public DateTimeOffset? LastRemoved { get; }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Type: {Type} KeyName: {KeyName} Service: {Service}";
        public string BatchValueData2 => $"DeviceDesc: {DeviceDesc} FriendlyName: {FriendlyName}";
        public string BatchValueData3 => $"Installed: {Installed?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} FirstInstalled: {FirstInstalled?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} LastConnected: {LastConnected?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} LastRemoved: {LastRemoved?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
