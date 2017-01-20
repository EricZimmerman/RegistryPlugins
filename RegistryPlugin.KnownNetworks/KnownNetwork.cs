using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            FirstConnect = firstConnect;
            LastConnected = lastConnect;
            Managed = managed;
            DNSSuffix = dnsSuffix;
            GatewayMacAddress = gatewayMacAddress;
            ProfileGUID = profileGuid;
        }

        public void UpdateInfo(string macAddress, string dnsSuffix, string networkName)
        {
            GatewayMacAddress = macAddress;
            DNSSuffix = dnsSuffix;
            NetworkName = networkName;
        }


        public string NetworkName { get; private set; }
        public NameTypes NameType { get; }

        public DateTimeOffset FirstConnect { get; }
        public DateTimeOffset LastConnected { get; }

        public bool Managed { get; }
        
        public string DNSSuffix { get; private set; }
        public string GatewayMacAddress { get; private set; }
        public string ProfileGUID { get; }
        
    }
}
