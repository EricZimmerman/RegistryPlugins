using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TaskFlowShellActivities
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string fullPath, string exeName, string windowTitle, string other, int pos)
        {
            FullPath = fullPath;
            ExeName = exeName;
            WindowTitle = windowTitle;
            Other = other;
            Position = pos;
        }

        public int Position { get; }
        public string FullPath { get; }
        public string ExeName { get; }
        public string WindowTitle { get; }
        public string Other { get; }

        public override string ToString()
        {
            return $"FP: {FullPath} WT: {WindowTitle}";
        }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Full path: {FullPath} Exe name: {ExeName}";
        public string BatchValueData2 => $"Window title: {WindowTitle} Position: {Position}";
        public string BatchValueData3 => $"Other: {Other}";
    }
}