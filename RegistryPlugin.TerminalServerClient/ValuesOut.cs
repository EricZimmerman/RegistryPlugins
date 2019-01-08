using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TerminalServerClient
{
    public class ValuesOut:IValueOut
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
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => HostName;
        public string BatchValueData2 => $"User: {Username}, Mru: {MRUPosition}";
        public string BatchValueData3 => $"Last modified: {LastModified.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} ";
    }
}