using System;

namespace RegistryPlugin.CIDSizeMRU
{
    public class CIDSizeInfo
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
    }
}