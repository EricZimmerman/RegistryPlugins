using System;

namespace RegistryPlugin.TypedURLs
{
    public class TypedURL
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
    }
}