using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.TerminalServerClient
{
   public class ValuesOut
    {

        public ValuesOut(int mru, string host, string user,DateTimeOffset lastmod)
        {
            MRUPosition = mru;
            HostName = host;
            Username = user;
            LastModified = lastmod;

        }
        
        public string HostName {get;}
        public string Username { get; }

        public int MRUPosition { get; }

        public DateTimeOffset LastModified { get; }



    }
}
