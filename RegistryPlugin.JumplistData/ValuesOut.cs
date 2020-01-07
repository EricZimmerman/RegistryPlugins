using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.JumplistData
{
   public class ValuesOut:IValueOut
    {
        public ValuesOut(string jumpListName, DateTimeOffset executedOn)
        {
            JumpListName = jumpListName;
            ExecutedOn = executedOn;
        }

        public string JumpListName { get; set; }
        public DateTimeOffset ExecutedOn { get; set; }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Name: {JumpListName}";
        public string BatchValueData2 => $"Executed: {ExecutedOn.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => "";

        public override string ToString()
        {
            return $"{JumpListName} ==> {ExecutedOn.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        }
    }
}
