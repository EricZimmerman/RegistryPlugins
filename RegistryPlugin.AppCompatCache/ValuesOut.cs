using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppCompatCache
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(int cacheEntryPosition, string programName, DateTimeOffset modDate)
        {
            CacheEntryPosition = cacheEntryPosition;
            ProgramName = programName;
            ModifiedTime = modDate.UtcDateTime;
        }

        public int CacheEntryPosition { get; }
        public string ProgramName { get; }
        public DateTime ModifiedTime { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"{ProgramName}";
        public string BatchValueData2 => $"Modified: {ModifiedTime.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Position: {CacheEntryPosition}";
    }
}
