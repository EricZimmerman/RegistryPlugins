using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryApplication
{
    public class InventoryApplication : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public InventoryApplication()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "1e06496f-eecb-41c1-8445-3dd38da4b23f";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ROOT\InventoryApplication"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Amcache-InventoryApplication";

        public string ShortDescription
            => "Amcache-InventoryApplication";

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
                    string name = subKey.Values.SingleOrDefault(t => t.ValueName == "Name")?.ValueData;
                    string version = subKey.Values.SingleOrDefault(t => t.ValueName == "Version")?.ValueData;
                    string publisher = subKey.Values.SingleOrDefault(t => t.ValueName == "Publisher")?.ValueData;
                    string source = subKey.Values.SingleOrDefault(t => t.ValueName == "Source")?.ValueData;
                    string rootDirPath = subKey.Values.SingleOrDefault(t => t.ValueName == "RootDirPath")?.ValueData;
                    string uninstallString = subKey.Values.SingleOrDefault(t => t.ValueName == "UninstallString")?.ValueData;
                    DateTimeOffset? ts = subKey.LastWriteTime;
                    var ff = new ValuesOut(name, version, publisher, source, rootDirPath, uninstallString, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing InventoryApplication key: {ex.Message}");
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
