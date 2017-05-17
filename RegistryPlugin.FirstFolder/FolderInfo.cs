using System;

namespace RegistryPlugin.FirstFolder
{
    public class FolderInfo
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
    }
}