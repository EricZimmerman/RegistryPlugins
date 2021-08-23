using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryApplicationFile
{
    public class InventoryApplicationFile : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public InventoryApplicationFile()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "625ee594-f17d-4e35-882b-9b29fc6bc359";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ROOT\InventoryApplicationFile"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Amcache-InventoryApplicationFile";

        public string ShortDescription
            => "Amcache-InventoryApplicationFile";

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
                    string lowerCaseLongPath = subKey.Values.SingleOrDefault(t => t.ValueName == "LowerCaseLongPath")?.ValueData;
                    string name = subKey.Values.SingleOrDefault(t => t.ValueName == "Name")?.ValueData;
                    string productName = subKey.Values.SingleOrDefault(t => t.ValueName == "ProductName")?.ValueData;
                    string publisher = subKey.Values.SingleOrDefault(t => t.ValueName == "Publisher")?.ValueData;
                    string version = subKey.Values.SingleOrDefault(t => t.ValueName == "Version")?.ValueData;
                    string sha1 = subKey.Values.SingleOrDefault(t => t.ValueName == "FileId")?.ValueData.Replace("0000", "");
                    DateTimeOffset? ts = subKey.LastWriteTime;
                    var ff = new ValuesOut(lowerCaseLongPath, name, productName, publisher, version, sha1, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing InventoryApplicationFile key: {ex.Message}");
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
