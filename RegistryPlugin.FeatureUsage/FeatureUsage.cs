using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FeatureUsage
{
    public class FeatureUsage : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public FeatureUsage()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "64b1f83e-d820-4d54-aac3-af8accb83009";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\FeatureUsage"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "FeatureUsage";

        public string ShortDescription
            => "FeatureUsage Information";

        public string LongDescription 
            => "https://blog.group-ib.com/featureusage";

        public double Version => 0.2;
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
                    foreach (var i in subKey.Values)
                    {
                        var ff = new ValuesOut(subKey.KeyName, i?.ValueName, i?.ValueData)
                        {
                            BatchValueName = "Multiple",
                            BatchKeyPath = subKey.KeyPath
                        };
                        l.Add(ff);
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing FeatureUsage key: {ex.Message}");
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
