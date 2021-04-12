using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.OpenSaveMRU
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string ext, string filename, string valueName, int mruPosition, DateTimeOffset? openedOn)
        {
            Extension = ext;
            Filename = filename;

            ValueName = valueName;
            MruPosition = mruPosition;
            OpenedOn = openedOn?.UtcDateTime;
        }

        public string Extension { get; }
        public string ValueName { get; }
        public int MruPosition { get; }

        public string Filename { get; }

        public DateTime? OpenedOn { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"File name: {Filename}";
        public string BatchValueData2 => $"Opened: {OpenedOn?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3  => $"MRU: {MruPosition}";
    }
}
