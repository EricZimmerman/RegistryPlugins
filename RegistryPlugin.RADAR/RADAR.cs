using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugins.RADAR
{
    public class RADAR : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public RADAR()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "76ce46f0-297c-4679-b625-03047b8a0635";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\RADAR\HeapLeakDetection"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "RADAR";

        public string ShortDescription
            => "RADAR - HeapLeakDetection";

        public string LongDescription =>
            "http://windowsir.blogspot.com/2011/09/registry-stuff.html";

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

        public DateTimeOffset GetDateTimeOffset(string timestamp)
        {
            var timestampInt = Convert.ToInt64(timestamp);
            return DateTime.FromFileTime(timestampInt).ToUniversalTime();
        }

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();

            var diagnosedApplicationsKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "DiagnosedApplications");
            var reflectionApplicationsKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "ReflectionApplications");

            foreach (var subKey in diagnosedApplicationsKey.SubKeys)
            {
                try
                {
                    var lastdetectiontime = GetDateTimeOffset(subKey.Values.SingleOrDefault(t => t.ValueName == "LastDetectionTime")?.ValueData);

                    var ff = new ValuesOut("DiagnosedApplications", subKey.KeyName, lastdetectiontime)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing RADAR key: {ex.Message}");
                }
            }

            foreach (var subKey in reflectionApplicationsKey.SubKeys)
            {
                try
                {
                    var ff = new ValuesOut("ReflectionApplications", subKey.KeyName, null)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing RADAR key: {ex.Message}");
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
