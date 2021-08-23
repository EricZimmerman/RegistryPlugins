using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Amcache_InventoryDriverBinary
{
    public class InventoryDriverBinary : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public InventoryDriverBinary()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "907cfa2a-27ea-4e3b-9fcd-41ae959b2ff6";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ROOT\InventoryDriverBinary"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Amcache-InventoryDriverBinary";

        public string ShortDescription
            => "Amcache-InventoryDriverBinary";

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
                    string path = subKey.KeyName;
                    string driverlastwritetime = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverLastWriteTime")?.ValueData;
                    string drivercompany = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverCompany")?.ValueData;
                    string drivername = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverName")?.ValueData;
                    string driverversion = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverVersion")?.ValueData;
                    string product = subKey.Values.SingleOrDefault(t => t.ValueName == "Product")?.ValueData;
                    string productversion = subKey.Values.SingleOrDefault(t => t.ValueName == "ProductVersion")?.ValueData;
                    string sha1 = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverId")?.ValueData.Replace("0000", "");
                    DateTimeOffset? ts = subKey.LastWriteTime;
                    var ff = new ValuesOut(driverlastwritetime, drivercompany, drivername, driverversion, product, productversion, path, sha1, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing InventoryDriverBinary key: {ex.Message}");
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
