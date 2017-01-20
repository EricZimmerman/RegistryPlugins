namespace RegistryPlugin.Ares
{
    public class ValuesOut
    {
        public ValuesOut(string propName, string propValue)
        {
            PropertyName = propName;
            PropertyValue = propValue;
        }

        public string PropertyName { get; }
        public string PropertyValue { get; }
    }
}