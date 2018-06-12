namespace RegistryPlugin.TaskFlowShellActivities
{
    public class ValuesOut
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
    }
}