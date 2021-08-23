using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryDeviceContainer
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string modelname, string friendlyname, string modelnumber, string manufacturer, string primarycategory, string modelid, DateTimeOffset? timestamp)
        {
            ModelName = modelname;
            FriendlyName = friendlyname;
            ModelNumber = modelnumber;
            Manufacturer = manufacturer;
            PrimaryCategory = primarycategory;
            ModelID = modelid;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string ModelName { get; }
        public string FriendlyName { get; }
        public string ModelNumber { get; }
        public string Manufacturer { get; }
        public string PrimaryCategory { get; }
        public string ModelID { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"ModelName: {ModelName} FriendlyName: {FriendlyName} ModelNumber: {ModelNumber}";
        public string BatchValueData2 => $"Manufacturer: {Manufacturer} PrimaryCategory: {PrimaryCategory} ModelID: {ModelID}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
