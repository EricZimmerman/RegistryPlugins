using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.OfficeMRU
{
    public class OfficeMRU : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public OfficeMRU()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "97e13511-2789-4cf3-9ab3-9e0115b41dc2";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Office\15.0\Word\User MRU\*\File MRU",
            @"Software\Microsoft\Office\15.0\Excel\User MRU\*\File MRU",
            @"Software\Microsoft\Office\15.0\PowerPoint\User MRU\*\File MRU",
            @"Software\Microsoft\Office\15.0\Access\User MRU\*\File MRU",
            @"Software\Microsoft\Office\15.0\OneNote\User MRU\*\File MRU",
            @"Software\Microsoft\Office\16.0\Word\User MRU\*\File MRU",
            @"Software\Microsoft\Office\16.0\Excel\User MRU\*\File MRU",
            @"Software\Microsoft\Office\16.0\PowerPoint\User MRU\*\File MRU",
            @"Software\Microsoft\Office\16.0\Access\User MRU\*\File MRU",
            @"Software\Microsoft\Office\16.0\OneNote\User MRU\*\File MRU"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Office MRU";

        public string ShortDescription =>
            "Extracts recent Office document names and last opened/closed times";

        public string LongDescription => ShortDescription;

        public double Version => 0.5;
        public List<string> Errors { get; }


        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {
                foreach (var keyValue in key.Values)
                {
                    //[F00000000][T01D005C5B44B6300][O00000000]*C:\Users\eric\Desktop\aa\Out\Deduplicated.tsv

                    var segs = keyValue.ValueData.Split('*');
                    var fName = segs.Last();

                    var segs2 = segs.First().Split('[');
                    //"T01D005C5B44B6300]"

                    var rawTime = segs2[2];
                    rawTime = rawTime.Substring(1);
                    rawTime = rawTime.Substring(0, rawTime.Length - 1);
                    var time = Convert.ToInt64(rawTime, 16);

                    var firstOpen = DateTimeOffset.FromFileTime(time);

                    //  @"Software\Microsoft\Office\15.0\Word\User MRU\*\File MRU", //Software\Microsoft\Office\15.0\Word\Reading Locations
                    //Software\Microsoft\Office\15.0\Word\Reading Locations
                    //Value Name	Value Type	Data
                    //File Path   RegSz C:\ProjectWorkingFolder\GOON2\GOON2\GOON2Manual.docx  

                    //jump up a few levels and check for Reading Locations
                    var readingLocKey =
                        key.Parent.Parent.Parent.SubKeys.SingleOrDefault(t => t.KeyName == "Reading Locations");

                    DateTimeOffset? lastOpen = null;

                    if (readingLocKey != null)
                    {
                        foreach (var registryKey in readingLocKey.SubKeys)
                        {
                            var readingLocVal = registryKey.Values.SingleOrDefault(t => t.ValueName == "File Path");

                            if (readingLocVal != null)
                            {
                                if (readingLocVal.ValueData == fName)
                                {
                                    lastOpen = registryKey.LastWriteTime;
                                    break;
                                }
                            }
                        }
                    }

                    var v = new ValuesOut(keyValue.ValueName, firstOpen, lastOpen, fName);

                    _values.Add(v);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing MRU key: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}