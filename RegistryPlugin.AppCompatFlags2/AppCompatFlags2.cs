using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppCompatFlags2
{
    public class AppCompatFlags2 : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public AppCompatFlags2()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "7de11f17-a671-4e80-adfb-0bf5045f8433";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "AppCompatFlags2";

        public string ShortDescription
            => "AppCompatFlags Compatibility Assistant & Layers Information";

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

        public ValuesOut addData(Registry.Abstractions.KeyValue data, RegistryKey subKey)
        {
            var ff = new ValuesOut(data?.ValueName)
            {
                BatchValueName = "Multiple",
                BatchKeyPath = subKey.KeyPath
            };
            return ff;
        }
        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();

            foreach (var subKey in key.SubKeys)
            {
                try
                {
                    if (subKey.KeyName == "Layers")
                    {
                        foreach (var i in subKey.Values)
                        {
                            l.Add((addData(i, subKey)));
                        }
                    }
                    else if (subKey.KeyName == "Compatibility Assistant")
                    {
                        foreach (var i in subKey.SubKeys) // Persisted, Store
                        {
                            foreach (var j in i.Values)
                            {
                                l.Add(addData(j, subKey));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing AppCompatFlags key: {ex.Message}");
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
