using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.NetworkSetup2
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string protocollist, string alias, string description, string type, string permanentaddress, string currentaddress)
        {
            ProtocolList = protocollist;
            Alias = alias;
            Description = description;
            Type = type;
            PermanentAddress = permanentaddress;
            CurrentAddress = currentaddress;
        }

        public string ProtocolList { get; }
        public string Alias { get; }
        public string Description { get; }
        public string Type { get; }
        public string PermanentAddress { get; }
        public string CurrentAddress { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"ProtocolList: {ProtocolList} Alias: {Alias}";
        public string BatchValueData2 => $"Description: {Description} Type: {Type}";
        public string BatchValueData3 => $"PermanentAddress: {PermanentAddress} CurrentAddress:{CurrentAddress}";
    }
}