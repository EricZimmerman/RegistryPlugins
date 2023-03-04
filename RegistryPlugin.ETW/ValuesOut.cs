using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.ETW
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string lastwritetimestamp, string guid, string provider, string enabled, string enabledproperty)
        {
            LastWriteTimestamp = lastwritetimestamp;
            Guid = guid;
            Provider = provider;
            Enabled = enabled;
            EnabledProperty = enabledproperty;
        }

        public string LastWriteTimestamp { get; }
        public string Guid { get; }
        public string Provider { get; }
        public string Enabled { get; }
        public string EnabledProperty { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Provider: {Provider}";
        public string BatchValueData2 => $"Enabled: {Enabled}";
        public string BatchValueData3 => $"EnabledProperty: {EnabledProperty}";
    }
}
