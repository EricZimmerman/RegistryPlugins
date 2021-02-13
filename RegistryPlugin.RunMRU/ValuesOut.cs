using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.RunMRU
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string valueName, string executable, int mruPosition, DateTimeOffset? openedOn)
        {
            Executable = executable;
            ValueName = valueName;
            MruPosition = mruPosition;
            OpenedOn = openedOn?.UtcDateTime;
        }

        public string ValueName { get; }
        public int MruPosition { get; }

        public string Executable { get; }

        public DateTime? OpenedOn { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Executable: {Executable}";
        public string BatchValueData2  => $"Opened on: {OpenedOn?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"MRU: {MruPosition}";
    }
}