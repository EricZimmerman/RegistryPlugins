using System;

namespace RegistryPlugin.UserAssist
{
    public class ValuesOut
    {
        public ValuesOut(string valueName, string programName, int runCounter, DateTimeOffset? lastRun, int? focusCount, int? focusMilliseconds)
        {
            ValueName = valueName;
            ProgramName = programName;
            RunCounter = runCounter;
            LastExecuted = lastRun?.UtcDateTime;
            FocusCount = focusCount;
            FocusMilliseconds = focusMilliseconds;
        }

        public string ValueName { get; }
        public string ProgramName { get; }
        public int RunCounter { get; }
        public int? FocusCount { get; }
        public int? FocusMilliseconds { get; }
        public DateTime? LastExecuted { get; }
    }
}