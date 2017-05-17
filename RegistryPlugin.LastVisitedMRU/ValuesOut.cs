using System;

namespace RegistryPlugin.LastVisitedMRU
{
    public class ValuesOut
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
    }
}