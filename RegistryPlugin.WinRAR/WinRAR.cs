using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.WinRAR
{
    public class WinRAR : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public WinRAR()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "9bea8d7b-2e53-4a7a-ac82-f16511b69e0a";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\WinRAR"
        });
        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "WinRAR";

        public string ShortDescription
            => "WinRAR Information";

        public string LongDescription 
            => "https://github.com/libyal/winreg-kb/blob/main/documentation/WinRar%20keys.asciidoc";

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
            var ff = new ValuesOut(data?.ValueData)
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
                    if (subKey.KeyName == "ArcHistory")
                    {
                        foreach (var i in subKey.Values)
                        {
                            l.Add(addData(i, subKey));
                        }
                    } 
                    else if (subKey.KeyName == "DialogEditHistory")
                    {
                        foreach (var i in subKey.SubKeys)
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
                    Errors.Add($"Error processing WinRAR key: {ex.Message}");
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
