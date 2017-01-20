using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FileExts
{
    public class FileExts : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public FileExts()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "4f76cdc0-91bd-46af-86ad-2fcf2efbd7e2";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "File Extensions";

        public string ShortDescription =>
            "Extracts list of file extensions and associated programs";

        public string LongDescription =>
            @"Processes subkeys of 'Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts' and displays associated executables, program IDs and user choice for each extension found. The list of executables used for a given file extension are in MRUList order"
            ;

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var currentKey = string.Empty;

            try
            {
                foreach (var registryKey in key.SubKeys) // subkeys of FileExts
                {
                    currentKey = registryKey.KeyName;

                    var oe = new List<string>();
                    var op = new List<string>();
                    var uc = "(UserChoice key not present)";

                    if (registryKey.SubKeys.Count == 0)
                    {
                        var progId = registryKey.Values.SingleOrDefault(t => t.ValueName == "Progid");
                        if (progId != null)
                        {
                            op.Add(progId.ValueData);
                        }
                        var vo1 = new ValuesOut(registryKey.KeyName, string.Join(", ", oe), string.Join(", ", op), uc);

                        _values.Add(vo1);
                        continue;
                    }


                    foreach (var subKey in registryKey.SubKeys) // subkey's subkeys
                    {
                        switch (subKey.KeyName)
                        {
                            case "OpenWithList":
                                // contains values with name == char and value data of an executable name
                                //there is an MRUList that contains the order the executables were selected

                                var mruList = subKey.Values.SingleOrDefault(t => t.ValueName == "MRUList");

                                if (mruList != null)
                                {
                                    //foreach slot in MRU, get the value and append it to our oe variable

                                    foreach (var mruPos in mruList.ValueData.ToCharArray())
                                    {
                                        var exeName =
                                            subKey.Values.SingleOrDefault(t => t.ValueName == mruPos.ToString());

                                        if (exeName != null)
                                        {
                                            oe.Add(exeName.ValueData);
                                        }
                                        else
                                        {
                                            oe.Add($"(Executable name for MRU slot '{mruPos}' not found!)");
                                        }
                                    }
                                }


                                break;
                            case "OpenWithProgids":
                                foreach (var proIdValue in subKey.Values)
                                {
                                    op.Add(proIdValue.ValueName);
                                }
                                break;
                            case "UserChoice":
                                var progId = subKey.Values.SingleOrDefault(t => t.ValueName == "ProgId");
                                if (progId != null)
                                {
                                    uc = progId.ValueData;
                                }
                                break;
                        }
                    }

                    //we have enough to add an entry

                    var vo = new ValuesOut(registryKey.KeyName, string.Join(", ", oe), string.Join(", ", op), uc);

                    _values.Add(vo);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing FileExts subkey {currentKey}: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}