using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin._7_ZipHistory
{
    public class SevenZip : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public SevenZip()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "6b1296a2-d3fb-441f-89c1-fd3706855acc";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\7-Zip\Compression"
        });

        public string ValueName => "ArcHistory";
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "7-Zip archive history";

        public string ShortDescription =>
            "Extracts archive history from ArcHistory key"
            ;

        public string LongDescription => ShortDescription;

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {
                var arcHist = key.Values.SingleOrDefault(t => t.ValueName == "ArcHistory");

                if (arcHist != null)
                {
                    var arcs = Encoding.Unicode.GetString(arcHist.ValueDataRaw).Split('\0');

                    foreach (var arc in arcs)
                    {
                        if (arc.Trim().Length == 0)
                        {
                            continue;
                        }
                        var v = new ValuesOut(arc);
                        Values.Add(v);
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing 7-Zip archive history: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}