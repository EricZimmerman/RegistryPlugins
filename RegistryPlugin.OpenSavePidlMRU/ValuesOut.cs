using System;

namespace RegistryPlugin.OpenSavePidlMRU
{
    public class ValuesOut
    {
        public ValuesOut(string ext, string absolutePath, string details, string valueName, int mruPosition,
            DateTimeOffset? openedOn)
        {
            Extension = ext;
            AbsolutePath = absolutePath;
            Details = details;
            ValueName = valueName;
            MruPosition = mruPosition;
            OpenedOn = openedOn;
        }

        public string Extension { get; }
        public string ValueName { get; }
        public int MruPosition { get; }
        public string AbsolutePath { get; }
        public DateTimeOffset? OpenedOn { get; }
        public string Details { get; }
    }
}