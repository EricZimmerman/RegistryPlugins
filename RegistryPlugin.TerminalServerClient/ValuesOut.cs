using System;

namespace RegistryPlugin.TerminalServerClient
{
    public class ValuesOut
    {
        public ValuesOut(int mru, string host, string user, DateTimeOffset lastmod)
        {
            MRUPosition = mru;
            HostName = host;
            Username = user;
            LastModified = lastmod.UtcDateTime;
        }

        public string HostName { get; }
        public string Username { get; }

        public int MRUPosition { get; }

        public DateTime LastModified { get; }
    }
}