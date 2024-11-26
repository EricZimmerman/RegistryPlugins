using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Mare
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string unknown, string path)
        {
            Unknown = unknown;
            Path = path;
        }

        public string Unknown { get; }
        public string Path { get; }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Unknown: {Unknown}";
        public string BatchValueData2 => $"Path: {Path}";
        public string BatchValueData3 => $"";
    }
}
