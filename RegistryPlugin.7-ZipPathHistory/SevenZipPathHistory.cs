using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin._7_ZipPathHistory
{
    public class SevenZipPathHistory : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public SevenZipPathHistory()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }

        public string InternalGuid => "d86e955c-d1c0-4da4-b4d0-5cfb7a9ba5a5";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\7-Zip\Extraction"
        });

        public string ValueName => "PathHistory";
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => string.Empty;
        public string Phone => string.Empty;
        public string PluginName => "7-Zip extraction path history";

        public string ShortDescription =>
            "Extracts the extraction destination history from the PathHistory value";

        public string LongDescription => ShortDescription;

        public double Version => 0.1;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();
            AlertMessage = null;

            try
            {
                var pathHistory = key.Values.FirstOrDefault(t => t.ValueName == ValueName);

                if (pathHistory != null)
                {
                    var paths = Encoding.Unicode.GetString(pathHistory.ValueDataRaw).Split('\0');
                    // TODO: More testing is required before presenting history order.
                    // var order = 1;

                    foreach (var path in paths)
                    {
                        if (path.Trim().Length == 0)
                        {
                            continue;
                        }

                        // var value = new ValuesOut(path, order++)
                        var value = new ValuesOut(path)
                        {
                            BatchKeyPath = key.KeyPath,
                            BatchValueName = pathHistory.ValueName
                        };

                        Values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing 7-Zip extraction path history: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public IBindingList Values => _values;
    }
}
