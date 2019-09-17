using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.MountedDevices
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string deviceName, string deviceData)
        {
            DeviceName = deviceName;
            DeviceData = deviceData;
        }

        public string DeviceName { get; }
        public string DeviceData { get; }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Name: {DeviceName}";
        public string BatchValueData2 => $"Data: {DeviceData})";
        public string BatchValueData3 => string.Empty ;
    }
}