using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.JumplistData
{
    public class JumplistData : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public JumplistData()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "1fed25en-7302-48fb-92ad-30f144731fa1";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Search\JumplistData"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "JumplistData";

        public string ShortDescription
            => "Displays program execution and execution time related to jump lists";

        public string LongDescription
            =>
                string.Empty;

        public double Version => 0.5;
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
            
            try
            {

                foreach (var keyValue in key.Values)
                {


                    var ff = new ValuesOut(keyValue.ValueName,
                        DateTimeOffset.FromFileTime(BitConverter.ToInt64(keyValue.ValueDataRaw, 0))
                            .ToUniversalTime()) {BatchKeyPath = key.KeyPath, BatchValueName = keyValue.ValueName};

                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing JumplistData key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}