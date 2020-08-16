using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TrustedDocuments
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string valueName, string tstamp, string fileName,string username)
        {
            EventType = valueName;
            Timestamp = tstamp;
            FileName = fileName;
            Username = username;
        }

        public string EventType { get; }
        public string Timestamp { get; }
        public string FileName { get; }
        public string Username { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"File name: {FileName}";
        public string BatchValueData2 => $"File Opened: {Timestamp})";
        public string BatchValueData3  => $"Event Type: {EventType})";
    }
}