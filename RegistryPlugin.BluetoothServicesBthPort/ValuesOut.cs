using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.BluetoothServicesBthPort
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string btname, string address, DateTimeOffset? lastSeenKey)
        {
            Name = btname;
            Address = address;
            LastSeen = lastSeenKey?.UtcDateTime;
        }

        public string Name { get; }
        public string Address { get; }
        public DateTime? LastSeen { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Name: {Name}";
        public string BatchValueData2 => $"Last seen: {LastSeen?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff})";
        public string BatchValueData3 => $"Address: {Address}";
    }
}