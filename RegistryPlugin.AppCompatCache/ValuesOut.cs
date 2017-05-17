using System;

namespace RegistryPlugin.AppCompatCache
{
    public class ValuesOut
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
    }
}