using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppPaths
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string filename, string path1, string path2, DateTimeOffset? timestamp)
        {
            FileName = filename;
            Path1 = path1;
            Path2 = path2;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string FileName { get; }
        public string Path1 { get; }
        public string Path2 { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"FileName: {FileName} Path1: {Path1} Path2: {Path2}";
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} ";
        public string BatchValueData3 => string.Empty;
    }
}
