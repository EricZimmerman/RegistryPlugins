using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Ares
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string propName, string propValue)
        {
            PropertyName = propName;
            PropertyValue = propValue;
        }

        public string PropertyName { get; }
        public string PropertyValue { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Name: {PropertyName}";
        public string BatchValueData2 => $"Value: {PropertyValue}";
        public string BatchValueData3 { get; }
    }
}