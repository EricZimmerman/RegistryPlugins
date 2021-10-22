using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.SAMBuiltin
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string groupname, string comment, string users)
        {
            GroupName = groupname;
            Comment = comment;
            Users = users;
        }

        public string GroupName { get; }
        public string Comment { get; }
        public string Users { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"GroupName: {GroupName}";
        public string BatchValueData2 => $"Comment: {Comment}";
        public string BatchValueData3 => $"Users: {Users}";
    }
}