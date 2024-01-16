using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Adobe.TrustManager_Websites
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string website, char userChoice)
        {
            Website = website;
            switch (userChoice)
            {
                case '2': UserChoice = "Allow"; break;
                case '3': UserChoice = "Block"; break;
                default: UserChoice = "Undefined"; break;
            }
            
           
        }


        public string Website { get; }
        public string UserChoice { get; }


        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Website: {Website}";
        public string BatchValueData2 => $"User Choice: {UserChoice}";
        public string BatchValueData3 => string.Empty;
    }
}
