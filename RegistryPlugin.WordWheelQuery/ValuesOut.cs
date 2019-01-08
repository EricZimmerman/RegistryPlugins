using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.WordWheelQuery
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string searchTerm, int mruPosition,string keyName)
        {
            SearchTerm = searchTerm;
            MruPosition = mruPosition;
            KeyName = keyName;
        }

        public string SearchTerm { get; }
       

        public int MruPosition { get; }
        public string KeyName { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Term: {SearchTerm}";
        public string BatchValueData2 => $"MRU: {MruPosition}";
        public string BatchValueData3 => string.Empty;
    }
}