using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.WindowsApp
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string keyname, string displayname, DateTimeOffset? installtime, string packagerootfolder)
        {
            KeyName = keyname;
            DisplayName = displayname;
            InstallTime = installtime;
            PackageRootFolder = packagerootfolder;
        }

        public string KeyName { get; }
        public string DisplayName { get; }
        public DateTimeOffset? InstallTime { get; }
        public string PackageRootFolder { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"KeyName: {KeyName} DisplayName: {DisplayName} PackageRootFolder: {PackageRootFolder}";
        public string BatchValueData2 => $"InstallTime: {InstallTime?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => string.Empty;
    }
}