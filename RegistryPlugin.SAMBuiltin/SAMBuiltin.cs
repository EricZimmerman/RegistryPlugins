using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.SAMBuiltin
{
    public class SAMBuiltin : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public SAMBuiltin()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "34c46f2d-eafe-4575-b34e-4d0309cb2cf8";
        public List<string> KeyPaths => new List<string>(new[]
        {
            @"SAM\Domains\Builtin\Aliases"
        });
        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Hyun Yi @hyuunnn";
        public string Email => "";
        public string Phone => "000-0000-0000";
        public string PluginName => "SAMBuiltin";

        public string ShortDescription
            => "https://github.com/keydet89/RegRipper3.0/blob/master/plugins/samparse.pl";

        public string LongDescription => ShortDescription;
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


        static uint Bitwise(byte[] buffer)
        {
            return ((uint)buffer[0])
                | (((uint)buffer[1]) << 8)
                | (((uint)buffer[2]) << 16)
                | (((uint)buffer[3]) << 24);
        }

        static uint GetIdAuth(byte[] buffer)
        {
            Array.Reverse(buffer);
            return Bitwise(buffer);
        }

        public String GetSID(byte[] rawData, int numUser)
        {
            List<String> result = new List<String>();
            var count = 0;
            for (int i = 0; i < numUser; i++)
            {
                var offset = BitConverter.ToInt32(rawData, 0x28) + 52 + count;
                var tmp = BitConverter.ToInt32(rawData, offset);

                if (tmp == 0x101)
                {
                    if (rawData[offset] == 0)
                        offset += 1;

                    byte[] tmp2 = new byte[12];
                    Array.Copy(rawData, offset, tmp2, 0, 12);
                    var br = new BinaryReader(new MemoryStream(tmp2));

                    var revision = (uint)br.ReadByte();
                    var dashes = (uint)br.ReadByte();
                    var idauth = GetIdAuth(br.ReadBytes(6));
                    var sub = Bitwise(br.ReadBytes(4));
                    result.Add(string.Format("S-{0}-{1}-{2}", revision, idauth, sub));

                    count += 12;
                }
                else if (tmp == 0x501)
                {
                    byte[] tmp2 = new byte[28];
                    Array.Copy(rawData, offset, tmp2, 0, 28);
                    var br = new BinaryReader(new MemoryStream(tmp2));

                    var revision = (uint)br.ReadByte();
                    var dashes = (uint)br.ReadByte();
                    var idauth = GetIdAuth(br.ReadBytes(6));
                    var sub1 = Bitwise(br.ReadBytes(4));
                    var sub2 = Bitwise(br.ReadBytes(4));
                    var sub3 = Bitwise(br.ReadBytes(4));
                    var sub4 = Bitwise(br.ReadBytes(4));
                    var rid = Bitwise(br.ReadBytes(4));
                    result.Add(string.Format("S-{0}-{1}-{2}-{3}-{4}-{5}-{6}", revision, idauth, sub1, sub2, sub3, sub4, rid));

                    count += 28;
                }
            }
            return string.Join(", ", result);

        }

        private IEnumerable<ValuesOut> ProcessKey(RegistryKey key)
        {
            var l = new List<ValuesOut>();
            foreach (var registryKey in key.SubKeys)
            {
                try
                {
                    var cVal = registryKey.Values.SingleOrDefault(t => t.ValueName == "C");
                    if (cVal != null)
                    {
                        var cValRaw = cVal.ValueDataRaw;
                        var groupNameOfset = BitConverter.ToInt32(cValRaw, 0x10);
                        var groupnNameLength = BitConverter.ToInt32(cValRaw, 0x14);

                        var commentOffset = BitConverter.ToInt32(cValRaw, 0x1c);
                        var commentLength = BitConverter.ToInt32(cValRaw, 0x20);

                        var groupName = Encoding.Unicode.GetString(cValRaw, groupNameOfset + 0x34, groupnNameLength);
                        var comment = Encoding.Unicode.GetString(cValRaw, commentOffset + 0x34, commentLength);

                        var numUser = BitConverter.ToInt32(cValRaw, 0x30);

                        var Users = GetSID(cValRaw, numUser);

                        var ff = new ValuesOut(groupName, comment, Users)
                        {
                            BatchValueName = "Multiple",
                            BatchKeyPath = key.KeyPath
                        };
                        l.Add(ff);
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing SAMBuiltin: {ex.Message}");
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
