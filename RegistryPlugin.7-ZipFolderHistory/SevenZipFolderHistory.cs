using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin._7_ZipFolderHistory
{
    public class SevenZipFolderHistory : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public SevenZipFolderHistory()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }

        public string InternalGuid => "7a44eb25-cec1-4ab2-94d2-157fc77b9620";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\7-Zip\FM"
        });

        public string ValueName => "FolderHistory";
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Phill Moore";
        public string Email => string.Empty;
        public string Phone => string.Empty;
        public string PluginName => "7-Zip folder history";

        public string ShortDescription =>
            "Extracts folder history from FolderHistory value";

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
                var folderHistory = key.Values.SingleOrDefault(t => t.ValueName == ValueName);

                if (folderHistory != null)
                {
                    var folders = Encoding.Unicode.GetString(folderHistory.ValueDataRaw).Split('\0');
                    // TODO: More testing is required before presenting history order.
                    // var order = 1;

                    foreach (var folder in folders)
                    {
                        if (folder.Trim().Length == 0)
                        {
                            continue;
                        }

                        // var value = new ValuesOut(folder, order++)
                        var value = new ValuesOut(folder)
                        {
                            BatchKeyPath = key.KeyPath,
                            BatchValueName = folderHistory.ValueName
                        };

                        Values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing 7-Zip folder history: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public IBindingList Values => _values;
    }
}
