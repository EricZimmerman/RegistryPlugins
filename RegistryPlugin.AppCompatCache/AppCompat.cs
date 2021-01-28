using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;
using acc = AppCompatCache.AppCompatCache;

namespace RegistryPlugin.AppCompatCache
{
    public class AppCompat : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public AppCompat()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "f2afe58b-8443-4f7c-8bc8-eeca9434dd2d";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet00*\Control\Session Manager\AppCompatCache"
        });

        public string ValueName => "AppCompatCache";
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "AppCompatCache";

        public string ShortDescription
            =>
                "Tracks application compatibility. The cache data tracks file path, size, and last modified time. In some cases, an executed flag is also available.";

        public string LongDescription
            =>
                "Useful to show evidence of execution";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var appcompatValue = key.Values.Single(t => t.ValueName == ValueName);

            var ctl = key.KeyPath.Split('\\').SingleOrDefault(t => t.Contains("ControlSet"));

            var num = -1;

            if (ctl != null)
            {
                num = ctl.ToCharArray().Last();
            }

            var env = key.Parent.SubKeys.Single(t => t.KeyName == "Environment");

            var val = env?.Values.SingleOrDefault(c => c.ValueName == "PROCESSOR_ARCHITECTURE");


            var is32bit = false;
            if (val != null)
            {
                is32bit= val.ValueData.Equals("x86");
            }


            try
            {
                var cache = new acc(appcompatValue.ValueDataRaw, num,is32bit);

                foreach (var c in cache.Caches)
                {
                    foreach (var cacheEntry in c.Entries)
                    {
                        try
                        {
                            var vo = new ValuesOut(cacheEntry.CacheEntryPosition, cacheEntry.Path,
                                cacheEntry.LastModifiedTimeUTC);

                            vo.BatchKeyPath = key.KeyPath;
                            vo.BatchValueName = ValueName;

                            _values.Add(vo);

                        }
                        catch (Exception ex)
                        {
                            Errors.Add($"Value name: {cacheEntry.CacheEntryPosition}, message: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing AppCompatCache: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}