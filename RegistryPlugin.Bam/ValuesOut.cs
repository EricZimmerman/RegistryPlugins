using System;

namespace RegistryPlugin.BamDam
{
   public class ValuesOut
    {
        public ValuesOut(string program, DateTimeOffset executionTime)
        {
            Program = program;
            ExecutionTime = executionTime;
        }
        public string Program { get; }
        public DateTimeOffset ExecutionTime { get; }
    }
}
