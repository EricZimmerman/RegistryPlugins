using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FileExts
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string ext, string openExes, string openProgIDs, string userChoice)
        {
            Extension = ext;
            OpensWithExecutables = openExes;
            OpensWithProgramIDs = openProgIDs;
            UserChoice = userChoice;
        }

        public string Extension { get; }
        public string OpensWithExecutables { get; }
        public string OpensWithProgramIDs { get; }
        public string UserChoice { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Extension: {Extension}";
        public string BatchValueData2 => $"Open Exes: {OpensWithExecutables} Open ProgIds: {OpensWithProgramIDs}";
        public string BatchValueData3 => $"User choise: {UserChoice}";
    }
}