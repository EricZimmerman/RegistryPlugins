using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugins.RADAR
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string keyname, string filename, DateTimeOffset? lastdetectiontime)
        {
            KeyName = keyname;
            Filename = filename;
            LastDetectionTime = lastdetectiontime;
        }

        public string KeyName { get; }
        public string Filename { get; }
        public DateTimeOffset? LastDectectionTime { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Filename: {Filename}";
        public string BatchValueData2 => $"LastDetectionTime: {LastDetectionTime?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"KeyName: {KeyName}";
    }
}
