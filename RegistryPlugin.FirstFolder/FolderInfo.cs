using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FirstFolder
{
    public class FolderInfo:IValueOut
    {
        public FolderInfo(string exeName, string folderName, int mruPos, DateTimeOffset? openedOn)
        {
            Executable = exeName;
            FolderName = folderName;
            MRUPosition = mruPos;
            OpenedOn = openedOn?.UtcDateTime;
        }

        public string Executable { get; }
        public string FolderName { get; }
        public int MRUPosition { get; }
        public DateTime? OpenedOn { get; }

        public override string ToString()
        {
            return $"Exe: {Executable}, Folder: {FolderName}, Mru: {MRUPosition}";
        }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Exe: {Executable} Folder: {FolderName}";
        public string BatchValueData2 => $"Opened: {OpenedOn?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Mru: {MRUPosition}";
    }
}
