using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using ExtensionBlocks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.RecentDocs
{
    public class RecentDocs : IRegistryPluginGrid
    {
        private readonly BindingList<RecentDoc> _values;

        public RecentDocs()
        {
            _values = new BindingList<RecentDoc>();

            Errors = new List<string>();
        }

        public string InternalGuid => "42c3891d-6a08-49df-a930-3f7cb2141840";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs",
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs\*"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Recent documents";
        public string ShortDescription => "Displays recently opened documents, by extension";

        public string LongDescription
            =>
                "A list of all recent documents will be generated when clicking on the root RecentDocs key. A list for a certain extension will be generated when clicking on a subkey of RecentDocs";

        public double Version => 0.6;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var rd in ProcessRecentKey(key))
            {
                _values.Add(rd);
            }
        }

        public IBindingList Values => _values;


        private IEnumerable<RecentDoc> ProcessRecentKey(RegistryKey key)
        {
            var l = new List<RecentDoc>();

            try
            {
                var mruList = key.Values.FirstOrDefault(t => t.ValueName == "MRUListEx");

                var mruPositions = new Dictionary<uint, int>();

                var index = 0;

                if (mruList != null)
                {
                    var i = 0;


                    //up to 0xFFFFFFFF or the end of the value (one without it threw, and the key was lost); a position seen
                    //before keeps its first place
                    while (index + 4 <= mruList.ValueDataRaw.Length)
                    {
                        var mruPos = BitConverter.ToUInt32(mruList.ValueDataRaw, index);
                        index += 4;

                        if (mruPos == 0xFFFFFFFF)
                        {
                            break;
                        }

                        if (mruPositions.ContainsKey(mruPos) == false)
                        {
                            mruPositions.Add(mruPos, i);
                        }

                        i++;
                    }

                    //mruPositions now contains a map of positions (the key) to the order it was opened (the value)    
                }


                foreach (var keyValue in key.Values)
                {
                    if (keyValue.ValueName == "MRUListEx" || keyValue.ValueName == "ViewStream")
                    {
                        continue;
                    }

                    //settings such as CachePrefix are not entries (one ended the key with an error)
                    if (uint.TryParse(keyValue.ValueName, out var valueNumber) == false)
                    {
                        continue;
                    }

                    try
                    {
                        var mru = mruPositions.TryGetValue(valueNumber, out var position) ? position : -1;

                        var targetName = Encoding.Unicode.GetString(keyValue.ValueDataRaw).Split('\0')[0];

                        var offsetToRemainingData = targetName.Length * 2 + 2;

                        //TODO do not use Skip. use Buffer.BlockCopy as its faster

                        var remainingData = keyValue.ValueDataRaw.Skip(offsetToRemainingData).ToArray();

                        index = 0;

                        var chunkLen = remainingData.Length >= 2 ? BitConverter.ToUInt16(remainingData, index) : (ushort) 0;

                        var chunks = new List<byte[]>();

                        while (remainingData.Length > index)
                        {
                            var chunk = remainingData.Skip(index).Take(chunkLen).ToArray();

                            chunks.Add(chunk);

                            index += chunkLen;

                            //a value can end with its last chunk, without the 0 after it (reading one threw)
                            chunkLen = index + 2 <= remainingData.Length ? BitConverter.ToUInt16(remainingData, index) : (ushort) 0;

                            if (chunkLen == 0)
                            {
                                break;
                            }
                        }

                        index = 2; //skip chunk length
                        var lnkNameType = chunks[0][index]; // if 32, its unicode, if 36, ascii
                        index += 12; //skip type that always seems to be [32|36]-00-00-00-00-00-00-00-00-00-00-00-

                        var lnkName = "";

                        if (lnkNameType == 36)
                        {
                            lnkName = Encoding.Unicode.GetString(chunks[0].Skip(index).ToArray()).Split('\0')[0];
                            index += lnkName.Length * 2;
                        }
                        else
                        {
                            lnkName = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(chunks[0].Skip(index).ToArray()).Split('\0')[0];
                            index += lnkName.Length;
                        }

                        var found = false;
                        while (index <= chunks[0].Length - 4)
                        {
                            if (BitConverter.ToUInt32(chunks[0], index) == 0xbeef0004)
                            {
                                found = true;
                                break;
                            }
                            index += 1; //move until our signature
                        }

                        //no 0xbeef0004 block (or a damaged one): the name from the item instead of its long name
                        Beef0004 beef = null;
                        if (found && index >= 4)
                        {
                            index -= 4; //jump back to start of extension block

                            var beefBytes = chunks[0].Skip(index).ToArray();

                            var sig = BitConverter.ToUInt32(beefBytes, 4);
                            beef = Utils.GetExtensionBlockFromBytes(sig, beefBytes) as Beef0004;
                        }

                        DateTimeOffset? openedOn = null;

                        if (mru == 0)
                        {
                            openedOn = key.LastWriteTime;
                        }

                        DateTimeOffset? extLastOpened = null;

                        var ext = Path.GetExtension(targetName).ToLowerInvariant();

                        var targetName1 = string.Empty;

                        if (ext.Length == 0)
                        {
                            //folder
                            var sk1 = key.SubKeys.FirstOrDefault(t => t.KeyName == "Folder");
                            var skmru = sk1?.Values.FirstOrDefault(t => t.ValueName == "MRUListEx");

                            if (skmru?.ValueDataRaw.Length >= 4)
                            {
                                //get last accessed folder value name
                                var mruPosf = BitConverter.ToInt32(skmru.ValueDataRaw, 0);

                                //pull folder name from the value
                                var val1 = sk1.Values.FirstOrDefault(t => t.ValueName == mruPosf.ToString());

                                targetName1 = val1 == null ? string.Empty : Encoding.Unicode.GetString(val1.ValueDataRaw).Split('\0')[0];
                            }

                            if (sk1 != null && targetName1 == targetName)
                            {
                                extLastOpened = sk1.LastWriteTime;
                            }
                        }
                        else
                        {
                            var sk2 = key.SubKeys.FirstOrDefault(t => t.KeyName.ToLowerInvariant() == ext);
                            var skmruf = sk2?.Values.FirstOrDefault(t => t.ValueName == "MRUListEx");

                            if (skmruf?.ValueDataRaw.Length >= 4)
                            {
                                //get last accessed folder value name
                                var mruPosff = BitConverter.ToInt32(skmruf.ValueDataRaw, 0);

                                //pull folder name from the value
                                var val1 = sk2.Values.FirstOrDefault(t => t.ValueName == mruPosff.ToString());

                                targetName1 = val1 == null ? string.Empty : Encoding.Unicode.GetString(val1.ValueDataRaw).Split('\0')[0];
                            }

                            if (sk2 != null && targetName1 == targetName)
                            {
                                extLastOpened = sk2.LastWriteTime;
                            }
                        }


                        var rd = new RecentDoc(mru, keyValue.ValueName, targetName,  beef?.LongName ?? lnkName, key.KeyName, openedOn, extLastOpened);

                        rd.BatchKeyPath = key.KeyPath;
                        rd.BatchValueName = keyValue.ValueName;

                        l.Add(rd);
                    }
                    catch (Exception ex)
                    {
                        //one value: the others are still listed
                        Errors.Add($"Error processing recent key ({key.KeyPath}), value {keyValue.ValueName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing recent key ({key.KeyPath}): {ex.Message}");
            }

            foreach (var registryKey in key.SubKeys)
            {
                var subItems = ProcessRecentKey(registryKey);

                l.AddRange(subItems);
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            l = l.OrderByDescending(t => t.Extension).ThenBy(t => t.MruPosition).ToList();

            return l;
        }
    }
}