using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.WindowsApp
{
    public class WindowsApp : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public WindowsApp()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }
        public string InternalGuid => "a0ea35bb-380a-4b1b-9d9a-f4b231242662";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\Repository", // UsrClass.dat
            @"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\Repository" // NTUSER.dat
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "Windows App";

        public string ShortDescription
            => "Windows App List";

        public string LongDescription => "https://www.datadigitally.com/2019/05/windows-10-specific-registry-keys.html";

        public double Version => 0.1;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var rd in ProcessKey(key))
            {
                _values.Add(rd);
            }
        }

        public IBindingList Values => _values;

        public DateTimeOffset GetDateTimeOffset(string timestamp)
        {
            var timestampInt = Convert.ToInt64(timestamp);
            var dt1 = DateTime.FromFileTime(timestampInt);
            return new DateTimeOffset(dt1);
        }

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();

            var familiesKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Families");
            var packagesKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Packages");

            if (familiesKey != null && packagesKey != null)
            {
                foreach (var subKey in familiesKey.SubKeys)
                {
                    foreach (var AppPath in subKey.SubKeys)
                    {
                        try
                        {
                            var installTime = GetDateTimeOffset(AppPath.Values.SingleOrDefault(t => t.ValueName == "InstallTime")?.ValueData);
                            var packagesPath = packagesKey.SubKeys.SingleOrDefault(t => t.KeyName == AppPath.KeyName);
                            string displayName = null;
                            string packageRootFolder = null;

                            if (packagesPath != null)
                            {
                                displayName = packagesPath.Values.SingleOrDefault(t => t.ValueName == "DisplayName")?.ValueData;
                                packageRootFolder = packagesPath.Values.SingleOrDefault(t => t.ValueName == "PackageRootFolder")?.ValueData;
                            }
                            var ff = new ValuesOut(AppPath.KeyName, displayName, installTime, packageRootFolder)
                            {
                                BatchValueName = "Multiple",
                                BatchKeyPath = subKey.KeyPath
                            };
                            l.Add(ff);
                        }
                        catch (Exception ex)
                        {
                            Errors.Add($"Error processing WindowsApp key: {ex.Message}");
                        }
                    }
                }
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}
