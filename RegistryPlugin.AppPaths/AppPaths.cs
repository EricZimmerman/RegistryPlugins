using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppPaths
{
    public class AppPaths : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public AppPaths()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "d22bab60-393a-48cb-a1d7-45a67364f137";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\Windows\CurrentVersion\App Paths",          // SOFTWARE
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths"  // NTUSER.DAT
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "AppPaths";

        public string ShortDescription
            => "AppPaths Information";

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
                    string fileName = subKey.KeyName;
                    string path1 = subKey.Values.SingleOrDefault(t => t.ValueName == "(default)")?.ValueData;
                    string path2 = subKey.Values.SingleOrDefault(t => t.ValueName == "Path")?.ValueData;
                    DateTimeOffset? ts = subKey.LastWriteTime;

                    var ff = new ValuesOut(fileName, path1, path2, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing AppPaths key: {ex.Message}");
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
