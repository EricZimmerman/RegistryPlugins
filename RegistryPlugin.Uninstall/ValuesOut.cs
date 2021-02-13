using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Uninstall
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string keyName, string displayName, string displayVersion, string publisher, string installDate, string installSource, string installLocation, string uninstallString, DateTimeOffset? timestamp)
        {
            KeyName = keyName;
            DisplayName = displayName;
            DisplayVersion = displayVersion;
            Publisher = publisher;
            InstallDate = installDate;
            InstallSource = installSource;
            InstallLocation = installLocation;
            UninstallString = uninstallString;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string KeyName { get;  }
        public string DisplayName { get; }
        public string DisplayVersion { get; }
        public string Publisher { get; }
        public string InstallDate { get; }
        public string InstallSource { get; }
        public string InstallLocation { get; }
        public string UninstallString { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"KeyName: {KeyName} DisplayName: {DisplayName} DisplayVersion: {DisplayVersion} Publisher: {Publisher} InstallDate: {InstallDate}";
        public string BatchValueData2 => $"InstallSource: {InstallSource} InstallLocation: {InstallLocation} UninstallString: {UninstallString}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
