using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.SAM
{
    public class UserAccounts : IRegistryPluginGrid
    {
        private readonly BindingList<UserOut> _values;

        public UserAccounts()
        {
            _values = new BindingList<UserOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "8702e82b-e2fb-4c6c-b392-bd524e41c6c7";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"SAM\Domains\Account\Users"
        });

        //TODO  @"SAM\Domains\Builtin"

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "User accounts";
        public string ShortDescription => "Displays user accounts and user account details";

        public string LongDescription
            =>
            "http://www.beginningtoseethelight.org/ntsecurity/index.htm has details on SAM layout"
            ;

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            var namesKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Names");

            var nameMap = new Dictionary<int, DateTimeOffset>();

            if (namesKey == null)
            {
                return;
            }

            foreach (var registryKey in namesKey.SubKeys)
            {
                if (nameMap.ContainsKey((int) registryKey.Values.First().VKRecord.DataTypeRaw))
                {
                    continue;
                }
                nameMap.Add((int) registryKey.Values.First().VKRecord.DataTypeRaw, registryKey.LastWriteTime.Value);
            }

            foreach (var key1 in key.SubKeys)
            {
                if (key1.KeyName == "Names")
                {
                    continue;
                }

                try
                {
                    var fVal = key1.Values.SingleOrDefault(t => t.ValueName == "F");

                    var userId = 0;
                    var invalidLogins = 0;
                    var totalLogins = 0;
                    DateTimeOffset? lastLoginTime = null;
                    DateTimeOffset? lastPwChangeTime = null;
                    DateTimeOffset? acctExpiresTime = null;
                    DateTimeOffset? lastIncorrectPwTime = null;

                    if (fVal != null)
                    {
                        userId = BitConverter.ToInt32(fVal.ValueDataRaw, 0x30);
                        invalidLogins = BitConverter.ToInt16(fVal.ValueDataRaw, 0x40);
                        totalLogins = BitConverter.ToInt16(fVal.ValueDataRaw, 0x42);

                        var tempTime = DateTimeOffset.FromFileTime(BitConverter.ToInt64(fVal.ValueDataRaw, 0x8));
                        if (tempTime.Year > 1700)
                        {
                            lastLoginTime = tempTime.ToUniversalTime();
                        }

                        tempTime = DateTimeOffset.FromFileTime(BitConverter.ToInt64(fVal.ValueDataRaw, 0x18));
                        if (tempTime.Year > 1700)
                        {
                            lastPwChangeTime = tempTime.ToUniversalTime();
                        }

                        tempTime = DateTimeOffset.MinValue;

                        try
                        {
                            tempTime = DateTimeOffset.FromFileTime(BitConverter.ToInt64(fVal.ValueDataRaw, 0x20));
                        }
                        catch (Exception)
                        {
                        }

                        if (tempTime.Year > 1700)
                        {
                            acctExpiresTime = tempTime.ToUniversalTime();
                        }

                        tempTime = DateTimeOffset.FromFileTime(BitConverter.ToInt64(fVal.ValueDataRaw, 0x28));
                        if (tempTime.Year > 1700)
                        {
                            lastIncorrectPwTime = tempTime.ToUniversalTime();
                        }
                    }

                    var vVal = key1.Values.SingleOrDefault(t => t.ValueName == "V");

                    var offToName = BitConverter.ToInt32(vVal.ValueDataRaw, 0xc) + 0xCC;
                    var nameLen = BitConverter.ToInt32(vVal.ValueDataRaw, 0xc + 4);
                    var name1 = Encoding.Unicode.GetString(vVal.ValueDataRaw, offToName, nameLen);

                    var offToFull = BitConverter.ToInt32(vVal.ValueDataRaw, 0x18) + 0xCC;
                    var fullLen = BitConverter.ToInt32(vVal.ValueDataRaw, 0x18 + 4);
                    var full1 = Encoding.Unicode.GetString(vVal.ValueDataRaw, offToFull, fullLen);

                    var offToComment = BitConverter.ToInt32(vVal.ValueDataRaw, 0x24) + 0xCC;
                    var commentLen = BitConverter.ToInt32(vVal.ValueDataRaw, 0x24 + 4);
                    var comment = Encoding.Unicode.GetString(vVal.ValueDataRaw, offToComment, commentLen);

                    var offToUserComment = BitConverter.ToInt32(vVal.ValueDataRaw, 0x30) + 0xCC;
                    var userCommentLen = BitConverter.ToInt32(vVal.ValueDataRaw, 0x30 + 4);
                    var userComment = Encoding.Unicode.GetString(vVal.ValueDataRaw, offToUserComment, userCommentLen);

                    var offHomeDir = BitConverter.ToInt32(vVal.ValueDataRaw, 0x48) + 0xCC;
                    var homeDirLen = BitConverter.ToInt32(vVal.ValueDataRaw, 0x48 + 4);
                    var homeDir = Encoding.Unicode.GetString(vVal.ValueDataRaw, offHomeDir, homeDirLen);

                    var createdOn = nameMap[userId];

                    var u = new UserOut(userId, invalidLogins, totalLogins, lastLoginTime, lastPwChangeTime,
                        lastIncorrectPwTime, acctExpiresTime, name1, full1, comment, userComment, homeDir, createdOn);

                    _values.Add(u);
                }
                catch (Exception ex)
                {
                    Errors.Add($"Error processing user account: {ex.Message}");
                }

                if (Errors.Count > 0)
                {
                    AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
                }
            }
        }

        public IBindingList Values => _values;
    }
}