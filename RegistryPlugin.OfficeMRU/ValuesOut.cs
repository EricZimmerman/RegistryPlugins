using System;

namespace RegistryPlugin.OfficeMRU
{
    public class ValuesOut
    {
        public ValuesOut(string valueName, DateTimeOffset lastOpened, DateTimeOffset? lastClosed, string fileName)
        {
            ValueName = valueName;
            LastClosed = lastClosed?.UtcDateTime;
            LastOpened = lastOpened.DateTime;
            FileName = fileName;
        }

        public string ValueName { get; }
        public DateTime LastOpened { get; }
        public DateTime? LastClosed { get; }
        public string FileName { get; }
    }
}