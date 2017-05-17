using System;

namespace RegistryPlugin.RunMRU
{
    public class ValuesOut
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
    }
}