using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryDeviceContainer
{
    public class InventoryDeviceContainer : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public InventoryDeviceContainer()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "971eef6b-c26e-429f-9ea9-96165742d926";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ROOT\InventoryDeviceContainer"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Amcache-InventoryDeviceContainer";

        public string ShortDescription
            => "Amcache-InventoryDeviceContainer";

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
                    string modelName = subKey.Values.SingleOrDefault(t => t.ValueName == "ModelName")?.ValueData;
                    string firendlyName = subKey.Values.SingleOrDefault(t => t.ValueName == "FriendlyName")?.ValueData;
                    string modelNumber = subKey.Values.SingleOrDefault(t => t.ValueName == "ModelNumber")?.ValueData;
                    string manufacturer = subKey.Values.SingleOrDefault(t => t.ValueName == "Manufacturer")?.ValueData;
                    string primaryCategory = subKey.Values.SingleOrDefault(t => t.ValueName == "PrimaryCategory")?.ValueData;
                    string modelid = subKey.Values.SingleOrDefault(t => t.ValueName == "ModelId")?.ValueData;
                    DateTimeOffset? ts = subKey.LastWriteTime;
                    var ff = new ValuesOut(modelName, firendlyName, modelNumber, manufacturer, primaryCategory, modelid, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing InventoryDeviceContainer key: {ex.Message}");
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
