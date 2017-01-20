namespace RegistryPlugin.TimeZoneInformation
{
    public class ValuesOut
    {
        public ValuesOut(string valueName, string valueData, string valueDataRaw)
        {
            ValueName = valueName;
            ValueData = valueData;
            ValueDataRaw = valueDataRaw;
        }

        public string ValueName { get; }
        public string ValueData { get; }
        public string ValueDataRaw { get; }
    }
}