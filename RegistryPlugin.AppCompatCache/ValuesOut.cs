using System;

namespace RegistryPlugin.AppCompatCache
{
    public class ValuesOut
    {
        public ValuesOut(int cacheEntryPosition, string programName, DateTimeOffset modDate)
        {
            CacheEntryPosition = cacheEntryPosition;
            ProgramName = programName;
            ModifiedTime = modDate;
        }

        public int CacheEntryPosition { get; }
        public string ProgramName { get; }
        public DateTimeOffset ModifiedTime { get; }
    }
}