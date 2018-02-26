using System;

namespace RegistryPlugin.BluetoothServicesBthPort
{
    public class ValuesOut
    {
        public ValuesOut(string btname, string btAddr, string btNameRaw, DateTimeOffset? lastSeenKey)
        {

            BtName = btname;
            BtAddr = btAddr;
            BtNameRaw = btNameRaw;
            LastSeen = lastSeenKey?.UtcDateTime;
        }


        public string BtName { get; }
        public string BtAddr { get; }
        public string BtNameRaw { get; }
        public DateTime? LastSeen { get; }
    }
}