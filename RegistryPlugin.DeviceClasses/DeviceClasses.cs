using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.DeviceClasses
{
    public class DeviceClasses : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public DeviceClasses()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "8be740fe-9d72-4876-a066-6e78a20c8d19";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Control\DeviceClasses",
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "DevicesClasses";

        public string ShortDescription
            => "DevicesClasses Information";

        public string LongDescription => "http://forensic-proof.com/archives/3632";

        public double Version => 0.1;
        public List<string> Errors { get; }

        public List<string> GUIDs = new List<string> { "{53F5630D-B6BF-11D0-94F2-00A0C91EFB8B}", "{A5DCBF10-6530-11D2-901F-00C04FB951ED}", "{53F56307-B6BF-11D0-94F2-00A0C91EFB8B}", "{6AC27878-A6FA-4155-BA85-F98F491D4F33}"};

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

        private ValuesOut ParseData(RegistryKey subKey)
        {
            string[] keyName = subKey.KeyName.Replace("##?#", "").Split('#');

            string Type = keyName[0];
            string Name = keyName[1];
            string serialNumber = keyName[2];

            DateTimeOffset? ts = subKey.LastWriteTime;

            var ff = new ValuesOut(Type, Name, serialNumber, ts)
            {
                BatchValueName = "Multiple",
                BatchKeyPath = subKey.KeyPath
            };
            return ff;
        }

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();

            foreach (var registryKey in key.SubKeys)
            {
                try
                { 
                    bool contains = GUIDs.Contains(registryKey.KeyName, StringComparer.OrdinalIgnoreCase);
                    if (!contains)
                        continue;

                    foreach (var subKey in registryKey.SubKeys)
                        l.Add(ParseData(subKey));
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing USBSTOR key: {ex.Message}");
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
