using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TimeZoneInformation
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string valueName, string valueData, string valueDataRaw)
        {
           
            ValueName = valueName;
            ValueData = valueData;
            ValueDataRaw = valueDataRaw;
        }

     
        public string ValueName { get; }
        public string ValueData { get; }
        public string ValueDataRaw { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => ValueData;
        public string BatchValueData2 => ValueDataRaw;
        public string BatchValueData3 =>  string.Empty;
    }
}