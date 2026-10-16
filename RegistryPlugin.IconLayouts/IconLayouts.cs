using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.IconLayouts
{
    public class IconLayouts : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public IconLayouts()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "4a30e585-cb91-40d1-b8b6-7bcbe250ab4a";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\Shell\Bags\1\Desktop"
        });
        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "IconLayouts";

        public string ShortDescription
            => "https://github.com/kacos2000/Win10/blob/master/Desktop_IconLayouts.pdf";

        public string LongDescription => ShortDescription;

        public double Version => 0.2;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();
            AlertMessage = null;

            foreach (var rd in ProcessKey(key))
            {
                _values.Add(rd);
            }
        }

        public IBindingList Values => _values;

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();
            var iconlayouts = key.Values.FirstOrDefault(t => t.ValueName == "IconLayouts")?.ValueDataRaw;
            if (iconlayouts != null)
            {
                try
                {
                    var br = new BinaryReader(new MemoryStream(iconlayouts));
                    br.BaseStream.Position = 0x18;

                    var count = br.ReadInt16();
                    br.BaseStream.Position += 0x6;

                    for (var i = 0; i < count; i++)
                    {
                        var length = br.ReadInt16();
                        br.BaseStream.Position += 0x6;
                        if (length < 0 || br.BaseStream.Position + length * 2 > br.BaseStream.Length)
                        {
                            Errors.Add($"Entry {i}: name length {length} is past the end of the value");
                            break;
                        }

                        var name = Encoding.Unicode.GetString(br.ReadBytes(length * 2));

                        var ff = new ValuesOut(name)
                        {
                            BatchValueName = "Multiple",
                            BatchKeyPath = key.KeyPath
                        };
                        l.Add(ff);
                    }
                }
                catch (EndOfStreamException)
                {
                    Errors.Add("The IconLayouts value is truncated");
                }
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}
