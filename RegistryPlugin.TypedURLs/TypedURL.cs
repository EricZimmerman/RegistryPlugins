using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.TypedURLs
{
    public class TypedURL
    {
        public DateTimeOffset? Timestamp { get; }

        public string Url { get; }
		public string Slack { get; }
		

        public TypedURL(string url, DateTimeOffset? timestamp, string slack)
        {
            Url = url;
            Timestamp = timestamp;
            Slack = slack;
        }
    }
}
