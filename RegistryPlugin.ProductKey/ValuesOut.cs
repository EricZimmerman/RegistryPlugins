using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.ProductKey
{
   public class ValuesOut:IValueOut
    {

        public ValuesOut(string productKey)
        {
            ProductKey = productKey;
        }
        public string ProductKey { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Product Key: {ProductKey}";
        public string BatchValueData2 { get; }
        public string BatchValueData3 { get; }
    }
}
