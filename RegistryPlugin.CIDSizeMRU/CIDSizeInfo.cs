using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.CIDSizeMRU
{
    public class CIDSizeInfo:IValueOut
    {
        public CIDSizeInfo(string exeName, int mruPos, DateTimeOffset? openedOn)
        {
            Executable = exeName;
            MRUPosition = mruPos;
            OpenedOn = openedOn?.UtcDateTime;
        }

        public string Executable { get; }
        public int MRUPosition { get; }

        public DateTime? OpenedOn { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"{Executable}";
        public string BatchValueData2=> $"Opened: {OpenedOn?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff})";
        public string BatchValueData3 => $"Mru: {MRUPosition}";
    }
}