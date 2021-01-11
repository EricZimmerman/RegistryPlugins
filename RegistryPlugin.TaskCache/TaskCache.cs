using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TaskCache
{
    public class TaskCache : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;


        public TaskCache()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }


        public string InternalGuid => "2a7a503e-46c6-5a8a-b310-bc7b5eb22b00";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "TaskCache";

        public string ShortDescription =>
            "Displays values from TaskCache keys";

        public string LongDescription =>
            ShortDescription;

        public double Version => 0.5;

        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var valuesList = new List<ValuesOut>();

            var gkn = string.Empty;

            try
            {
                foreach (var guidKey in key.SubKeys)
                {
                    gkn = guidKey.KeyName;

                    var source = guidKey.Values.SingleOrDefault(t => t.ValueName == "Source");
                    var author = guidKey.Values.SingleOrDefault(t => t.ValueName == "Author");
                    var desc = guidKey.Values.SingleOrDefault(t => t.ValueName == "Description");
                    var path = guidKey.Values.SingleOrDefault(t => t.ValueName == "Path");

                    var blob = guidKey.Values.SingleOrDefault(t => t.ValueName == "DynamicInfo");

                    var ver = -1;
                    DateTimeOffset created;
                    DateTimeOffset? lastStart = null;
                    DateTimeOffset? lastStop = null;
                    var taskState = -1;
                    var lastActionResult = -1;

                    if (blob == null)
                    {
                        continue;
                    }
                 
                    ver = BitConverter.ToInt32(blob.ValueDataRaw, 0);
                    var createdRaw = BitConverter.ToInt64(blob.ValueDataRaw, 4);
                    created = DateTimeOffset.FromFileTime(createdRaw).ToUniversalTime();

                    var lastStartRaw = BitConverter.ToInt64(blob.ValueDataRaw, 0xc);
                    if (lastStartRaw > 0)
                    {
                        lastStart = DateTimeOffset.FromFileTime(lastStartRaw).ToUniversalTime();
                    }

                    var lastStopRaw = BitConverter.ToInt64(blob.ValueDataRaw, 0x1c);
                    if (lastStopRaw > 0)
                    {
                        lastStop = DateTimeOffset.FromFileTime(lastStopRaw).ToUniversalTime();
                    }

                    lastActionResult = BitConverter.ToInt32(blob.ValueDataRaw, 0x18);
                    taskState = BitConverter.ToInt32(blob.ValueDataRaw, 0x14);

                    var v = new ValuesOut(ver, gkn, created, lastStart, lastStop, taskState, lastActionResult, source?.ValueData, desc?.ValueData, author?.ValueData,path?.ValueData);

                    _values.Add(v);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing TaskCache key '{gkn}': {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }


            var v1 = valuesList;

            foreach (var source in v1.ToList())
            {
                _values.Add(source);
            }
        }

        public IBindingList Values => _values;
    }
}