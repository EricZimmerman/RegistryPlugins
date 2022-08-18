using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.RecentDocs
{
    public class RecentDoc:IValueOut
    {
        public RecentDoc(int mruPos, string valueName, string targetName, string lnkName, string extension,
            DateTimeOffset? openedOn, DateTimeOffset? extensionLastOpened)
        {
            LnkName = lnkName;
            MruPosition = mruPos;
            ValueName = valueName;
            TargetName = targetName;
            Extension = extension;
            OpenedOn = openedOn?.UtcDateTime;
            ExtensionLastOpened = extensionLastOpened?.UtcDateTime;
        }

        public string Extension { get; }
        public string ValueName { get; }
        public string TargetName { get; }
        public string LnkName { get; }
        public int MruPosition { get; }

        public DateTime? OpenedOn { get; }

        public DateTime? ExtensionLastOpened { get; }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Lnk: {LnkName} Target: {TargetName}";
        public string BatchValueData2 => $"Opened on: {OpenedOn?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} Ext last open: {ExtensionLastOpened?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"MRU: {MruPosition}";
    }
}
