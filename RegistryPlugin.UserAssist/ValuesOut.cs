using System;
using RegistryPluginBase.Interfaces;


namespace RegistryPlugin.UserAssist
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string valueName, string programName, int runCounter, DateTimeOffset? lastRun, int? focusCount, string focusTime)
        {
            BatchValueName = valueName;
            ProgramName = programName;
            RunCounter = runCounter;
            LastExecuted = lastRun?.UtcDateTime;
            FocusCount = focusCount;
            FocusTime = focusTime;
        }

        
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string ProgramName { get; }
        public int RunCounter { get; }
        public int? FocusCount { get; }
        public string FocusTime{ get; }
        public DateTime? LastExecuted { get; }

        public string BatchValueData1 => $"{ProgramName}";
        public string BatchValueData2 => $"Last executed: {LastExecuted?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Run count: {RunCounter:N0}";
    }
}