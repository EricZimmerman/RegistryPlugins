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
            @"ControlSet00*\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules",
            @"ControlSet00*\Services\SharedAccess\Parameters\FirewallPolicy\RestrictedServices\AppIso\FirewallRules",
            @"ControlSet00*\Services\SharedAccess\Parameters\FirewallPolicy\Mdm\FirewallRules"
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

        public void SetData(string regexValue, string compareString, List<String> data)
        {
            if (new Regex(regexValue).IsMatch(compareString))
            {
                var splitData = compareString.Split('=');
                if (splitData[0] == "Protocol")
                {
                    try
                    {
                        data.Add(NumToProtocol[splitData[1]]);
                    }
                    catch
                    {
                        data.Add(splitData[1]);
                    }
                }
                else
                {
                    data.Add(splitData[1]);
                }
            }
        }

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();
            foreach (var i in key.Values)
            {
                try
                {
                    string rules = i.ValueData;
                    string[] ruleList = rules.Split('|');

                    List<String> action = new List<String>();
                    List<String> active = new List<String>();
                    List<String> dir = new List<String>();
                    List<String> protocol = new List<String>();
                    List<String> lport = new List<String>();
                    List<String> rport = new List<String>();
                    List<String> name = new List<String>();
                    List<String> desc = new List<String>();
                    List<String> app = new List<String>();

                    foreach (var j in ruleList)
                    {
                        SetData(@"Action=.*", j, action);
                        SetData(@"Active=.*", j, active);
                        SetData(@"Dir=.*", j, dir);
                        SetData(@"Protocol=.*", j, protocol);
                        SetData(@"LPort=.*", j,lport);
                        SetData(@"RPort=.*", j,rport);
                        SetData(@"Name=.*", j,name);
                        SetData(@"Desc=.*", j,desc);
                        SetData(@"App=.*", j, app);
                    }

                    var ff = new ValuesOut(string.Join(",", action), string.Join(",", active), string.Join(",", dir), 
                        string.Join(",", protocol), string.Join(",", lport), string.Join(",", rport), string.Join(",", name), string.Join(",", desc), string.Join(",", app))
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
