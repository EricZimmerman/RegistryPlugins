using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscUtils.Compression;
using DiscUtils.Ntfs;
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

        public string ValueName => "AppCompatCache";
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

        public double Version => 0.5;
        public List<string> Errors { get; }

        private static object CreateInstance<T>(string name)
        {

            return typeof(T).Assembly.CreateInstance(name);

        }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {

                var instance = CreateInstance<NtfsFileSystem>("DiscUtils.Ntfs.LZNT1");
                var compressor = (BlockCompressor)instance;

                var dataLen = 100;

                
                //Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\CIT\System
               

               foreach (var keyValue in key.Values)
               {
                   var compSize = BitConverter.ToInt32(keyValue.ValueDataRaw, 0);
                   var decompSize = BitConverter.ToInt32(keyValue.ValueDataRaw, 4);


                   byte[] decompressed = new byte[decompSize];
                   var numDecompressed = compressor.Decompress(keyValue.ValueDataRaw, 8, compSize, decompressed, 0);

                   File.WriteAllBytes($"C:\\temp\\{keyValue.ValueName}_{Guid.NewGuid()}.bin",decompressed);
   
               }





//                foreach (var c in cache.Caches)
//                {
//                    foreach (var cacheEntry in c.Entries)
//                    {
//                        try
//                        {
//                            var vo = new ValuesOut("Foo");
//
//                            _values.Add(vo);
//                        }
//                        catch (Exception ex)
//                        {
//                            Errors.Add($"Value name: {cacheEntry.CacheEntryPosition}, message: {ex.Message}");
//                        }
//                    }
//                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing AppCompatCache: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}
