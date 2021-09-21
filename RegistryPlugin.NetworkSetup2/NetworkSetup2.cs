using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.NetworkSetup2
{
    public class NetworkSetup2 : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public NetworkSetup2()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "4f89e1ca-cccc-4f39-9324-bc04e37875ff";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Control\NetworkSetup2"
        });
        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "NetworkSetup2";

        public string ShortDescription
            => "https://thinkdfir.com/2019/10/05/hunting-for-mac-addresses/";

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
            var interfaces = key.SubKeys.SingleOrDefault(t => t.KeyName == "Interfaces");
            if (interfaces != null) {
                foreach (var i in interfaces.SubKeys)
                {
                    try
                    {
                        var kernel = i.SubKeys.SingleOrDefault(t => t.KeyName == "Kernel");
                        if (kernel != null)
                        {
                            var protocollist = kernel.Values.SingleOrDefault(t => t.ValueName == "ProtocolList")?.ValueData;
                            var ifalias = kernel.Values.SingleOrDefault(t => t.ValueName == "IfAlias")?.ValueData;
                            var ifdescr = kernel.Values.SingleOrDefault(t => t.ValueName == "IfDescr")?.ValueData;
                            var iftype = kernel.Values.SingleOrDefault(t => t.ValueName == "IfType")?.ValueData;
                            var permanentaddress = kernel.Values.SingleOrDefault(t => t.ValueName == "PermanentAddress")?.ValueData;
                            var currentaddress = kernel.Values.SingleOrDefault(t => t.ValueName == "CurrentAddress")?.ValueData;

                            var ff = new ValuesOut(protocollist, ifalias, ifdescr, iftype, permanentaddress, currentaddress)
                            {
                                BatchValueName = "Multiple",
                                BatchKeyPath = i.KeyPath
                            };
                            l.Add(ff);
                        }
                    }
                    catch (Exception ex)
                    {
                        Errors.Add($"Error processing NetworkSetup2 key: {ex.Message}");
                    }
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
