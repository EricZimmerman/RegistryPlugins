using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;


namespace RegistryPlugin.Mare
{
    public class Mare : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public Mare()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "37f8757b-1403-48ef-bef7-85c7a0d502b2";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Root\Mare"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Mare";

        public string ShortDescription
           => "Mare";

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
                    var restore = subKey.Values.SingleOrDefault(t => t.ValueName == "restore")?.ValueData;
                    if (restore != null)
                    {
                        var lastIndex = restore.LastIndexOf('|');
                        if (lastIndex != -1)
                        {
                            var unknown = restore.Substring(0, lastIndex);
                            var path = restore.Substring(lastIndex + 1);

                            var ff = new ValuesOut(unknown, path)
                            {
                                BatchValueName = "Multiple",
                                BatchKeyPath = subKey.KeyPath
                            };
                            l.Add(ff);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing Mare key: {ex.Message}");
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
