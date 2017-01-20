namespace RegistryPlugin.FileExts
{
    public class ValuesOut
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
    }
}