using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.WordWheelQuery
{
 public   class WordWheelQuery : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public WordWheelQuery()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "5cca2b62-a5dd-4fa7-9ccc-1b3bdec726b0";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\WordWheelQuery"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "WordWheelQuery";
        public string ShortDescription => "Displays recent searches";

        public string LongDescription
            =>
            ""
            ;

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
                var mruList = key.Values.Single(t => t.ValueName == "MRUListEx");

                var mruPositions = new Dictionary<uint, int>();

                var i = 0;

                var index = 0;

                var mruPos = BitConverter.ToUInt32(mruList.ValueDataRaw, index);
                index += 4;

                while (mruPos != 0xFFFFFFFF)
                {
                    mruPositions.Add(mruPos, i);
                    i++;
                    mruPos = BitConverter.ToUInt32(mruList.ValueDataRaw, index);
                    index += 4;
                }

                //mruPositions now contains a map of positions (the key) to the order it was opened (the value)

                foreach (var keyValue in key.Values)
                {
                    if (keyValue.ValueName == "MRUListEx")
                    {
                        continue;
                    }

                    var mru = mruPositions[uint.Parse(keyValue.ValueName)];

                    var st = Encoding.Unicode.GetString(keyValue.ValueDataRaw).Trim('\0');

                    var ff = new ValuesOut(st,mru);

                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing WordWheelQuery key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l.OrderBy(t => t.MruPosition);
        }
    }
}
