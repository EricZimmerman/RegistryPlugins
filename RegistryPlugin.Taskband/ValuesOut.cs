using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Taskband
{
  public  class ValuesOut:IValueOut
    {
        public ValuesOut(string lnkName, string executable, string pinType)
        {
            LnkName = lnkName;
            Executable = executable;
            PinType = pinType;
        }

        public string LnkName { get; }
        public string Executable { get; }
        public string PinType { get; }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Lnk Name: {LnkName}";
        public string BatchValueData2 => $"Executable: {Executable}";
        public string BatchValueData3 => $"Pin Type: {PinType}";

        public override string ToString()
        {
            return $"{BatchValueData1} {BatchValueData2} {BatchValueData3}";
        }
    }
}
