using System;

namespace RegistryPlugin.OpenSaveMRU
{
    public class ValuesOut
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
    }
}