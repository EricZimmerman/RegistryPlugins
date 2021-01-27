using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.USBSTOR
{
    public class USBSTOR : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public USBSTOR()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "efcd3292-b6d7-4251-bd95-df6bdb34f0fe";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Enum\USBSTOR"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "USBSTOR";

        public string ShortDescription
            => "USBSTOR Information";

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

            try
            {
                foreach (var subKey in key.SubKeys)
                {
                    if (subKey.SubKeys.Count == 0)
                    {
                        continue;
                    }

                    string[] words = subKey.KeyName.Split('&');

                    if (words.Length != 4)
                    {
                        continue;
                    }
                    string Manufacture = words[1];
                    string Title = words[2];
                    string Version = words[3];
                    string serialNumber = subKey.SubKeys.First().KeyName;

                    DateTimeOffset? ts = subKey.LastWriteTime;

                    var ff = new ValuesOut(Manufacture, Title, Version, serialNumber, ts)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = key.KeyPath
                    };
                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing USBSTOR key: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}
