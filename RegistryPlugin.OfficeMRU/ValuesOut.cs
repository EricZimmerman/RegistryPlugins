using System;

namespace RegistryPlugin.OfficeMRU
{
    public class ValuesOut
    {
        public ValuesOut(string valueName, DateTimeOffset firstOpened, DateTimeOffset? lastOpened, string fileName)
        {
            ValueName = valueName;
            LastOpened = lastOpened?.UtcDateTime;
            FirstOpened = firstOpened.DateTime;
            FileName = fileName;
        }

        public string ValueName { get; }
        public DateTime FirstOpened { get; }
        public DateTime? LastOpened { get; }
        public string FileName { get; }
    }
}