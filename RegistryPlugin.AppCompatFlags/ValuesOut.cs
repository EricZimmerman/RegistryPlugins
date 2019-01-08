using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppCompatFlags
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string exePath,string valueName)
        {
            Executable = exePath;
            ValueName = valueName;
        }

        public string Executable { get; set; }
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"{Executable}";
        public string BatchValueData2 => string.Empty;
        public string BatchValueData3 => string.Empty;
    }
}
