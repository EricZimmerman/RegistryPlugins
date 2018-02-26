using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.BluetoothServicesBthPort
{
    public class BluetoothBthPort : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public BluetoothBthPort()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "aa11dce3-ce1e-4a70-abbe-7a18419afe33";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet001\services\BTHPORT\Parameters\Devices",
            @"ControlSet002\services\BTHPORT\Parameters\Devices"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }

        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;

        public string Author => "Frank Coleman";
        public string Email => "f.a.coleman@gmail.com";
        public string Phone => "941-799-9330";
        public string PluginName => "BluetoothBthPort";

        public string ShortDescription =>
            "Displays Bluetooth connection history in a simple viewing format";

        public string LongDescription =>
            "";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();
            foreach (var vout in ProcessKey(key))
            {
                _values.Add(vout);
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

                    var ff = ProcessVals(subKey, subKey.KeyName);

                    if (ff != null)
                    {
                        l.Add(ff);
                    }

                    foreach (var sKey in subKey.SubKeys)
                    {
                        if (sKey.SubKeys.Count == 0)
                        {
                            continue;
                        }

                        var ff1 = ProcessVals(sKey, subKey.KeyName);
                        if (ff1 != null)
                        {
                            l.Add(ff1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Bluetooth Devices key: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }

        private ValuesOut ProcessVals(RegistryKey ssKey, string originKey)
        {
            var btname = ssKey.Values.SingleOrDefault(t => t.ValueName == "Name");
            if (btname == null)
            {
                return null;
            }

            var name = btname.ValueData;
            return new ValuesOut(name, originKey, ssKey.LastWriteTime);
        }
    }
}