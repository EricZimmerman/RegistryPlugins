using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.CIDSizeMRU
{
    public class CIDSizeMRU : IRegistryPluginGrid
    {
        private readonly BindingList<CIDSizeInfo> _values;

        public CIDSizeMRU()
        {
            _values = new BindingList<CIDSizeInfo>();

            Errors = new List<string>();
        }

        public string InternalGuid => "30e3c2b7-899f-409a-9687-613ca63933c6";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\CIDSizeMRU"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "ComDlg32 CIDSizeMRU";
        public string ShortDescription => "Displays program executables";

        public string LongDescription
            =>
                "";

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


        private IEnumerable<CIDSizeInfo> ProcessRecentKey(RegistryKey key)
        {
            var l = new List<CIDSizeInfo>();


            try
            {
                var mruList = key.Values.FirstOrDefault(t => t.ValueName == "MRUListEx");

                var mruPositions = new Dictionary<uint, int>();

                var i = 0;

                var index = 0;

                //up to 0xFFFFFFFF or the end of the value; a position seen before keeps its first place
                while (mruList != null && index + 4 <= mruList.ValueDataRaw.Length)
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

                foreach (var keyValue in key.Values)
                {
                    if (keyValue.ValueName == "MRUListEx")
                    {
                        continue;
                    }

                    if (uint.TryParse(keyValue.ValueName, out var valueNumber) == false)
                    {
                        continue;
                    }

                    //a value that isn't in MRUListEx has no position (it ended the key with an error)
                    var mru = mruPositions.TryGetValue(valueNumber, out var position) ? position : -1;

                    var chunks = Encoding.Unicode.GetString(keyValue.ValueDataRaw).Split('\0');

                    var exeName = chunks[0];

                    DateTimeOffset? openedOn = null;

                    if (mru == 0)
                    {
                        openedOn = key.LastWriteTime;
                    }

                    var ff = new CIDSizeInfo(exeName, mru, openedOn);
                    ff.BatchValueName = keyValue.ValueName;
                    ff.BatchKeyPath = key.KeyPath;


                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing CIDSizeMRU key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }


            return l.OrderBy(t => t.MRUPosition);
        }
    }
}