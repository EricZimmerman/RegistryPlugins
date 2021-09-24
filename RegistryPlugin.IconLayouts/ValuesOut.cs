using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.IconLayouts
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Name: {Name}";
        public string BatchValueData2 => $"";
        public string BatchValueData3 => $"";
    }
}
