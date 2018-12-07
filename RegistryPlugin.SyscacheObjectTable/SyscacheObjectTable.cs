using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.SyscacheObjectTable
{
    public class SyscacheObjectTable : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public SyscacheObjectTable()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "b3560cb3-a19c-4017-848d-c7e7ea41d8f7";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"DefaultObjectStore\ObjectTable"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Syscache.hve ObjectTable";

        public string ShortDescription =>
            "Extracts information from Syscache.hve ObjectTable subkeys/values";

        public string LongDescription => ShortDescription;

        public double Version => 0.5;
        public List<string> Errors { get; }


        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var registryKey in key.SubKeys)
            {
                try
                {
                    long objId = 0;
                    long objLru = 0;
                    var fileId = new byte[8];
                    long usn = 0;
                    long usnId = 0;
                    var aeFile = string.Empty;
                    var aeProgram = string.Empty;

                    foreach (var registryKeyValue in registryKey.Values)
                    {
                        switch (registryKeyValue.ValueName)
                        {
                            case "_ObjectId_":
                                objId = long.Parse(registryKeyValue.ValueData);
                                break;
                            case "_ObjectLru_":
                                objLru = long.Parse(registryKeyValue.ValueData);
                                break;
                            case "_FileId_":
                                fileId = registryKeyValue.ValueDataRaw;
                                break;
                            case "_Usn_":
                                usn = long.Parse(registryKeyValue.ValueData);
                                break;
                            case "_UsnJournalId_":
                                usnId = long.Parse(registryKeyValue.ValueData);
                                break;
                            case "AeFileID":
                                aeFile = Encoding.Unicode.GetString(registryKeyValue.ValueDataRaw).Trim('\0');
                                break;
                            case "AeProgramID":
                                aeProgram = Encoding.Unicode.GetString(registryKeyValue.ValueDataRaw).Trim('\0');
                                break;
                        }
                    }

                    //we have all our data

                    var sequenceNumber = BitConverter.ToUInt16(fileId, 6);

                    ulong entryIndex = 0;

                    ulong entryIndex1 = BitConverter.ToUInt32(fileId, 0);
                    ulong entryIndex2 = BitConverter.ToUInt16(fileId, 4);

                    if (entryIndex2 == 0)
                    {
                        entryIndex = entryIndex1;
                    }
                    else
                    {
                        entryIndex2 = entryIndex2 * 16777216; //2^24
                        entryIndex = entryIndex1 + entryIndex2;
                    }

                    var sha = aeFile.Length > 4 ? aeFile.Substring(4).ToLowerInvariant() : string.Empty;

                    var vo = new ValuesOut( objId, usn, usnId, (int) entryIndex, sequenceNumber, sha, aeProgram, objLru,registryKey.LastWriteTime.Value,Registry.Other.Helpers.StripRootKeyNameFromKeyPath(registryKey.KeyPath));
                    _values.Add(vo);
                }
                catch (Exception ex)
                {
                    Errors.Add(
                        $"Error processing Syscache ObjectTable entry in key {registryKey.KeyName}: {ex.Message}");
                }
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}