using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Adobe
{
public class Adobe : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public Adobe()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "7dbafd4d-ef39-5e6b-7234-82f5a6f17a4a";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Adobe"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Adobe program information";

        public string ShortDescription =>
            "Decodes various Adobe product information";

        public string LongDescription =>
            "Includes full path, last time file was opened, file name, file size, and page count";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {

                foreach (var productKey in key.SubKeys)
                {
                    foreach (var versionKey in productKey.SubKeys)
                    {
                        var avGeneral = versionKey.SubKeys.SingleOrDefault(t => t.KeyName == "AVGeneral");

                        var recentFiles = avGeneral?.SubKeys.SingleOrDefault(t => t.KeyName == "cRecentFiles");

                        if (recentFiles == null)
                        {
                            continue;
                        }

                        foreach (var recentFilesSubKey in recentFiles.SubKeys)
                        {
                            try
                            {
var fullPath = recentFilesSubKey.Values.SingleOrDefault(t => t.ValueName == "tDIText")
                                ?.ValueData;
                            var lastOpened = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(recentFilesSubKey.Values
                                .SingleOrDefault(t => t.ValueName == "sDate")
                                ?.ValueDataRaw).Trim('\0');
                            var fileName = recentFilesSubKey.Values.SingleOrDefault(t => t.ValueName == "tFileName")
                                ?.ValueData;
                            var fileSizeRaw = recentFilesSubKey.Values.SingleOrDefault(t => t.ValueName == "uFileSize")
                                ?.ValueDataRaw;

                            var fileSize = 0;
                            if (fileSizeRaw != null)
                            {
                                fileSize = BitConverter.ToInt32(fileSizeRaw,0);
                            }
                            var fileSource = recentFilesSubKey.Values.SingleOrDefault(t => t.ValueName == "tFileSource")
                                ?.ValueData;
                            var pageCountRaw = recentFilesSubKey.Values.SingleOrDefault(t => t.ValueName == "uPageCount")
                                ?.ValueDataRaw;

                            var pageCount = 0;
                            if (pageCountRaw != null)
                            {
                                pageCount = BitConverter.ToInt32(pageCountRaw, 0);
                            }

                            var v = new ValuesOut(productKey.KeyName,versionKey.KeyName,fullPath,ConvertToDTO(lastOpened),fileName,fileSize,fileSource,pageCount);

                            v.BatchKeyPath = recentFiles.KeyPath;
                            v.BatchValueName = recentFilesSubKey.KeyName;

                            _values.Add(v);
                            }
                            catch (Exception e)
                            {
                                Errors.Add($"Error processing Adobe product '{versionKey.KeyPath}': {e.Message}");
                            }
                        }


                    }
                }
                
                
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Adobe products: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        private DateTimeOffset ConvertToDTO(string input)
        {
            var start = input.Substring(2);

            var year = start.Substring(0, 4);
            var month = start.Substring(4, 2);
            var day = start.Substring(6, 2);
            var hours = start.Substring(8, 2);
            var mins = start.Substring(10, 2);
            var sec = start.Substring(12, 2);
            var tzi = start.Substring(14);
            tzi = tzi.Replace("'", ":").TrimEnd(':');

            var dateString = $"{month}/{day}/{year}";
            var timeString = $"{hours}:{mins}:{sec}";

           // Console.WriteLine($"{dateString} {timeString} {tzi}");

            if (tzi.EndsWith("Z"))
            {
                tzi = "+0:00";
            }

            var aaa = DateTimeOffset.ParseExact($"{dateString} {timeString} {tzi}","MM/dd/yyyy HH:mm:ss zzz",CultureInfo.InvariantCulture);

            return aaa.ToUniversalTime();

            
        }

        public IBindingList Values => _values;

        private static string DecodeHexToAscii(string hex)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < hex.Length - 1; i += 2)
            {
                var chunk = hex.Substring(i, 2);

                sb.Append(Convert.ToChar(Convert.ToUInt32(chunk, 16)).ToString());
            }

            return sb.ToString();
        }
    }
}
