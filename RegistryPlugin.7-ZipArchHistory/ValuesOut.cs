using RegistryPluginBase.Interfaces;

namespace RegistryPlugin._7_ZipArchHistory
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string archiveName)
        {
            ArchiveName = archiveName;
            // Order = order;
        }

        public string ArchiveName { get; }
        // public int Order { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Archive: {ArchiveName}";
        // public string BatchValueData2 => $"Order: {Order}";
        public string BatchValueData2 => string.Empty;
        public string BatchValueData3 => string.Empty;
    }
}