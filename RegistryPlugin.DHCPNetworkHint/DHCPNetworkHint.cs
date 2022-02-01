using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.DHCPNetworkHint
{
    public class DHCPNetworkHint : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public DHCPNetworkHint()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "d28e6fb0-2ef9-44fd-8fa9-128454b5aa1e";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Services\Tcpip\Parameters\Interfaces"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "DHCPNetworkHints";

        public string ShortDescription =>
            "Extracts information from TCPIP Interfaces key including domain, network hint, lease times, etc.";

        public string LongDescription
            =>
                "";

        public double Version => 0.5;
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

            try
            {
                foreach (var sKey in key.SubKeys)
                {
                    if (sKey.SubKeys.Count == 0)
                    {
                        continue;
                    }

                    var ff1 = ProcessVals(sKey, sKey.KeyName);

                    if (ff1 != null)
                    {
                        l.Add(ff1);
                    }


                    foreach (var ssKey in sKey.SubKeys)
                    {
                        var ff = ProcessVals(ssKey, sKey.KeyName);

                        if (ff != null)
                        {
                            l.Add(ff);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Interfaces key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }

        private ValuesOut ProcessVals(RegistryKey ssKey, string originKey)
        {
            var dhcpHintVal = ssKey.Values.SingleOrDefault(t => t.ValueName == "DhcpNetworkHint");

            if (dhcpHintVal == null)
            {
                return null;
            }

            var hint = string.Empty;

            var hitValRev = new string(dhcpHintVal.ValueData.ToCharArray().Reverse().ToArray());

            var bytes = new byte[hitValRev.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var currentHex = hitValRev.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(currentHex, 16);
            }

            hitValRev = new string(CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(bytes).ToCharArray().Reverse().ToArray());

            hint = hitValRev;


            var gateway = string.Empty;

            var gatewayVal = ssKey.Values.SingleOrDefault(t => t.ValueName == "DhcpDefaultGateway");

            if (gatewayVal != null)
            {
                gateway = gatewayVal.ValueData;
            }


            var dhcpAddress = string.Empty;

            var dhcpAddressVal = ssKey.Values.SingleOrDefault(t => t.ValueName == "DhcpIPAddress");

            if (dhcpAddressVal != null)
            {
                dhcpAddress = dhcpAddressVal.ValueData;
            }


            var dhcpServer = string.Empty;
            var dhcpServerVal = ssKey.Values.SingleOrDefault(t => t.ValueName == "DhcpServer");

            if (dhcpServerVal != null)
            {
                dhcpServer = dhcpServerVal.ValueData;
            }


            var obt = DateTimeOffset.Now;
            var obtVal = ssKey.Values.SingleOrDefault(t => t.ValueName == "LeaseObtainedTime");

            if (obtVal != null)
            {
                var oob = int.Parse(obtVal.ValueData);

                obt = DateTimeOffset.FromUnixTimeSeconds(oob);
            }


            var exp = DateTimeOffset.Now;
            var expVal = ssKey.Values.SingleOrDefault(t => t.ValueName == "LeaseTerminatesTime");

            if (expVal != null)
            {
                var oep = int.Parse(expVal.ValueData);

                exp = DateTimeOffset.FromUnixTimeSeconds(oep);
            }

            var dhcpDomain = string.Empty;

            var dhcpDomainVal = ssKey.Values.SingleOrDefault(t => t.ValueName == "DhcpDomain");

            if (dhcpDomainVal != null)
            {
                dhcpDomain = dhcpDomainVal.ValueData;
            }


            var vo =  new ValuesOut(hint, gateway, obt, exp, dhcpAddress, dhcpServer, originKey, ssKey.KeyName,
                dhcpDomain);

            vo.BatchValueName = "Multiple";
            vo.BatchKeyPath = ssKey.KeyPath;

            return vo;
        }
    }
}