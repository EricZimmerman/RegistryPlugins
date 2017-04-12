using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.KnownNetworks
{
  public  class KnownNetworks : IRegistryPluginGrid
    {
        private readonly BindingList<KnownNetwork> _values;

        public KnownNetworks()
        {
            _values = new BindingList<KnownNetwork>();

            Errors = new List<string>();
        }

        public string InternalGuid => "663d3ac7-0229-4756-ada3-ed4f39646170";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Microsoft\Windows NT\CurrentVersion\NetworkList"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Known networks";

        public string ShortDescription
            => "Displays information about networks a computer has connected to";

        public string LongDescription
            =>
            "Note: The first and last connect timestamps are displayed in LOCAL time and will always reflect the time zone of the machine this plugin is executed on.";

        public double Version => 0.5;
        public List<string> Errors { get; }
        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var profiles = key.SubKeys.SingleOrDefault(t => t.KeyName == "Profiles");

            if (profiles == null)
            {
                Errors.Add($"'Profiles' key missing!' ");
                return;
            }

            foreach (var profilesSubKey in profiles.SubKeys)
            {
                try
                {
var rawCreated = profilesSubKey.Values.Single(t => t.ValueName == "DateCreated").ValueDataRaw;
               var rawLast = profilesSubKey.Values.Single(t => t.ValueName == "DateLastConnected").ValueDataRaw;

                var isManaged = profilesSubKey.Values.Single(t => t.ValueName == "DateLastConnected").ValueData == "0";

                var profileName = profilesSubKey.Values.Single(t => t.ValueName == "ProfileName").ValueData;

                var networkName = string.Empty;
                if (!isManaged)
                {
                    networkName = profileName;
                }

                var dnsSuffix = string.Empty;
                var macAddress = string.Empty;

                var typeNum = int.Parse( profilesSubKey.Values.Single(t => t.ValueName == "NameType").ValueData);

                var networkType = KnownNetwork.NameTypes.Unknown;

                if (typeNum > 0)
                {
                    try
                    {
                        networkType = (KnownNetwork.NameTypes)typeNum;
                    }
                    catch (Exception)
                    {
                        Errors.Add($"Could not determine network type! Type value: {typeNum}");
                    }
                }

                var kn = new KnownNetwork(networkName,networkType,GetDateFrom128Bit(rawCreated),GetDateFrom128Bit(rawLast),isManaged,dnsSuffix,macAddress,profilesSubKey.KeyName);
                _values.Add(kn);
                }
                catch (Exception e)
                {
                    Errors.Add($"Error processing Profiles subkey '{profilesSubKey.KeyName}': {e.Message}");
                }
               
            }

            var sigsKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Signatures");

            if (sigsKey == null)
            {
                Errors.Add($"'Signatures' key missing!' ");
                return;
            }

            var unmanaged = sigsKey.SubKeys.SingleOrDefault(t => t.KeyName == "Unmanaged");

            if (unmanaged == null)
            {
                Errors.Add($"'Unmanaged' key missing!' ");
                return;
            }

            foreach (var unmanagedKey in unmanaged.SubKeys)
            {
                try
                {
                    var gatewayMacRaw = unmanagedKey.Values.Single(t => t.ValueName == "DefaultGatewayMac").ValueData;
                    var dnsSuffix = unmanagedKey.Values.Single(t => t.ValueName == "DnsSuffix").ValueData;
                    var profileGuid = unmanagedKey.Values.Single(t => t.ValueName == "ProfileGuid").ValueData;
                    var firstNetwork = unmanagedKey.Values.Single(t => t.ValueName == "FirstNetwork").ValueData;

                    var kn = _values.SingleOrDefault(t => t.ProfileGUID == profileGuid);

                    kn?.UpdateInfo(gatewayMacRaw, dnsSuffix, firstNetwork);
                }
                catch (Exception e)
                {
                    Errors.Add($"Error processing Unmanaged subkey '{unmanagedKey.KeyName}': {e.Message}");
                }
            }

            var managed = sigsKey.SubKeys.SingleOrDefault(t => t.KeyName == "Managed");

            if (managed == null)
            {
                Errors.Add("'Managed' key missing!' ");
                return;
            }

            foreach (var managedKey in managed.SubKeys)
            {
                try
                {
                    var gatewayMacRaw = managedKey.Values.Single(t => t.ValueName == "DefaultGatewayMac").ValueData;
                    var dnsSuffix = managedKey.Values.Single(t => t.ValueName == "DnsSuffix").ValueData;
                    var profileGuid = managedKey.Values.Single(t => t.ValueName == "ProfileGuid").ValueData;
                    var firstNetwork = managedKey.Values.Single(t => t.ValueName == "FirstNetwork").ValueData;

                    var kn = _values.SingleOrDefault(t => t.ProfileGUID == profileGuid);

                    kn?.UpdateInfo(gatewayMacRaw, dnsSuffix, firstNetwork);
                }
                catch (Exception e)
                {
                    Errors.Add($"Error processing Managed subkey '{managedKey.KeyName}': {e.Message}");
                }
            }

        }

        public DateTimeOffset GetDateFrom128Bit(byte[] rawBytes)
        {
            int year = BitConverter.ToInt16(rawBytes, 0);
            int month = BitConverter.ToInt16(rawBytes, 2);
            int weekday = BitConverter.ToInt16(rawBytes, 4);
            int day = BitConverter.ToInt16(rawBytes, 6);
            int hour = BitConverter.ToInt16(rawBytes, 8);
            int minutes = BitConverter.ToInt16(rawBytes, 10);
            int seconds = BitConverter.ToInt16(rawBytes, 12);
            int thousands = BitConverter.ToInt16(rawBytes, 14);

          // var dt = new DateTimeOffset(year, month, day, hour, minutes, seconds, thousands, TimeZoneInfo.Local.GetUtcOffset(DateTime.Now));
           var dt1 = new DateTime(year, month, day, hour, minutes, seconds, thousands, DateTimeKind.Local);

            var dt = new DateTimeOffset(dt1);

            return dt;
        }

        public IBindingList Values => _values;
    }
}
