using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.RunMRU
{
    public class RunMRU : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public RunMRU()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
            AlertMessage = string.Empty;
        }

        public string InternalGuid => "1fe944ff-ecc3-4db8-bc60-f7fb7a01eb7c";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "RunMRU";

        public string ShortDescription =>
            "Extracts recently executed programs from RunMRU key";

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
                currentKey = key.KeyName;

                //get MRU key and read it in

                var mruVal = key.Values.SingleOrDefault(t => t.ValueName == "MRUList");

                var mruListOrder = new ArrayList();

                if (mruVal != null)
                {
                    foreach (var c in mruVal.ValueData.ToCharArray())
                    {
                        mruListOrder.Add(c.ToString());
                    }
                }


                foreach (var keyValue in key.Values)
                {
                    if (keyValue.ValueName == "MRUList")
                    {
                        continue;
                    }


                    var mru = mruListOrder.IndexOf(keyValue.ValueName);

                    DateTimeOffset? openedOn = null;

                    if (mru == 0)
                    {
                        openedOn = key.LastWriteTime;
                    }

                    var vd = keyValue.ValueData;

                    if (vd.EndsWith(@"\1"))
                    {
                        vd = keyValue.ValueData.Substring(0, keyValue.ValueData.Length - 2);
                    }

                    var v = new ValuesOut(keyValue.ValueName, vd, mru, openedOn);
                    v.BatchValueName = keyValue.ValueName;
                    v.BatchKeyPath = key.KeyPath;

                    valuesList.Add(v);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing RunMRU subkey {currentKey}: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            var v1 = valuesList.OrderBy(t => t.MruPosition);

            foreach (var source in v1.ToList())
            {
                _values.Add(source);
            }
        }


        public IBindingList Values => _values;
    }
}