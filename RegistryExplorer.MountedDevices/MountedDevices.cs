using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.MountedDevices
{
    public class MountedDevices : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public MountedDevices()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "363a237e-0b61-4883-b4f5-53c023f8b87f";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"MountedDevices"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "MountedDevices";

        public string ShortDescription
            => "Displays mounted devices including GUIDs and device information";

        public string LongDescription => ShortDescription;


        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var currVal = string.Empty;


            try
            {
                foreach (var keyValue in key.Values)
                {
                    var vData = string.Empty;

                    var first = Convert.ToString(keyValue.ValueDataRaw[0]);

                    switch (first)
                    {
                        case "{":
                        case "\\":
                        case "_":
                            
                            vData = Encoding.Unicode.GetString(keyValue.ValueDataRaw);
                            break;
                            
                            default:
                                vData = Encoding.GetEncoding(1252).GetString(keyValue.ValueDataRaw);
                                break;

                    }

                    currVal = keyValue.ValueName;

                    

                    var v1 = new ValuesOut(keyValue.ValueName, vData);

                    Values.Add(v1);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing MountedDevices value {currVal}: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public IBindingList Values => _values;
    }
}