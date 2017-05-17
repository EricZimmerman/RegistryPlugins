using System;

namespace RegistryPlugin.LastVisitedPidlMRU
{
    public class ValuesOut
    {
        public ValuesOut(string ext, string absolutePath, string details, string valueName, int mruPosition,
            DateTimeOffset? openedOn)
        {
            Executable = ext;
            AbsolutePath = absolutePath;
            Details = details;
            ValueName = valueName;
            MruPosition = mruPosition;
            OpenedOn = openedOn?.UtcDateTime;
        }

        public string ValueName { get; }
        public int MruPosition { get; }

        public string Executable { get; }
        public string AbsolutePath { get; }

        public DateTime? OpenedOn { get; }
        public string Details { get; }
    }
}