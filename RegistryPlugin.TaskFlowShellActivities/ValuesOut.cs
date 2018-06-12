namespace RegistryPlugin.TaskFlowShellActivities
{
    public class ValuesOut
    {
        public ValuesOut(string fullPath, string exeName, string windowTitle, string other)
        {
            FullPath = fullPath;
            ExeName = exeName;
            WindowTitle = windowTitle;
            Other = other;
        }

        public string FullPath { get; }
        public string ExeName { get; }
        public string WindowTitle { get; }
        public string Other { get; }

        public override string ToString()
        {
            return $"FP: {FullPath} WT: {WindowTitle}";
        }
    }
}