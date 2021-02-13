using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.BamDam
{
   public class ValuesOut:IValueOut
    {
        public ValuesOut(string program, DateTimeOffset executionTime)
        {
            Program = program;
            ExecutionTime = executionTime;
        }
        public string Program { get; }
        public DateTimeOffset ExecutionTime { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Program: {Program}";
        public string BatchValueData2 => $"Execution time: {ExecutionTime.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff})";
        public string BatchValueData3 { get; }
    }
}
