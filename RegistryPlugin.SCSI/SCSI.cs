using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlguin.SCSI
{
    public class SCSI : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public SCSI()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "a80dee0c-d1a4-4c57-9c03-670045b8f1db";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Enum\SCSI"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "SCSI";

        public string ShortDescription
            => "SCSI Information";

        public string LongDescription => ShortDescription;

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
                    var words = subKey.KeyName.Split('&');

                    if (words.Length != 3)
                        continue;

                    var Manufacturer = words[1];
                    var Title = words[2];

                    foreach (var serialSubKey in subKey.SubKeys)
                    {
                        var serialNumber = serialSubKey.KeyName;

                        var deviceName = Encoding.Unicode?.GetString(
                            GetData(serialSubKey, GUIDs[0], "0004")
                        );
                        var installed = GetUTC(
                            GetData(serialSubKey, GUIDs[1], "0064")
                        );
                        var firstInstalled = GetUTC(
                            GetData(serialSubKey, GUIDs[1], "0065")
                        );
                        var lastConnected = GetUTC(
                             GetData(serialSubKey, GUIDs[1], "0066")
                         );
                        var lastRemoved = GetUTC(
                            GetData(serialSubKey, GUIDs[1], "0067")
                        );
                        var ts = subKey.LastWriteTime;

                        var deviceParametersKey = subKey.SubKeys.First().SubKeys.SingleOrDefault(t => t.KeyName == "Device Parameters");

                        var diskId = "";
                        DateTimeOffset? initialTimestamp = null;

                        if (deviceParametersKey != null)
                        {
                            var aaaa = deviceParametersKey.SubKeys.SingleOrDefault(t => t.KeyName == "Partmgr");
                            var storportKey = deviceParametersKey.SubKeys.SingleOrDefault(t => t.KeyName == "Storport");

                            if (aaaa != null)
                            {
                                var ddd = aaaa.Values.SingleOrDefault(t => t.ValueName == "DiskId");

                                if (ddd != null)
                                {
                                    diskId = ddd.ValueData;
                                }
                            }

                            if (storportKey != null)
                            {
                                var ddd = storportKey.Values.SingleOrDefault(t => t.ValueName == "InitialTimestamp");

                                if (ddd != null)
                                {
                                    initialTimestamp = GetUTC(ddd.ValueDataRaw);
                                }
                            }
                        }
                        var ff = new ValuesOut(ts, Manufacturer, Title, serialNumber,
                        deviceName, initialTimestamp, installed, firstInstalled, lastConnected, lastRemoved, diskId)
                        {
                            BatchValueName = "Multiple",
                            BatchKeyPath = subKey.KeyPath
                        };
                        l.Add(ff);
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing SCSI key: {ex.Message}");
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