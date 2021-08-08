using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.AppCompatFlags2
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string path)
        {
            Path = path;
        }

        public string Path { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Path: {Path}";
        public string BatchValueData2 => $"";
        public string BatchValueData3 => $"";
    }
}