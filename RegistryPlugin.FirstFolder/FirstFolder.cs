using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FirstFolder
{
    public class FirstFolder : IRegistryPluginGrid
    {
        private readonly BindingList<FolderInfo> _values;

        public FirstFolder()
        {
            _values = new BindingList<FolderInfo>();

            Errors = new List<string>();
        }

        public string InternalGuid => "0fdd15ea-7302-48fb-92ad-30f144731fa1";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\FirstFolder"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "First folder";

        public string ShortDescription
            => "Displays program executables and optionally, the first folder to select for said program";

        public string LongDescription
            =>
                "Note: Not all entries will have a folder value";

        public double Version => 0.5;
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


        private IEnumerable<FolderInfo> ProcessKey(RegistryKey key)
        {
            var l = new List<FolderInfo>();


            try
            {
                var mruList = key.Values.Single(t => t.ValueName == "MRUListEx");

                var mruPositions = new Dictionary<uint, int>();

                var i = 0;

                var index = 0;

                var mruPos = BitConverter.ToUInt32(mruList.ValueDataRaw, index);
                index += 4;

                while (mruPos != 0xFFFFFFFF)
                {
                    mruPositions.Add(mruPos, i);
                    i++;
                    mruPos = BitConverter.ToUInt32(mruList.ValueDataRaw, index);
                    index += 4;
                }

                //mruPositions now contains a map of positions (the key) to the order it was opened (the value)

                foreach (var keyValue in key.Values)
                {
                    if (keyValue.ValueName == "MRUListEx")
                    {
                        continue;
                    }

                    var mru = mruPositions[uint.Parse(keyValue.ValueName)];

                    var chunks = Encoding.Unicode.GetString(keyValue.ValueDataRaw).Split('\0');

                    var exeName = chunks[0];

                    var folder = string.Empty;

                    if (chunks.Length > 1)
                    {
                        folder = chunks[1];
                    }

                    DateTimeOffset? openedOn = null;

                    if (mru == 0)
                    {
                        openedOn = key.LastWriteTime;
                    }

                    var ff = new FolderInfo(exeName, folder, mru, openedOn)
                    {
                        BatchKeyPath = key.KeyPath, BatchValueName = keyValue.ValueName
                    };

                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing FirstFolder key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l.OrderBy(t => t.MRUPosition);
        }
    }
}