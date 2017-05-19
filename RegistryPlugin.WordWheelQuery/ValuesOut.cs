namespace RegistryPlugin.WordWheelQuery
{
    public class ValuesOut
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
    }
}