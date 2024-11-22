using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.ProfileList
{
    public class ProfileList : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public ProfileList()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "fe25dd08-0fde-4785-b44a-94bc15661b34";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\Windows NT\CurrentVersion\ProfileList"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "ProfileList";

        public string ShortDescription
            => "ProfileList Information";

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

        public DateTimeOffset? GetDateTime(string high, string low)
        {
            if (high == null || low == null) return null;
            string hexString = Convert.ToInt64(high).ToString("X") + Convert.ToInt64(low).ToString("X");
            var timestampInt = Convert.ToInt64(hexString, 16);
            var dt1 = DateTime.FromFileTime(timestampInt);
            return new DateTimeOffset(dt1);
        }

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();

            foreach (var subKey in key.SubKeys)
            {
                try
                {
                    string keyName = subKey.KeyName;
                    string profileImagePath = subKey.Values.SingleOrDefault(t => t.ValueName == "ProfileImagePath")?.ValueData;
                    string loadTimeHigh = subKey.Values.SingleOrDefault(t => t.ValueName == "LocalProfileLoadTimeHigh")?.ValueData;
                    string loadTimeLow = subKey.Values.SingleOrDefault(t => t.ValueName == "LocalProfileLoadTimeLow")?.ValueData;
                    string unloadTimeHigh = subKey.Values.SingleOrDefault(t => t.ValueName == "LocalProfileUnloadTimeHigh")?.ValueData;
                    string unloadTimeLow = subKey.Values.SingleOrDefault(t => t.ValueName == "LocalProfileUnloadTimeLow")?.ValueData;
                    DateTimeOffset? ts = subKey.LastWriteTime;

                    DateTimeOffset? loadTime = GetDateTime(loadTimeHigh, loadTimeLow);
                    DateTimeOffset? unloadTime = GetDateTime(unloadTimeHigh, unloadTimeLow);

                    var ff = new ValuesOut(keyName, profileImagePath, ts, loadTime, unloadTime)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing ProfileList key: {ex.Message}");
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
