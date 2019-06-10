using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TerminalServerClient
{
    public class TerminalServerClient : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public TerminalServerClient()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "d28d6fb0-2ef9-47fd-8fa9-128454b5aa1e";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Terminal Server Client"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "TerminalServerClient";
        public string ShortDescription => "Displays RDP connection history and username information";

        public string LongDescription
            =>
                "";

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
                var defaultKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Default");

                if (defaultKey == null)
                {
                    return l;
                }

                var mruOrder = new Dictionary<string, int>();

                foreach (var defaultKeyValue in defaultKey.Values)
                {
                    var pos = defaultKeyValue.ValueName.Remove(0, 3);
                    var name = defaultKeyValue.ValueData;

                    mruOrder.Add(name, int.Parse(pos));
                }


                var serversKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Servers");

                if (serversKey == null)
                {
                    return l;
                }

                foreach (var serverKey in serversKey.SubKeys)
                {
                    var host = serverKey.KeyName;

                    var mru = -1;
                    if (mruOrder.ContainsKey(host))
                    {
                        mru = mruOrder[host];
                    }

                    var hintVal = serverKey.Values.SingleOrDefault(t => t.ValueName == "UsernameHint");

                    var hint = "<None>";

                    if (hintVal != null)
                    {
                        hint = hintVal.ValueData;
                    }

               

                    var ff = new ValuesOut(mru, host, hint, serverKey.LastWriteTime.Value);
                    ff.BatchKeyPath = key.KeyPath;
                    ff.BatchValueName = hintVal?.ValueName;

                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Terminal Server Client key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}