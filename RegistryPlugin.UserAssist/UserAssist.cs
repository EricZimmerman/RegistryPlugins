using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ExtensionBlocks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.UserAssist
{
    public class UserAssist : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public UserAssist()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "5222820b-efea-4f5d-b2bd-4ef3dcd3007b";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist\*\Count"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "UserAssist";
        public string ShortDescription => "Un ROT-13s UserAssist key values and extracts execution count, last run, etc.";

        public string LongDescription
            =>
                "UserAssist is a method used to populate a user’s start menu with frequently used applications. This is achieved by maintaining a count of application use in each users NTUSER.DAT registry file. This key is suppose to contain information about programs and shortcuts accessed by the Windows GUI, including execution count, date of last execution, count of focuses, and total seconds focused";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var keyValue in key.Values)
            {
                try
                {
                    var unrot = Helpers.Rot13Transform(keyValue.ValueName);
                    var run = 0;

                    string guid = null;
                    try
                    {
                        guid =
                            Regex.Match(unrot, @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b",
                                RegexOptions.IgnoreCase).Value;

                        var foldername = Utils.GetFolderNameFromGuid(guid);

                        unrot = unrot.Replace(guid, foldername);
                    }
                    catch (ArgumentException)
                    {
                        // Syntax error in the regular expression
                    }


                    DateTimeOffset? lastRun = null;
                    int? focusCount = null;
                    TimeSpan focusTime = new TimeSpan();

                    if (keyValue.ValueDataRaw.Length >= 16)
                    {
                        run = BitConverter.ToInt32(keyValue.ValueDataRaw, 4);

                        lastRun = DateTimeOffset.FromFileTime(BitConverter.ToInt64(keyValue.ValueDataRaw, 8));

                        // Windows 7 and up, new format
                        if (keyValue.ValueDataRaw.Length >= 68)
                        {
                            focusCount = BitConverter.ToInt32(keyValue.ValueDataRaw, 8);
                            focusTime = TimeSpan.FromMilliseconds(BitConverter.ToInt32(keyValue.ValueDataRaw, 12));
                            lastRun = DateTimeOffset.FromFileTime(BitConverter.ToInt64(keyValue.ValueDataRaw, 60));
                        }
                    }

                    if (lastRun?.Year < 1970)
                    {
                        lastRun = null;
                    }

                    var vo = new ValuesOut(keyValue.ValueName, unrot, run, lastRun, focusCount, focusTime.ToString(@"d'd, 'h'h, 'mm'm, 'ss's'"));
                    vo.BatchKeyPath = key.KeyPath;
                    vo.BatchValueName = keyValue.ValueName;

                    _values.Add(vo);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Value name: {keyValue.ValueName}, message: {ex.Message}");
                }
            }
        }


        public IBindingList Values => _values;
    }
}