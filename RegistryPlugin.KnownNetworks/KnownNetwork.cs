using System;

namespace RegistryPlugin.KnownNetworks
{
    public class KnownNetwork
    {
        public enum NameTypes
        {
            Wireless = 71,
            Wired = 6,
            WWAN = 23,
            Unknown = 0
        }

        public KnownNetwork(string networkName, NameTypes type, DateTimeOffset firstConnect, DateTimeOffset lastConnect,
            bool managed, string dnsSuffix, string gatewayMacAddress, string profileGuid)
        {
            NetworkName = networkName;
            NameType = type;
            FirstConnectLOCAL = firstConnect.DateTime;
            LastConnectedLOCAL = lastConnect.DateTime;
            Managed = managed;
            DNSSuffix = dnsSuffix;
            GatewayMacAddress = gatewayMacAddress;
            ProfileGUID = profileGuid;
        }


        public string NetworkName { get; private set; }
        public NameTypes NameType { get; }

        public DateTime FirstConnectLOCAL { get; }
        public DateTime LastConnectedLOCAL { get; }

        public bool Managed { get; }

        public string DNSSuffix { get; private set; }
        public string GatewayMacAddress { get; private set; }
        public string ProfileGUID { get; }

        public void UpdateInfo(string macAddress, string dnsSuffix, string networkName)
        {
            GatewayMacAddress = macAddress;
            DNSSuffix = dnsSuffix;
            NetworkName = networkName;
        }
    }
}