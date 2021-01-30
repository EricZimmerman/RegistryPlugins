using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.NetworkAdapters
{
    public class NetworkAdapters : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public NetworkAdapters()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "0fff7743-08b3-4fdb-8514-6559bf3d009d";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "NetworkAdapters";

        public string ShortDescription
            => "NetworkAdapters Information";

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
                    Regex regex = new Regex(@"^00\d\d$");
                    if (!regex.IsMatch(subKey.KeyName))
                        continue;

                    string driverDesc = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverDesc")?.ValueData;
                    string driverDate = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverDate")?.ValueData;
                    string driverVersion = subKey.Values.SingleOrDefault(t => t.ValueName == "DriverVersion")?.ValueData;
                    string deviceInstanceID = subKey.Values.SingleOrDefault(t => t.ValueName == "DeviceInstanceID")?.ValueData;
                    string providerName = subKey.Values.SingleOrDefault(t => t.ValueName == "ProviderName")?.ValueData;

                    DateTimeOffset? ts = subKey.LastWriteTime;

                    var ff = new ValuesOut(driverDesc, driverDate, driverVersion, deviceInstanceID, providerName, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing {{4d36e972-e325-11ce-bfc1-08002be10318}} key: {ex.Message}");
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
