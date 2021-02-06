using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.WindowsPortableDevices
{
    public class WindowsPortableDevices : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public WindowsPortableDevices()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "77e9b1d3-3d8d-42d7-a400-5e0763f820c4";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\Windows Portable Devices"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Windows Portable Devices";

        public string ShortDescription
            => "Windows Portable Devices Information";

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

            var devices = key.SubKeys.SingleOrDefault(t => t.KeyName == "Devices");

            if (devices != null)
            {
                foreach (var subKey in devices.SubKeys)
                {
                    try
                    {
                        string keyName = subKey.KeyName;
                        string name = null;
                        string device = null;
                        string serialNumber = null;
                        string guid = null;
                        string friendlyName = subKey.Values.SingleOrDefault(t => t.ValueName == "FriendlyName")?.ValueData;

                        // https://github.com/keydet89/RegRipper3.0/blob/master/plugins/portdev.pl
                        if (new Regex(@"\#\#").IsMatch(keyName))
                            name = keyName.Split(new string[] { "##" }, StringSplitOptions.None)[1];

                        if (new Regex(@"\?\?").IsMatch(keyName))
                            name = keyName.Split(new string[] { "??" }, StringSplitOptions.None)[1];

                        if (name != null)
                        {
                            string[] result = name.Split('#');
                            device = result[1];
                            serialNumber = result[2];
                        }

                        var guidMatch = Regex.Match(keyName, @"[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]");
                        if (guidMatch.Success)
                            guid = guidMatch.Value;

                        DateTimeOffset? ts = subKey.LastWriteTime;

                        var ff = new ValuesOut(device, serialNumber, guid, friendlyName, ts)
                        {
                            BatchValueName = "Multiple",
                            BatchKeyPath = subKey.KeyPath
                        };
                        l.Add(ff);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add($"Error processing USB key: {ex.Message}");
                    }
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
