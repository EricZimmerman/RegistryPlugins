using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppCompatFlags
{
    public class AppCompatFlags: IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public AppCompatFlags()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "f2afe57b-5423-2f1c-3bd9-acca9434dd2d";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\Windows NT\CurrentVersion\AppCompatFlags\CIT\System"
            
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "AppCompatFlags";

        public string ShortDescription
            =>
                "Tracks full paths to executables, but its compressed with LZNT1 compression";

        public string LongDescription
            =>
                "Possibly useful to show evidence of execution";

        public double Version => 0.6;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var keyValue in key.Values)
            {
                try
                {
                    var raw = keyValue.ValueDataRaw;
                    if (raw == null || raw.Length < 8)
                    {
                        continue;
                    }

                    //compressed size, decompressed size, LZNT1 data
                    var compSize = BitConverter.ToInt32(raw, 0);
                    var decompSize = BitConverter.ToInt32(raw, 4);
                    if (compSize <= 0 || decompSize <= 0 || decompSize > 0x1000000)
                    {
                        continue;
                    }

                    var decompressed = new byte[decompSize];
                    var numDecompressed = Lznt1.Decompress(raw, 8, compSize, decompressed);

                    var uni = Encoding.Unicode.GetString(decompressed, 0, numDecompressed & ~1);

                    var indexStart = uni.IndexOf("\\DEV", StringComparison.Ordinal);
                    if (indexStart < 0)
                    {
                        continue;
                    }

                    foreach (var str in uni.Substring(indexStart).Split('\0'))
                    {
                        if (str.Length < 8)
                        {
                            continue;
                        }

                        var vo = new ValuesOut(str, keyValue.ValueName)
                        {
                            BatchKeyPath = key.KeyPath,
                            ValueName = keyValue.ValueName
                        };

                        _values.Add(vo);
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing AppCompatFlags value '{keyValue.ValueName}': {ex.Message}");
                }
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}
