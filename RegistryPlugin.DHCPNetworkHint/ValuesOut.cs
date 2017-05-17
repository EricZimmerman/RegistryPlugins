using System;

namespace RegistryPlugin.DHCPNetworkHint
{
    public class ValuesOut
    {
        public ValuesOut(string hint, string gateway, DateTimeOffset obtained, DateTimeOffset expires,
            string dhcpAddress, string dhcpServer, string inter, string interSubkey, string dhcpDomain)
        {
            NetworkHint = hint;
            DefaultGateway = gateway;
            LeaseObtained = obtained.UtcDateTime;
            LeaseExpires = expires.UtcDateTime;
            DHCPAddress = dhcpAddress;
            DHCPServer = dhcpServer;
            Interface = inter;
            InterfaceSubkey = interSubkey;
            DHCPDomain = dhcpDomain;
        }

        public string NetworkHint { get; }
        public string DHCPAddress { get; }
        public string DHCPServer { get; }

        public string DHCPDomain { get; }

        public DateTime LeaseObtained { get; }
        public DateTime LeaseExpires { get; }

        public string DefaultGateway { get; }


        public string Interface { get; }

        public string InterfaceSubkey { get; }
    }
}