using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin._7_ZipCopyHistory
{
    public class SevenZipCopyHistory : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public SevenZipCopyHistory()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }

        public string InternalGuid => "a432b868-004f-4428-9429-1b3c6d25000c";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\7-Zip\FM"
        });

        public string ValueName => "CopyHistory";
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Phill Moore";
        public string Email => string.Empty;
        public string Phone => string.Empty;
        public string PluginName => "7-Zip copy history";

        public string ShortDescription =>
            "Extracts copy history from CopyHistory value";

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
                var copyHistory = key.Values.SingleOrDefault(t => t.ValueName == ValueName);

                if (copyHistory != null)
                {
                    var paths = Encoding.Unicode.GetString(copyHistory.ValueDataRaw).Split('\0');
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
                            BatchValueName = copyHistory.ValueName
                        };

                        Values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing 7-Zip copy history: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public IBindingList Values => _values;
    }
}
