using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.ETW
{
    public class ETW : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public ETW()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "ebb62a86-4922-4410-a2ee-ef3a2a4a904a";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet001\Control\WMI\Autologger\EventLog-Application",
            @"ControlSet001\Control\WMI\Autologger\EventLog-System"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "0xHasanM";
        public string Email => "muhamedhasan@protonmail.com";
        public string Phone => "000-0000-0000";
        public string PluginName => "ETW";

        public string ShortDescription
            => "Cross reference ETW guids with providers";

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
            using (var reader = new StreamReader(@".\Settings\ETW.csv"))
            {
                StringDictionary Providers_GUID = new StringDictionary();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    
                    Providers_GUID.Add(values[0], values[1]);
                }
                foreach (var subkey in key.SubKeys)
                {
                    try
                    {
                        var Enabled_value = subkey.GetValue("Enabled").ToString();
                        var Enableproperty_value = subkey.GetValue("EnableProperty").ToString();
                        var ff = new ValuesOut(subkey.LastWriteTime.ToString(),
                                             subkey.KeyName,
                                             Providers_GUID[subkey.KeyName.Trim()],
                                             Enabled_value,
                                             Enableproperty_value)
                        {
                            BatchValueName = "Provider",
                            BatchKeyPath = subkey.KeyPath
                        };
                        l.Add(ff);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add(ex.Message);
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
