using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;


namespace RegistryPlugin.SWD
{
    public class SWD : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public SWD()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "4d2096fa-03a1-4b1a-aabf-4b2d8226cff6";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Enum\SWD"
        });
        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "SWD";
        public string ShortDescription => "SWD Information";
        
        public string LongDescription => "https://www.binary-zone.com/2020/04/04/investigating-usb-drives-using-mount-points-not-drive-letters/, https://systemcenterdiary.wordpress.com/2020/09/17/clean-up-the-wsd-and-upnp-device-list/";

        public double Version => 0.1;
        public List<string> Errors { get; }

        private readonly List<string> GUIDs = new List<string> { "{540b947e-8b40-45bc-a8a2-6a0b894cbda2}", "{83da6326-97a6-4088-9453-a1923f573b29}" };

        private byte[] GetData(RegistryKey serialSubKey, string guidValue, string numValue)
        {
            var properties = serialSubKey.SubKeys.SingleOrDefault(t => t.KeyName == "Properties");
            if (properties == null)
                return null;

            var GUID = properties.SubKeys.SingleOrDefault(t => t.KeyName == guidValue);
            if (GUID == null)
                return null;

            var subKey = GUID.SubKeys.SingleOrDefault(t => t.KeyName == numValue);
            if (subKey == null)
                return null;

            return subKey.Values.SingleOrDefault(t => t.ValueName == "(default)")?.ValueDataRaw;
        }

        private DateTimeOffset? GetUTC(byte[] data)
        {
            if (data == null || data.Length != 8)
                return null;

            return DateTimeOffset.FromFileTime(BitConverter.ToInt64(data, 0)).ToUniversalTime();
        }

        private string SplitData(string data)
        {
            if (data != null)
            {
                string[] split = data.Split(';');
                return split[split.Length - 1];
            }
            else
            {
                return null;
            }
        }

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

            foreach (var registryKey in key.SubKeys)
            {
                try
                {
                    string type = registryKey.KeyName;

                    foreach (var subKey in registryKey.SubKeys)
                    {
                        string keyName = subKey.KeyName;
                        string service = subKey.Values.SingleOrDefault(t => t.ValueName == "Service")?.ValueData;
                        string deviceDesc = SplitData(subKey.Values.SingleOrDefault(t => t.ValueName == "DeviceDesc")?.ValueData);
                        string friendlyName = SplitData(subKey.Values.SingleOrDefault(t => t.ValueName == "FriendlyName")?.ValueData);

                        var installed = GetUTC(
                            GetData(subKey, GUIDs[1], "0064")
                        );
                        var firstInstalled = GetUTC(
                            GetData(subKey, GUIDs[1], "0065")
                        );
                        var lastConnected = GetUTC(
                             GetData(subKey, GUIDs[1], "0066")
                         );
                        var lastRemoved = GetUTC(
                            GetData(subKey, GUIDs[1], "0067")
                        );

                        var ff = new ValuesOut(type, keyName, service, deviceDesc, friendlyName, installed, firstInstalled, lastConnected, lastRemoved)
                        {
                            BatchValueName = "Multiple",
                            BatchKeyPath = subKey.KeyPath
                        };
                        l.Add(ff);
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing SWD key: {ex.Message}");
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
