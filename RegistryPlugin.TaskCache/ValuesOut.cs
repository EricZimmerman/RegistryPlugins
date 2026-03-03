using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TaskCache
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(int version, string keyName, DateTimeOffset createdOn, DateTimeOffset? lastStart, DateTimeOffset? lastStop, int taskState, int lastActionResult, string source, string description, string secdesc, string author, string path, string command, string arguments)
        {
            Version = version;
            KeyName = keyName;
            CreatedOn = createdOn;
            LastStart = lastStart;
            LastStop = lastStop;
            TaskState = taskState;
            LastActionResult = lastActionResult;
            Source = source;
            Description = description;
            SecurityDescriptor = secdesc;
            Author = author;
            Path = path;
            Command = command;
            Arguments = arguments;
        }

        public int Version { get; }
        public string KeyName { get; }
        public string Path { get; }
        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset? LastStart { get; }
        public DateTimeOffset? LastStop { get; }
        public int TaskState { get; }
        public int LastActionResult { get; }

        public string Source { get; }

        public string Description { get; }
        public string Command { get; }
        public string Arguments { get; }

        public string SecurityDescriptor { get; }

        public string Author { get; }

      

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Created on: {CreatedOn.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData2 => $"Last start: {LastStart?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}, Last stop: {LastStop?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Path: {Path}, Command: {Command}, Arguments: {Arguments}";

        public override string ToString()
        {
            return $"{BatchValueData1} {BatchValueData2} {BatchValueData3}";
        }
    }
}
