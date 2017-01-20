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
            OpenedOn = openedOn;
        }

        public string Executable { get; }
        public string FolderName { get; }
        public int MRUPosition { get; }
        public DateTimeOffset? OpenedOn { get; }

        public override string ToString()
        {
            return $"Exe: {Executable}, Folder: {FolderName}, Mru: {MRUPosition}";
        }
    }
}