using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryDriverBinary
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string driverlastwritetime, string drivercompany, string drivername, string driverversion, string product, string productversion, string path, string sha1, DateTimeOffset? timestamp)
        {
            DriverLastWriteTime = driverlastwritetime;
            DriverCompany = drivercompany;
            DriverName = drivername;
            DriverVersion = driverversion;
            Product = product;
            ProductVersion = productversion;
            Path = path;
            SHA1 = sha1;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string DriverLastWriteTime { get; }
        public string DriverCompany { get; }
        public string Product { get; }
        public string ProductVersion { get; }
        public string DriverName { get; }
        public string DriverVersion { get; }
        public string Path { get; }
        public string SHA1 { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"DriverCompany: {DriverCompany} Product: {Product} ProductVersion: {ProductVersion}";
        public string BatchValueData2 => $"DriverName: {DriverName} DriverVersion: {DriverVersion} SHA1: {SHA1}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}, DriverLastWriteTime: {DriverLastWriteTime}";
    }
}
