using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppCompatCache
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(int cacheEntryPosition, string programName, DateTimeOffset? modDate, string executed, bool fromSlack)
        {
            CacheEntryPosition = cacheEntryPosition;
            ProgramName = programName;
            ModifiedTime = modDate?.UtcDateTime;
            Executed = executed;
            FromSlack = fromSlack;
        }

        public int CacheEntryPosition { get; }
        public string ProgramName { get; }
        public DateTime? ModifiedTime { get; }

        /// <summary>Yes, No, or NA where the format has no such flag (see AppCompatCacheParser)</summary>
        public string Executed { get; }

        /// <summary>Recovered from the slack of the value's cell: no longer in the cache</summary>
        public bool FromSlack { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"{ProgramName}";
        public string BatchValueData2 => $"Modified: {ModifiedTime?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Position: {CacheEntryPosition}, Executed: {Executed}{(FromSlack ? ", from slack" : "")}";
    }
}
