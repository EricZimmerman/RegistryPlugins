using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.OpenSavePidlMRU
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string ext, string absolutePath, string details, string valueName, int mruPosition,
            DateTimeOffset? openedOn)
        {
            Extension = ext;
            AbsolutePath = absolutePath;
            Details = details;
            ValueName = valueName;
            MruPosition = mruPosition;
            OpenedOn = openedOn?.UtcDateTime;
        }

        public string Extension { get; }
        public string ValueName { get; }
        public int MruPosition { get; }
        public string AbsolutePath { get; }
        public DateTime? OpenedOn { get; }
        public string Details { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Extension: {Extension} Absolute path: {AbsolutePath}";
        public string BatchValueData2 => $"Opened: {OpenedOn?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff})";
        public string BatchValueData3 => $"MRU: {MruPosition} Details: {Details}" ;
    }
}