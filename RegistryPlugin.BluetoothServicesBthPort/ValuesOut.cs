using System;

namespace RegistryPlugin.BluetoothServicesBthPort
{
    public class ValuesOut
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
    }
}