using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.BamDam
{
 public class BamDam : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public BamDam()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "07770c31-7845-4c11-b683-c09c41f76899";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet001\Services\bam\UserSettings\*",
            @"ControlSet002\Services\bam\UserSettings\*",
            @"ControlSet001\Services\dam\UserSettings\*",
            @"ControlSet002\Services\dam\UserSettings\*"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "BamDam";
        public string ShortDescription => "Extracts program information and last run times from bam and dam keys";

        public string LongDescription
            =>"https://padawan-4n6.hatenablog.com/entry/2018/02/22/131110 https://msdn.microsoft.com/en-us/windows/compatibility/desktop-activity-moderator";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var keyValue in key.Values)
            {
                if (keyValue.ValueName == "Version" || keyValue.ValueName == "SequenceNumber")
                {
                    continue;
                }
                try
                {
                    var ts = BitConverter.ToInt64(keyValue.ValueDataRaw, 0);
                    var execTime = DateTimeOffset.FromFileTime(ts).ToUniversalTime();

                    var vo = new ValuesOut(keyValue.ValueName,execTime);

                    _values.Add(vo);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Value name: {keyValue.ValueName}, message: {ex.Message}");
                }
            }
        }


        public IBindingList Values => _values;
    }
}

