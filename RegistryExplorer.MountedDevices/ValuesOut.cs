namespace RegistryExplorer.MountedDevices
{
    public class ValuesOut
    {
        public ValuesOut(string deviceName, string deviceData)
        {
            DeviceName = deviceName;
            DeviceData = deviceData;
        }

        public string DeviceName { get; }
        public string DeviceData { get; }
    }
}