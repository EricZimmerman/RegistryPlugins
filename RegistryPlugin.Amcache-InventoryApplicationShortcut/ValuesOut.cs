using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryApplicationShortcut
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string path, DateTimeOffset? timestamp)
        {
            Path = path;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Path { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Path: {Path}";
        public string BatchValueData2 => $"";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
