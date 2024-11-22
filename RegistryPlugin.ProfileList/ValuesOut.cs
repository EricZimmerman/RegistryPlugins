using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.ProfileList
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string keyName, string profileimagepath, DateTimeOffset? timestamp, DateTimeOffset? lastlogontime, DateTimeOffset? lastlogofftime)
        {
            KeyName = keyName;
            ProfileImagePath = profileimagepath;
            Timestamp = timestamp;
            LastLogonTime = lastlogontime;
            LastLogoffTime = lastlogofftime;
        }

        public DateTimeOffset? Timestamp { get; }

        public string KeyName { get; }
        public string ProfileImagePath { get; }
        public DateTimeOffset? LastLogonTime { get; }
        public DateTimeOffset? LastLogoffTime { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"KeyName: {KeyName}";
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"ProfileImagePath: {ProfileImagePath}";
    }
}
