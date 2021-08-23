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
                    string displayName = null;
                    string packageName = null;
                    string installDate = null;
                    string publisher = null;
                    string language = null;
                    string installLocation = null;
                    string installSource = null;
                    string comments = null;
                    DateTimeOffset? ts = null;

                    var properties = subKey.SubKeys.SingleOrDefault(t => t.KeyName == "InstallProperties");
                    if (properties != null) {
                        displayName = properties.Values.SingleOrDefault(t => t.ValueName == "DisplayName")?.ValueData;
                        packageName = properties.Values.SingleOrDefault(t => t.ValueName == "DisplayVersion")?.ValueData;
                        installDate = properties.Values.SingleOrDefault(t => t.ValueName == "InstallDate")?.ValueData;
                        publisher = properties.Values.SingleOrDefault(t => t.ValueName == "Publisher")?.ValueData;
                        language = properties.Values.SingleOrDefault(t => t.ValueName == "Language")?.ValueData;
                        installLocation = properties.Values.SingleOrDefault(t => t.ValueName == "InstallLocation")?.ValueData;
                        installSource = properties.Values.SingleOrDefault(t => t.ValueName == "InstallSource")?.ValueData;
                        comments = properties.Values.SingleOrDefault(t => t.ValueName == "Comments")?.ValueData;
                        ts = properties.LastWriteTime;
                    }

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
