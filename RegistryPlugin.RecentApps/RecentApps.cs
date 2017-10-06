using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.RecentApps
{
    public class RecentApps : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public RecentApps()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "2a7a502e-26c6-4a8a-b310-bc9b5ad20b00";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Search\RecentApps",
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "RecentApps";

        public string ShortDescription =>
            "Displays values from RecentApps subkeys and extracts information about program execution, last access time, launch count, and recent items";

        public string LongDescription =>
            "Displays values from RecentApps subkeys and extracts information about program execution, last access time, launch count, and recent items";

        public double Version => 0.5;

        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();


            var vn = string.Empty;

            try
            {
                foreach (var registryKey in key.SubKeys)
                {
                    var appId = registryKey.Values.SingleOrDefault(t => t.ValueName == "AppId")?.ValueData;
                    var appPath = registryKey.Values.SingleOrDefault(t => t.ValueName == "AppPath")?.ValueData;
                    var lAccess = registryKey.Values.SingleOrDefault(t => t.ValueName == "LastAccessedTime")?.ValueData;
                    var lc = registryKey.Values.SingleOrDefault(t => t.ValueName == "LaunchCount")?.ValueData;

                        var vo = new ValuesOut(registryKey.KeyName,appId,appPath,DateTimeOffset.FromFileTime(long.Parse(lAccess)),int.Parse(lc));

                    var recentItems = registryKey.SubKeys.SingleOrDefault(t => t.KeyName == "RecentItems");

                    if (recentItems != null)
                    {
                        foreach (var recentItemsSubKey in recentItems.SubKeys)
                        {
                            var displayName = recentItemsSubKey.Values.SingleOrDefault(t => t.ValueName == "DisplayName")?.ValueData;
                            var lAccess2 = recentItemsSubKey.Values.SingleOrDefault(t => t.ValueName == "LastAccessedTime")?.ValueData;
                            var appPath2 = recentItemsSubKey.Values.SingleOrDefault(t => t.ValueName == "Path")?.ValueData;

                            vo.RecentItems.Add(new RecentItem(recentItemsSubKey.KeyName,displayName,DateTimeOffset.FromFileTime(long.Parse(lAccess2)),appPath2));
                            
                        }
                    }

                    _values.Add(vo);
                }
                
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing RecentApps value '{vn}': {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public IBindingList Values => _values;
    }
}
