using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using ExtensionBlocks;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.OpenSavePidlMRU
{
    public class OpenSavePidlMRU : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        private List<IShellBag> bags;

        public OpenSavePidlMRU()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "c76533f8-f721-40ff-896c-89ae832ac60f";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "ComDlg32 OpenSavePidlMRU";

        public string ShortDescription =>
            "Extracts shell items from OpenSavePidlMRU subkeys";

        public string LongDescription =>
            "OpenSavePidlMRU subkeys contain a wealth of information including timestamps, MFT information, GUIDs, and more";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var valuesList = new List<ValuesOut>();

            var currentKey = string.Empty;


            try
            {
                foreach (var registryKey in key.SubKeys)
                {
                    currentKey = registryKey.KeyName;

                    //get MRU key and read it in

                    var mruVal = registryKey.Values.FirstOrDefault(t => t.ValueName == "MRUListEx");

                    var mruListOrder = new ArrayList();

                    if (mruVal != null)
                    {
                        var index = 0;
                        var mruPos = 0;

                        while (index < mruVal.ValueDataRaw.Length)
                        {
                            mruPos = BitConverter.ToInt32(mruVal.ValueDataRaw, index);
                            index += 4;

                            if (mruPos != -1)
                            {
                                mruListOrder.Add(mruPos);
                            }
                        }
                    }


                    

                    foreach (var keyValue in registryKey.Values)
                    {
                        if (keyValue.ValueName == "MRUListEx")
                        {
                            continue;
                        }

                        if (registryKey.KeyName == "lnk" && keyValue.ValueName == "0")
                        {
                            Debug.WriteLine(1);

                        }

                        bags = new List<IShellBag>();

                        var shellItemsRaw = new List<byte[]>();

                        var mru = mruListOrder.IndexOf(int.Parse(keyValue.ValueName));  //(int) mruListOrder[int.Parse(keyValue.BatchValueName)];
                        DateTimeOffset? openedOn = null;

                        if (mru == 0)
                        {
                            openedOn = registryKey.LastWriteTime;
                        }

                        try
                        {
                            var det = new StringBuilder();

                            var index = 0;
                            while (index < keyValue.ValueDataRaw.Length)
                            {
                                var size = BitConverter.ToInt16(keyValue.ValueDataRaw, index);

                                if (size == 0)
                                {
                                    break;
                                }

                                var shellRaw = new byte[size];
                                Buffer.BlockCopy(keyValue.ValueDataRaw, index, shellRaw, 0, size);

                                shellItemsRaw.Add(shellRaw);

                                index += size;
                            }

                            foreach (var bytese in shellItemsRaw)
                            {
                                //the shared shell item classes (ExtensionBlocks), as ShellBags and Lnk read them
                                IShellBag bag;
                                try
                                {
                                    bag = ShellItemFactory.Create(bytese);
                                }
                                catch (Exception ex)
                                {
                                    //the other items (and the value) are still listed, as when the plugin had its own classes
                                    var message = $"Key: {registryKey.KeyName}, Value name: {keyValue.ValueName}, Message: shell item 0x{bytese[2]:x2} could not be parsed: {ex.Message}";
                                    det.AppendLine(message);
                                    Errors.Add(message);
                                    continue;
                                }

                                if (bag is ShellBagUnknown)
                                {
                                    var message = $"Key: {registryKey.KeyName}, Value name: {keyValue.ValueName}, Message: **** Unsupported ShellID: 0x{bytese[2]:x2}. Send this ID to saericzimmerman@gmail.com so support can be added!! ****";
                                    det.AppendLine(message);
                                    Errors.Add(message);
                                    continue;
                                }

                                det.AppendLine(bag.ToString());
                                bags.Add(bag);
                            }

                            var v = new ValuesOut(registryKey.KeyName,
                                $"{GetAbsolutePathFromTargetIDs(bags)}", det.ToString(), keyValue.ValueName, mru,
                                openedOn);
                            v.BatchKeyPath = registryKey.KeyPath;
                            v.BatchValueName = keyValue.ValueName;

                            valuesList.Add(v);
                        }
                        catch (Exception ex)
                        {
                            Errors.Add(
                                $"Key: {registryKey.KeyName}, Value name: {keyValue.ValueName}, message: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing OpenSavePidlMRU subkey {currentKey}: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            var v1 = valuesList.OrderBy(t => t.MruPosition);

            foreach (var source in v1.ToList())
            {
                _values.Add(source);
            }
        }


        public IBindingList Values => _values;

        private static string GetAbsolutePathFromTargetIDs(List<IShellBag> ids)
        {
            var absPath = string.Empty;

            foreach (var shellBag in ids)
            {
                absPath += shellBag.Value + @"\";
            }

            absPath = absPath.Substring(0, absPath.Length - 1);

            return absPath;
        }
    }
}