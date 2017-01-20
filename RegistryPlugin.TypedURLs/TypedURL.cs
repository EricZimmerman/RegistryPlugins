using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.TypedURLs
{
    public class TypedURL
    {
		public string Url { get; }
		public DateTimeOffset? Timestamp { get; }

        public TypedURL(string url, DateTimeOffset? timestamp)
        {
            Url = url;
            Timestamp = timestamp;
        }
    }
}
