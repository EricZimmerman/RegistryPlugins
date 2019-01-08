using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.OpenSaveMRU
{
    public class OpenSaveMRU : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;


        public OpenSaveMRU()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "ad458121-8c1c-422a-ac24-7d395d15ca4a";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSaveMRU"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "ComDlg32 OpenSaveMRU";

        public string ShortDescription =>
            "Extracts file paths from OpenSaveMRU and subkeys";

        public string LongDescription => ShortDescription;


        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var valuesList = new List<ValuesOut>();

            var currentKey = string.Empty;


            try
            {
                //this key has folders stored in the root as well

                var mruVal1 = key.Values.SingleOrDefault(t => t.ValueName == "MRUList");

                var mruListOrder1 = new ArrayList();

                if (mruVal1 != null)
                {
                    foreach (var c in mruVal1.ValueData.ToCharArray())
                    {
                        mruListOrder1.Add(c.ToString());
                    }
                }

                foreach (var keyValue in key.Values)
                {
                    if (keyValue.ValueName == "MRUList")
                    {
                        continue;
                    }
                    var mru1 = mruListOrder1.IndexOf(keyValue.ValueName);

                    DateTimeOffset? openedOn1 = null;

                    if (mru1 == 0)
                    {
                        openedOn1 = key.LastWriteTime;
                    }

                    var v1 = new ValuesOut("OpenSaveMRU", keyValue.ValueData, keyValue.ValueName, mru1, openedOn1);
                    v1.BatchKeyPath = key.KeyPath;
                    v1.BatchValueName = keyValue.ValueName;

                    valuesList.Add(v1);
                }

                foreach (var registryKey in key.SubKeys)
                {
                    currentKey = registryKey.KeyName;

                    //get MRU key and read it in

                    var mruVal = registryKey.Values.SingleOrDefault(t => t.ValueName == "MRUList");

                    var mruListOrder = new ArrayList();

                    if (mruVal != null)
                    {
                        foreach (var c in mruVal.ValueData.ToCharArray())
                        {
                            mruListOrder.Add(c.ToString());
                        }
                    }

                    foreach (var keyValue in registryKey.Values)
                    {
                        if (keyValue.ValueName == "MRUList")
                        {
                            continue;
                        }

                        var mru = mruListOrder.IndexOf(keyValue.ValueName);

                        DateTimeOffset? openedOn = null;

                        if (mru == 0)
                        {
                            openedOn = registryKey.LastWriteTime;
                        }

                        var v = new ValuesOut(registryKey.KeyName, keyValue.ValueData, keyValue.ValueName, mru,
                            openedOn);
                        v.BatchKeyPath = registryKey.KeyPath;
                        v.BatchValueName = keyValue.ValueName;

                        valuesList.Add(v);
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing OpenSaveMRU subkey {currentKey}: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }


            var v2 = valuesList.OrderBy(t => t.MruPosition);

            foreach (var source in v2.ToList())
            {
                _values.Add(source);
            }
        }


        public IBindingList Values => _values;
    }
}