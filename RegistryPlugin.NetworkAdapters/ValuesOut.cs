using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.NetworkAdapters
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string driverdesc, string driverdate, string driverversion, string deviceinstanceid, string providername, DateTimeOffset? timestamp)
        {
            DriverDesc = driverdesc;
            DriverDate = driverdate;
            DriverVersion = driverversion;
            ProviderName = providername;
            DeviceInstanceid = deviceinstanceid;
            Timestamp = timestamp;
        }

        public DateTimeOffset? Timestamp { get; }

        public string DriverDesc { get; }
        public string DriverDate { get; }
        public string DriverVersion { get; }
        public string ProviderName { get; }
        public string DeviceInstanceid { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"DriveName: {DriverDesc} DriverDate: {DriverDate} DriverVersion: {DriverVersion} ProviderName: {ProviderName}";
        public string BatchValueData2 => $"DeviceInstanceid: {DeviceInstanceid}";
        public string BatchValueData3 => $"Timestamp: {Timestamp?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}
