using RegistryPluginBase.Interfaces;

namespace RegistryPlugin._7_ZipCopyHistory
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string copyPath)
        {
            CopyPath = copyPath;
            // Order = order;
        }

        public string CopyPath { get; }
        // public int Order { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Path: {CopyPath}";
        // public string BatchValueData2 => $"Order: {Order}";
        public string BatchValueData2 => string.Empty;
        public string BatchValueData3 => string.Empty;
    }
}
