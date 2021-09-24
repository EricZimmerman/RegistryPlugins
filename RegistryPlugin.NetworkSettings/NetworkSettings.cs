using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.NetworkSettings
{
    public class NetworkSettings : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public NetworkSettings()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "c5aba7fe-996a-4e6d-a6ed-8cf05a99b30e";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Services\Tcpip\Parameters\Interfaces"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "NetworkSettings";

        public string ShortDescription
            => "NetworkSettings Information";

        public string LongDescription => ShortDescription;

        public double Version => 0.1;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var rd in ProcessKey(key))
            {
                _values.Add(rd);
            }
        }

        public IBindingList Values => _values;

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();

            foreach (var subKey in key.SubKeys)
            {
                try
                {
                    string ipaddress = subKey.Values.SingleOrDefault(t => t.ValueName == "IPAddress")?.ValueData;
                    string subnetmask = subKey.Values.SingleOrDefault(t => t.ValueName == "SubnetMask")?.ValueData;
                    string dhcpsubnetmask = subKey.Values.SingleOrDefault(t => t.ValueName == "DhcpSubnetMask")?.ValueData;
                    string dhcpserver = subKey.Values.SingleOrDefault(t => t.ValueName == "DhcpServer")?.ValueData;
                    string dhcpnameserver = subKey.Values.SingleOrDefault(t => t.ValueName == "DhcpNameServer")?.ValueData;
                    string dhcpipaddress = subKey.Values.SingleOrDefault(t => t.ValueName == "DhcpIPAddress")?.ValueData;
                    string dhcpdefaultgateway = subKey.Values.SingleOrDefault(t => t.ValueName == "DhcpDefaultGateway")?.ValueData;
                    bool enabledhcp = subKey.Values.SingleOrDefault(t => t.ValueName == "EnableDHCP")?.ValueData == "1";

                    var ff = new ValuesOut(ipaddress, subnetmask, dhcpsubnetmask, dhcpserver, dhcpnameserver, dhcpipaddress, dhcpdefaultgateway, enabledhcp)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing Interfaces key: {ex.Message}");
                }
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}
