using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.NetworkSettings
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string ipaddress, string subnetmask, string dhcpsubnetmask, string dhcpserver, string dhcpnameserver, string dhcpipaddress, string dhcpdefaultgateway, bool enabledhcp)
        {
            IPAddress = ipaddress;
            SubnetMask = subnetmask;
            DHCPSubnetMask = dhcpsubnetmask;
            DHCPServer = dhcpserver;
            DHCPNameServer = dhcpnameserver;
            DHCPIPAddress = dhcpipaddress;
            DHCPDefaultGateway = dhcpdefaultgateway;
            EnabledDHCP = enabledhcp;
        }

        public string IPAddress { get; }
        public string SubnetMask { get; }
        public string DHCPSubnetMask { get; }
        public string DHCPServer { get; }
        public string DHCPNameServer { get; }
        public string DHCPIPAddress { get; }
        public string DHCPDefaultGateway { get; }
        public bool EnabledDHCP { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"IPAddress: {IPAddress} SubnetMask: {SubnetMask}";
        public string BatchValueData2 => $"DHCPSubnetMask: {DHCPSubnetMask} DHCPServer: {DHCPServer} DHCPNameServer: {DHCPNameServer} DHCPIPAddress: {DHCPIPAddress} DHCPDefaultGateway: {DHCPDefaultGateway}";
        public string BatchValueData3 => $"EnabledDHCP: {EnabledDHCP}";
    }
}

