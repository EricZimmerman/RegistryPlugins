using RegistryPluginBase.Interfaces;

namespace RegistryPlugin._7_ZipFolderHistory
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string folderPath)
        {
            FolderPath = folderPath;
            // Order = order;
        }

        public string FolderPath { get; }
        // public int Order { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Folder: {FolderPath}";
        // public string BatchValueData2 => $"Order: {Order}";
        public string BatchValueData2 => string.Empty;
        public string BatchValueData3 => string.Empty;
    }
}
