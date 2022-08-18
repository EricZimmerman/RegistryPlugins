using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Products
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string displayname, string packagename, string installdate, string publisher, string language, string installlocation, string installsource, string comments, DateTimeOffset? timestamp)
        {
            DisplayName = displayname;
            PackageName = packagename;
            InstallDate = installdate;
            Publisher = publisher;
            Language = language;
            InstallLocation = installlocation;
            InstallSource = installsource;
            Comments = comments;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string DisplayName { get; }
        public string PackageName { get; }
        public string InstallDate { get; }
        public string Publisher { get; }
        public string Language { get; }
        public string InstallLocation { get; }
        public string InstallSource { get; }
        public string Comments { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"DisplayName: {DisplayName} PackageName: {PackageName} InstallDate: {InstallDate} Publisher: {Publisher} Language: {Language}";
        public string BatchValueData2 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"InstallLocation: {InstallLocation} InstallSource: {InstallSource} Comments: {Comments}";
    }
}
