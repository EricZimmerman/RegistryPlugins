using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryApplicationFile
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string path, string name, string productname, string publisher, string version, string sha1, DateTimeOffset? timestamp)
        {
            Path = path;
            Name = name;
            ProductName = productname;
            Publisher = publisher;
            Version = version;
            SHA1 = sha1;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Path { get; }
        public string Name { get; }
        public string ProductName { get; }
        public string Publisher { get; }
        public string Version { get; }
        public string SHA1 { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Path: {Path} Name: {Name} ProductName: {ProductName}";
        public string BatchValueData2 => $"Publisher: {Publisher} Version: {Version} SHA1: {SHA1}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
