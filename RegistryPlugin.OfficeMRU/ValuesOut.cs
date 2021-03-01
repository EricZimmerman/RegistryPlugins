using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.OfficeMRU
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string valueName, DateTimeOffset? lastOpened, DateTimeOffset? lastClosed, string fileName)
        {
            ValueName = valueName;
            LastClosed = lastClosed?.UtcDateTime;
            LastOpened = lastOpened?.UtcDateTime;
            FileName = fileName;
        }

        public string ValueName { get; }
        public DateTime? LastOpened { get; }
        public DateTime? LastClosed { get; }
        public string FileName { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"File name: {FileName}";
        public string BatchValueData2 => $"Last opened: {LastOpened?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3  => $"Last closed: {LastClosed?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}