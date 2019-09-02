using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ExtensionBlocks;
using Registry.Abstractions;
using RegistryPlugin.Taskband.ShellItems;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;
using ShellBag = RegistryPlugin.Taskband.ShellItems.ShellBag;


namespace RegistryPlugin.Taskband
{
    public class Taskband : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        private List<ShellBag> bags;

        public Taskband()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "2a7a503e-46c6-5a8a-b310-bc9b5be31b00";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband"
        });

        public string ValueName => "Favorites";
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Taskband";

        public string ShortDescription =>
            "Displays values from Taskband Favorites value";

        public string LongDescription =>
            ShortDescription;

        public double Version => 0.5;

        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var valuesList = new List<ValuesOut>();

            var vn = string.Empty;

            try
            {
                var fav = key.Values.SingleOrDefault(t => t.ValueName == "Favorites");

                if (fav == null)
                {
                    return;
                }

                var br = new BinaryReader(new MemoryStream(fav.ValueDataRaw));

                var chunks = new List<byte[]>();
                var shellItems = new List<byte[]>();

                var unk = br.ReadByte(); //

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    var size = br.ReadInt32();
                    br.BaseStream.Seek(-4, SeekOrigin.Current);

                    var b = br.ReadBytes(size);

                    chunks.Add(b);

                    var marker = br.ReadBytes(5);
                }

                foreach (var chunk in chunks)
                {
                    var chunkstream = new BinaryReader(new MemoryStream(chunk));

                    var chunksize = chunkstream.ReadInt32();

                    while (chunkstream.BaseStream.Position < chunkstream.BaseStream.Length)
                    {
                        var siSize = chunkstream.ReadInt16();

                        chunkstream.BaseStream.Seek(-2, SeekOrigin.Current);

                        var siBytes = chunkstream.ReadBytes(siSize);

                        shellItems.Add(siBytes);
                    }
                }


                var det = new StringBuilder();

                bags = new List<ShellBag>();


                ShellBag bag = null;

                foreach (var bytese in shellItems)
                {
                    try
                    {
                        switch (bytese[2])
                        {
                            case 0x00:
                                bag = new ShellBag0X00(bytese);

                                break;
                            case 0x1f:
                                bag = new ShellBag0X1F(bytese);

                                break;
                            case 0x2f:
                                bag = new ShellBag0X2F(bytese);

                                break;
                            case 0x2e:
                                bag = new ShellBag0X2E(bytese);

                                break;
                            case 0xb1:
                            case 0x31:
                            case 0x35:
                            case 0x36:
                                bag = new ShellItems.ShellBag0X31(bytese);

                                break;
                            case 0x32:
                                bag = new ShellBag0X32(bytese);

                                break;

                            case 0x3a:
                                bag = new ShellBag0X3A(bytese);
                                break;
                            case 0x71:
                                bag = new ShellBag0X71(bytese);

                                break;
                            case 0x74:
                                bag = new ShellBag0X74(bytese);

                                break;
                            case 0x40:
                                bag = new ShellBag0X40(bytese);

                                break;
                            case 0x61:
                                bag = new ShellBag0X61(bytese);

                                break;
                            case 0xc3:
                                bag = new ShellBag0Xc3(bytese);

                                break;
                            default:
                                det.AppendLine(
                                    $"Key: {key.KeyName}, Value name: {fav.ValueName}, Message: **** Unsupported ShellID: 0x{bytese[2]:x2}. Send this ID to saericzimmerman@gmail.com so support can be added!! ****");

                                Errors.Add(
                                    $"Key: {key.KeyName}, Value name: {fav.ValueName}, Message: **** Unsupported ShellID: 0x{bytese[2]:x2}. Send this ID to saericzimmerman@gmail.com so support can be added!! ****");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Errors.Add($"Error processing Favorites value '{vn}': {e.Message}");
                    }


                    if (bag != null)
                    {
                        det.AppendLine(bag.ToString());
                        bags.Add(bag);
                    }
                }

                foreach (var shellBag in bags)
                {
                    var exe = "(unknown)";
                    var ed = shellBag.ExtensionBlocks.SingleOrDefault(t => t is ExtensionBlocks.Beef001d);

                    if (ed != null)
                    {
                        exe = ((Beef001d) ed).Executable;
                    }

                    var pp = shellBag.ExtensionBlocks.SingleOrDefault(t => t is Beef001e);

                    var pt = "(unknown)";
                    if (pp != null)
                    {
                        pt = ((Beef001e) pp).PinType;
                    }

                    if (shellBag.FriendlyName == "Directory")
                    {
                        exe = "(Directory)";
                        pt = "(Directory)";
                    }

                    var v = new ValuesOut(shellBag.Value,exe,pt);
                    v.BatchKeyPath = key.KeyPath;
                    v.BatchValueName = fav.ValueName;

                    valuesList.Add(v);
                }


            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Favorites value '{vn}': {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }


            var v1 = valuesList;

            foreach (var source in v1.ToList())
            {
                _values.Add(source);
            }
        }

        public IBindingList Values => _values;
    }
}