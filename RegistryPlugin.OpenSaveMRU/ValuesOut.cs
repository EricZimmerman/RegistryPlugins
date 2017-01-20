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
            OpenedOn = openedOn;
        }

        public string Extension { get; }
        public string ValueName { get; }
        public int MruPosition { get; }

        public string Filename { get; }

        public DateTimeOffset? OpenedOn { get; }
    }
}