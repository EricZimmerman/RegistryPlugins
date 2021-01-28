using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.VolumeInfoCache
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string drivename, string volumelabel, DateTimeOffset? timestamp)
        {
            DriveName = drivename;
            VolumeLabel = volumelabel;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string DriveName { get; }
        public string VolumeLabel { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"DriveName: {DriveName} VolumeLabel: {VolumeLabel}";
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} ";
        public string BatchValueData3 => string.Empty;
    }
}
