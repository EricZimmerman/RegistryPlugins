using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FeatureUsage
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string keypath, string valuename, string count)
        {
            KeyPath = keypath;
            ValueName = valuename;
            Count = count;
        }

        public string KeyPath { get; }
        public string ValueName { get; }
        public string Count { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"KeyPath: {KeyPath}";
        public string BatchValueData2 => $"ValueName: {ValueName}";
        public string BatchValueData3 => $"Count: {Count}";
    }
}
