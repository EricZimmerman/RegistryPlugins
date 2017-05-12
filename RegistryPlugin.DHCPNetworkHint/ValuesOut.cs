using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.DHCPNetworkHint
{
   public class ValuesOut
    {
        public ValuesOut(string hint, string gateway, DateTimeOffset obtained,DateTimeOffset expires,string dhcpAddress, string dhcpServer, string inter, string interSubkey, string dhcpDomain)
        {
            NetworkHint = hint;
            DefaultGateway = gateway;
            LeaseObtained = obtained;
            LeaseExpires = expires;
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

        public DateTimeOffset LeaseObtained { get; }
        public DateTimeOffset LeaseExpires { get; }
        
        public string DefaultGateway { get; }
        

        public string Interface { get; }

        public string InterfaceSubkey { get; }
    }
}
