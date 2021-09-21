using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FirewallRules
{
    public class FirewallRules : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public FirewallRules()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "a2e377a8-9c38-49e3-871a-b6b7057b624a";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "FirewallRules";

        public string ShortDescription
            => "FirewallRules";

        public string LongDescription
            => "https://www.iana.org/assignments/protocol-numbers/protocol-numbers.xhtml";

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

        public Dictionary<string, string> NumToProtocol = new Dictionary<string, string>()
        {
            {"1","ICMP"},
            {"2","IGMP"},
            {"3","GGP"},
            {"4","IPv4"},
            {"5","ST"},
            {"6","TCP"},
            {"17","UDP"},
            {"18","MUX"},
            {"27","RDP"},
            {"41","IPv6"},
            {"47","GRE"},
            {"58","IPv6-ICMP"},
            {"92","MTP"},
            {"121","SMP"},
            {"143","Ethernet"}
        };

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();
            foreach (var i in key.Values)
            {
                try
                {
                    string rules = i.ValueData;
                    string[] ruleList = rules.Split('|');

                    string action = null;
                    string active = null;
                    string dir = null;
                    string protocol = null;
                    string name = null;
                    string desc = null;
                    string app = null;

                    foreach (var j in ruleList)
                    {
                        Regex rgx = new Regex(@"Action=.*");
                        if (rgx.IsMatch(j))
                            action = j.Split('=')[1];

                        rgx = new Regex(@"Active=.*");
                        if (rgx.IsMatch(j))
                            active = j.Split('=')[1];

                        rgx = new Regex(@"Dir=.*");
                        if (rgx.IsMatch(j))
                            dir = j.Split('=')[1];

                        rgx = new Regex(@"Protocol=.*");
                        if (rgx.IsMatch(j))
                        {
                            var protocolNum = j.Split('=')[1];
                            try 
                            {
                                protocol = NumToProtocol[protocolNum];
                            }
                            catch
                            {
                                protocol = protocolNum;
                            }
                        }

                        rgx = new Regex(@"Name=.*");
                        if (rgx.IsMatch(j))
                            name = j.Split('=')[1];

                        rgx = new Regex(@"Desc=.*");
                        if (rgx.IsMatch(j))
                            desc = j.Split('=')[1];

                        rgx = new Regex(@"App=.*");
                        if (rgx.IsMatch(j))
                            app = j.Split('=')[1];
                    }


                    var ff = new ValuesOut(action, active, dir, protocol, name, desc, app)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = key.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing FirewallRules key: {ex.Message}");
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
