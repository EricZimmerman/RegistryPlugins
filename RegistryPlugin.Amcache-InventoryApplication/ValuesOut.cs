using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryApplication
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string name, string version, string publisher, string source, string rootdirpath, string uninstallstring, DateTimeOffset? timestamp)
        {
            Name = name;
            Version = version;
            Publisher = publisher;
            Source = source;
            RootDirPath = rootdirpath;
            UninstallString = uninstallstring;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Name { get; }
        public string Version { get; }
        public string Publisher { get; }
        public string Source { get; }
        public string RootDirPath { get; }
        public string UninstallString { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Name: {Name} Version: {Version} Publisher: {Publisher}";
        public string BatchValueData2 => $"Source: {Source} RootDirPath: {RootDirPath} UninstallString: {UninstallString}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
