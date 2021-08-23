using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryApplicationShortcut
{
    public class InventoryApplicationShortcut : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public InventoryApplicationShortcut()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "02196a7c-6961-472e-9213-89af407da777";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ROOT\InventoryApplicationShortcut"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Amcache-InventoryApplicationShortcut";

        public string ShortDescription
            => "Amcache-InventoryApplicationShortcut";

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
                    string shortcutPath = subKey.Values.SingleOrDefault(t => t.ValueName == "ShortcutPath")?.ValueData;
                    DateTimeOffset? ts = subKey.LastWriteTime;
                    var ff = new ValuesOut(shortcutPath, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing InventoryApplicationShortcut key: {ex.Message}");
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
