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

        public double Version => 0.1;
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
            var iconlayouts = key.Values.SingleOrDefault(t => t.ValueName == "IconLayouts").ValueDataRaw;
            if (iconlayouts != null)
            {
                var br = new BinaryReader(new MemoryStream(iconlayouts));
                br.BaseStream.Position = 0x18;

                byte[] countBytes = br.ReadBytes(2);
                var count = BitConverter.ToInt16(countBytes, 0);
                br.BaseStream.Position += 0x6;

                for (var i  = 0; i < count; i++)
                {
                    byte[] lengthBytes = br.ReadBytes(2);
                    var length = BitConverter.ToInt16(lengthBytes, 0);
                    br.BaseStream.Position += 0x6;
                    var name = Encoding.Unicode.GetString(br.ReadBytes(length * 2));

                    var ff = new ValuesOut(name)
                    {
                        BatchValueName = "Multiple",
                        BatchKeyPath = key.KeyPath
                    };
                    l.Add(ff);
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
