using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TypedURLs
{
    public class TypedURL:IValueOut
    {
        public TypedURL(string url, DateTimeOffset? timestamp, string slack)
        {
            Url = url;
            Timestamp = timestamp;
            Slack = slack;
        }

        public DateTimeOffset? Timestamp { get; }

        public string Url { get; }
        public string Slack { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => Url;
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Slack: {Slack}";
    }
}
