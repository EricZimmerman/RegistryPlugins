using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryDevicePnp
{
    public class InventoryDevicePnp : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public InventoryDevicePnp()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "a4e5145f-9b5d-4b02-8a7e-0053550d4fcb";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ROOT\InventoryDevicePnp"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Amcache-InventoryDevicePnp";

        public string ShortDescription
            => "Amcache-InventoryDevicePnp";

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
                    string model = subKey.Values.SingleOrDefault(t => t.ValueName == "Model")?.ValueData;
                    string manufacturer = subKey.Values.SingleOrDefault(t => t.ValueName == "Manufacturer")?.ValueData;
                    string description = subKey.Values.SingleOrDefault(t => t.ValueName == "Description")?.ValueData;
                    string installDate = subKey.Values.SingleOrDefault(t => t.ValueName == "InstallDate")?.ValueData;
                    string parentId = subKey.Values.SingleOrDefault(t => t.ValueName == "ParentId")?.ValueData;
                    string matchingID = subKey.Values.SingleOrDefault(t => t.ValueName == "MatchingID")?.ValueData;
                    DateTimeOffset? ts = subKey.LastWriteTime;
                    var ff = new ValuesOut(model, manufacturer, description, installDate, parentId, matchingID, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing InventoryDevicePnp key: {ex.Message}");
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
