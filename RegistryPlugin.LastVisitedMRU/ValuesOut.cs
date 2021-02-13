using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.LastVisitedMRU
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string valueName, string executable, string directory, int mruPosition,
            DateTimeOffset? openedOn)
        {
            Executable = executable;
            Directory = directory;
            ValueName = valueName;
            MruPosition = mruPosition;
            OpenedOn = openedOn?.UtcDateTime;
        }

        public string ValueName { get; }
        public int MruPosition { get; }
        public string Executable { get; }
        public string Directory { get; }
        public DateTime? OpenedOn { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Exe: {Executable} Folder: {Executable} Directory: {Directory}";
        public string BatchValueData2 => $"Opened: {OpenedOn?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff})";
        public string BatchValueData3 => $"MRU: {MruPosition}";
    }
}