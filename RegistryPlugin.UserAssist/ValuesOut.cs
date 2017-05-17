using System;

namespace RegistryPlugin.UserAssist
{
    public class ValuesOut
    {
        public ValuesOut(string valueName, string programName, int runCounter, DateTimeOffset? lastRun)
        {
            ValueName = valueName;
            ProgramName = programName;
            RunCounter = runCounter;
            LastExecuted = lastRun?.UtcDateTime;
        }

        public string ValueName { get; }
        public string ProgramName { get; }
        public int RunCounter { get; }
        public DateTime? LastExecuted { get; }
    }
}