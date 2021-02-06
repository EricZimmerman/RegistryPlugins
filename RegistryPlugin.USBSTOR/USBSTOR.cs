using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        private readonly List<string> GUIDs = new List<string> { "{540b947e-8b40-45bc-a8a2-6a0b894cbda2}", "{83da6326-97a6-4088-9453-a1923f573b29}"};

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

            foreach (var subKey in key.SubKeys)
            {
                if (subKey.SubKeys.Count == 0)
                {
                    continue;
                }

                try
                {
                    string[] words = subKey.KeyName.Split('&');

                    if (words.Length != 4)
                        continue;

                    string Manufacture = words[1];
                    string Title = words[2];
                    string Version = words[3];

                    var serialSubKey = subKey.SubKeys.First();
                    string serialNumber = serialSubKey.KeyName;

                    string deviceName = Encoding.Unicode?.GetString(
                        GetData(serialSubKey, GUIDs[0], "0004")
                    );
                    DateTimeOffset? installed = GetUTC(
                        GetData(serialSubKey, GUIDs[1], "0064")
                    );
                    DateTimeOffset? firstInstalled = GetUTC(
                        GetData(serialSubKey, GUIDs[1], "0065")
                    );
                    DateTimeOffset? lastConnected = GetUTC(
                        GetData(serialSubKey, GUIDs[1], "0066")
                    );
                    DateTimeOffset? lastRemoved = GetUTC(
                        GetData(serialSubKey, GUIDs[1], "0067")
                    );
                    DateTimeOffset? ts = subKey.LastWriteTime;

                    var ff = new ValuesOut(Manufacture, Title, Version, serialNumber, ts,
                        deviceName, installed, firstInstalled, lastConnected, lastRemoved)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = subKey.KeyPath
                    };
                    l.Add(ff);
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
