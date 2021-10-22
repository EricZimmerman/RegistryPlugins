using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Products
{
    public class Products : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public Products()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "9216183d-09a2-479a-8d48-7616c38f2b1a";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\Windows\CurrentVersion\Installer\UserData\*\Products"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Products";

        public string ShortDescription
            => "Products Information";

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
                    var properties = subKey.SubKeys.SingleOrDefault(t => t.KeyName == "InstallProperties");
                    if (properties == null)
                        continue;

                    var displayName = properties.Values.SingleOrDefault(t => t.ValueName == "DisplayName")?.ValueData;
                    var packageName = properties.Values.SingleOrDefault(t => t.ValueName == "DisplayVersion")?.ValueData;
                    var installDate = properties.Values.SingleOrDefault(t => t.ValueName == "InstallDate")?.ValueData;
                    var publisher = properties.Values.SingleOrDefault(t => t.ValueName == "Publisher")?.ValueData;
                    var language = properties.Values.SingleOrDefault(t => t.ValueName == "Language")?.ValueData;
                    var installLocation = properties.Values.SingleOrDefault(t => t.ValueName == "InstallLocation")?.ValueData;
                    var installSource = properties.Values.SingleOrDefault(t => t.ValueName == "InstallSource")?.ValueData;
                    var comments = properties.Values.SingleOrDefault(t => t.ValueName == "Comments")?.ValueData;
                    DateTimeOffset? ts = properties.LastWriteTime;

                    var ff = new ValuesOut(displayName, packageName, installDate, publisher, language, installLocation, installSource, comments, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing Products key: {ex.Message}");
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
