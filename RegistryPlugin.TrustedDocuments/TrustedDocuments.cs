using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegistryPlugin.TrustedDocuments
{
    public class TrustedDocuments : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public TrustedDocuments()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "5866960d-0bf7-4a73-8443-931caeda30c3";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Office\15.0\Word\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\15.0\Excel\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\15.0\PowerPoint\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\15.0\Access\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\15.0\OneNote\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\16.0\Word\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\16.0\Excel\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\16.0\PowerPoint\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\16.0\Access\Security\Trusted Documents\TrustRecords",
            @"Software\Microsoft\Office\16.0\OneNote\Security\Trusted Documents\TrustRecords"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Partha Alwar";
        public string Email => "parthaalwar@gmail.com";
        public string Phone => "000-000-0000";
        public string PluginName => "TrustedDocuments";

        public string ShortDescription =>
            "Extracts names of Office documents where the user may have clicked on \"Enable Editing\" or \"Enable Macro or Enable Content\"";

        public string LongDescription => ShortDescription;

        public double Version => 0.1;
        public List<string> Errors { get; }


        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {
                foreach (var keyValue in key.Values)
                {
                    //value data contains binary data that provides the timestamp as well as information on if "Enable Macro" or "Enable Content" was clicked by the user
                    var valData = keyValue.ValueData.ToString();
                    //value name contains the full path of the document
                    var fName = keyValue.ValueName;
                    var bin = valData.ToString().Replace("-", "");
                    var hexDate = bin.Substring(0, 16);
                    var type = bin.Substring(bin.Length - 8);
                    string EventType;
                    byte[] dateval = StringToByteArray(hexDate);
                    DateTimeOffset tstamp = ConvertWindowsDate(dateval);
                    if (type == "01000000")
                    {
                        EventType = "Enable Editing";
                    }
                    else if (type == "FFFFFF7F")
                    {
                        EventType = "Enable Content/Macros";
                    }
                    else
                    {
                        EventType = "Unknown value";
                    }
                    // to read the username of the current NTUSER.DAT hive, go up a few levels and list the value Software\Microsoft\Internet Explorer\Suggested Sites\LocalLogFolder
                    var internetExplorerKey = key.Parent.Parent.Parent.Parent.Parent.Parent.SubKeys.SingleOrDefault(t => t.KeyName == "Internet Explorer");
                    string username = "";
                    foreach (var registryKey in internetExplorerKey.SubKeys)
                    {
                        var suggestedSitesKey = internetExplorerKey.SubKeys.SingleOrDefault(t => t.KeyName == "Suggested Sites");
                        foreach (var value in suggestedSitesKey.Values)
                        {
                            if (value.ValueName == "LogFileFolder")
                            {
                                username = value.ValueData.Split('\\')[2].ToString();
                                break;
                            }
                        }
                    }
                    var v = new ValuesOut(EventType, tstamp, fName, username);
                    v.BatchKeyPath = key.KeyPath;
                    v.BatchValueName = keyValue.ValueName;
                    _values.Add(v);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Trusted Documents key: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public static DateTimeOffset ConvertWindowsDate(byte[] bytes)
        {
            if (bytes.Length != 8) throw new ArgumentException();
            return DateTime.SpecifyKind((DateTime.FromFileTimeUtc(BitConverter.ToInt64(bytes, 0))),DateTimeKind.Utc);
        }

        public IBindingList Values => _values;
    }
}