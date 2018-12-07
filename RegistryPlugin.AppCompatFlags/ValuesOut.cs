using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.AppCompatFlags
{
    public class ValuesOut
    {
        public ValuesOut(string exePath)
        {
            Executable = exePath;
        }

        public string Executable { get; set; }
    }
}
