using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryDevicePnp
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string model, string manufacturer, string description, string installdate, string parentid, string matchingid, DateTimeOffset? timestamp)
        {
            Model = model;
            Manufacturer = manufacturer;
            Description = description;
            InstallDate = installdate;
            ParentID = parentid;
            MatchingID = matchingid;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Model { get; }
        public string Manufacturer { get; }
        public string Description { get; }
        public string InstallDate { get; }
        public string ParentID { get; }
        public string MatchingID { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Model: {Model} Manufacturer: {Manufacturer} Description: {Description}";
        public string BatchValueData2 => $"InstallDate: {InstallDate} ParentID: {ParentID} MatchingID: {MatchingID}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
