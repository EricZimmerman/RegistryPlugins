using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Uninstall
{
    public class UnInstall : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public UnInstall()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "afedb770-1479-4ec1-977f-2f9facfcd9d2";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",     // NTUSER.DAT
            @"Microsoft\Windows\CurrentVersion\Uninstall",              // SOFTWARE
            @"WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"   // SOFTWARE
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Uninstall";

        public string ShortDescription
            => "Installed Programs";

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
                    string keyName = subKey.KeyName;
                    string displayName = subKey.Values.SingleOrDefault(t => t.ValueName == "DisplayName")?.ValueData;
                    string displayVersion = subKey.Values.SingleOrDefault(t => t.ValueName == "DisplayVersion")?.ValueData;
                    string Publisher = subKey.Values.SingleOrDefault(t => t.ValueName == "Publisher")?.ValueData;
                    string installDate = subKey.Values.SingleOrDefault(t => t.ValueName == "InstallDate")?.ValueData;
                    string installSource = subKey.Values.SingleOrDefault(t => t.ValueName == "InstallSource")?.ValueData;
                    string installLocation = subKey.Values.SingleOrDefault(t => t.ValueName == "InstallLocation")?.ValueData;
                    string uninstallString = subKey.Values.SingleOrDefault(t => t.ValueName == "UninstallString")?.ValueData;
                    DateTimeOffset? ts = subKey.LastWriteTime;

                    var ff = new ValuesOut(keyName, displayName, displayVersion, Publisher, installDate, installSource, installLocation, uninstallString, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing Uninstall key: {ex.Message}");
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
