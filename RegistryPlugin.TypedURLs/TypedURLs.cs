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

namespace RegistryPlugin.TypedURLs
{
    public class TypedURLs : IRegistryPluginGrid
    {
        private readonly BindingList<TypedURL> _values;

        public TypedURLs()
        {
            _values = new BindingList<TypedURL>();

            Errors = new List<string>();
        }

        public string InternalGuid => "d28d6fb0-2ef9-47fd-8fa9-128354b5aa1e";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Internet Explorer\TypedURLs"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "TypedURLs";
        public string ShortDescription => "Displays URLs typed on the keyboard and corresponding timestamps";

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


        private IEnumerable<TypedURL> ProcessKey(RegistryKey key)
        {
            var l = new List<TypedURL>();

            try
            {
                var typedTimes = key.Parent.SubKeys.SingleOrDefault(t => t.KeyName == "TypedURLsTime");

                foreach (var keyValue in key.Values)
                {
                    var url = keyValue.ValueData;

                    DateTimeOffset? ts = null;

                    var tsRaw = typedTimes?.Values.SingleOrDefault(t => t.ValueName == keyValue.ValueName);

                    if (tsRaw != null)
                    {
                        ts = DateTimeOffset.FromFileTime(BitConverter.ToInt64(tsRaw.ValueDataRaw,0)).ToUniversalTime();
                    }



                    var ff = new TypedURL(url, ts);

                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing TypedURLs key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}