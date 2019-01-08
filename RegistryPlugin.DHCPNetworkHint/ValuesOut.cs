using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.DHCPNetworkHint
{
    public class ValuesOut:IValueOut
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
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 { get; }
        public string BatchValueData2 { get; }
        public string BatchValueData3 { get; }
    }
}