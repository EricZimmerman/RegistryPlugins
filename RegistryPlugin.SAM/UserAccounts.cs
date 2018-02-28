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
    ///<summary>SAM Account flags</summary>
    [Flags]
    public enum AccountFlags
    {
        ///<summary>Default value (no flags)</summary>
        None                    = 0x0000,
        ///<summary>Account is disabled</summary>
        AccountDisabled         = 0x0001,
        ///<summary>The home directory is required.</summary>
        HomeDirectoryRequired   = 0x0002,
        ///<summary>No password is required.</summary>
        PasswordNotRequired     = 0x0004,
        ///<summary>This is an account for users whose primary account is in another domain. This account provides user access to this domain, but not to any domain that trusts this domain. Also known as a local user account.</summary>
        TempDuplicateAccount    = 0x0008,
        ///<summary>This is a default account type that represents a typical user.</summary>
        NormalUserAccount       = 0x0010,
        ///<summary>This is an Majority Node Set (MNS) logon account. With MNS, you can configure a multi-node Windows cluster without using a common shared disk.</summary>
        MnsLogonAccount         = 0x0020,
        ///<summary>This is a permit to trust account for a system domain that trusts other domains.</summary>
        InterdomainTrustAccount = 0x0040,
        ///<summary>This is a computer account for a Windows or Windows Server that is a member of this domain.</summary>
        WorkstationTrustAccount = 0x0080,
        ///<summary>This is a computer account for a system backup domain controller that is a member of this domain.</summary>
        ServerTrustAccount      = 0x0100,
        ///<summary>The password will not expire on this account.</summary>
        PasswordDoesNotExpire   = 0x0200,
        ///<summary>Account auto locked (I'm not entirely sure what this means)</summary>
        AutoLocked              = 0x0400
    }

    public class UserAccounts : IRegistryPluginGrid
    {
        private readonly BindingList<UserOut> _values;

        public UserAccounts()
        {
            _values = new BindingList<UserOut>();

            Errors = new List<string>();

            Groups = new List<GroupInfo>();
        }

        private List<GroupInfo> Groups { get; }

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
            => String.Join(
            Environment.NewLine,
            "See the following URLs for more",
            "http://www.beginningtoseethelight.org/ntsecurity/index.htm has details on SAM layout,",
            "https://windowsir.blogspot.com/2009/07/user-account-analysis.html further explains the 'Password not required' flag,"
        );

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();
            Groups.Clear();

            var namesKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Names");

            var nameMap = new Dictionary<int, DateTimeOffset>();

            if (namesKey == null)
            {
                return;
            }

            GetGroups(key);

            foreach (var registryKey in namesKey.SubKeys)
            {
                if (registryKey.Values.Count == 0)
                {
                    continue;
                }
                if (nameMap.ContainsKey((int) registryKey.Values.First().VkRecord.DataTypeRaw))
                {
                    continue;
                }
                nameMap.Add((int) registryKey.Values.First().VkRecord.DataTypeRaw, registryKey.LastWriteTime.Value);
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
                    var parsedAccountFlags = AccountFlags.None;

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

                        if (fVal.ValueDataRaw.Length >= 0x56)
                        {
                            parsedAccountFlags = (AccountFlags)BitConverter.ToInt16(fVal.ValueDataRaw, 0x56);
                        }
                        
                    }

                    var vVal = key1.Values.SingleOrDefault(t => t.ValueName == "V");

                    if (vVal != null)
                    {
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
                        var userComment =
                            Encoding.Unicode.GetString(vVal.ValueDataRaw, offToUserComment, userCommentLen);

                        var offHomeDir = BitConverter.ToInt32(vVal.ValueDataRaw, 0x48) + 0xCC;
                        var homeDirLen = BitConverter.ToInt32(vVal.ValueDataRaw, 0x48 + 4);
                        var homeDir = Encoding.Unicode.GetString(vVal.ValueDataRaw, offHomeDir, homeDirLen);

                        var createdOn = nameMap[userId];

                        var groups = GetGroupsForUser(userId);

                        var hint = string.Empty;

                        var hintVal = key1.Values.SingleOrDefault(t => t.ValueName == "UserPasswordHint");

                        if (hintVal != null)
                        {
                            hint = Encoding.Unicode.GetString(hintVal.ValueDataRaw);
                        }


                        var u = new UserOut(userId, invalidLogins, totalLogins, lastLoginTime, lastPwChangeTime,
                            lastIncorrectPwTime, acctExpiresTime, name1, full1, comment, userComment, homeDir,
                            createdOn, groups, hint, parsedAccountFlags);

                        _values.Add(u);
                    }
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

        private void GetGroups(RegistryKey baseKey)
        {
            var btKey = baseKey.Parent.Parent.SubKeys.SingleOrDefault(t => t.KeyName == "Builtin");
            var aliasesKey = btKey.SubKeys.SingleOrDefault(t => t.KeyName == "Aliases");

            foreach (var aliasesKeySubKey in aliasesKey.SubKeys)
            {
                var cVal = aliasesKeySubKey.Values.SingleOrDefault(t => t.ValueName == "C");

                if (cVal == null)
                {
                    continue;
                }

                var offsetToGroupName = BitConverter.ToInt32(cVal.ValueDataRaw, 0x10) + 0x34; //add header len
                var nameLen = BitConverter.ToInt32(cVal.ValueDataRaw, 0x14);


                var offsetToGroupDesc = BitConverter.ToInt32(cVal.ValueDataRaw, 0x1C) + 0x34; //add header len
                var descLen = BitConverter.ToInt32(cVal.ValueDataRaw, 0x20);

                var offsetToUsers = BitConverter.ToInt32(cVal.ValueDataRaw, 0x28) + 0x34; //add header len
                var userLen = BitConverter.ToInt32(cVal.ValueDataRaw, 0x2C);
                var userCount = BitConverter.ToInt32(cVal.ValueDataRaw, 0x30);

                if (userCount == 0)
                {
                    continue;
                }

                var groupName = Encoding.Unicode.GetString(cVal.ValueDataRaw, offsetToGroupName, nameLen);
                var desc = Encoding.Unicode.GetString(cVal.ValueDataRaw, offsetToGroupDesc, descLen);

                var index = offsetToUsers;

                var newg = new GroupInfo(groupName, desc, userCount);


                for (var i = 0; i < userCount; i++)
                {
                    var sidType = BitConverter.ToInt16(cVal.ValueDataRaw, index);

                    byte[] buff = null;
                    var sid = string.Empty;

                    switch (sidType)
                    {
                        case 0x501:
                            buff = new byte[0x1c];
                            Buffer.BlockCopy(cVal.ValueDataRaw, index, buff, 0, 0x1c);

                            sid = ConvertHexStringToSidString(buff);


                            index += 0x1c;
                            break;

                        case 0x101:
                            buff = new byte[0xc];
                            Buffer.BlockCopy(cVal.ValueDataRaw, index, buff, 0, 0xc);

                            sid = ConvertHexStringToSidString(buff);

                            index += 0xc;
                            break;
                    }

                    newg.Sids.Add(sid);
                }


                Groups.Add(newg);
            }
        }

        public string ConvertHexStringToSidString(byte[] hex)
        {
            const string header = "S";

            var sidVersion = hex[0].ToString();

            var authId = BitConverter.ToInt32(hex.Skip(4).Take(4).Reverse().ToArray(), 0);

            var index = 8;

            var sid = $"{header}-{sidVersion}-{authId}";

            do
            {
                var tempAuthHex = hex.Skip(index).Take(4).ToArray();

                var tempAuth = BitConverter.ToUInt32(tempAuthHex, 0);

                index += 4;

                sid = $"{sid}-{tempAuth}";
            } while (index < hex.Length);


            return sid;
        }

        private string GetGroupsForUser(int userId)
        {
            var groups = new List<string>();

            foreach (var groupInfo in Groups)
            {
                foreach (var groupInfoSid in groupInfo.Sids)
                {
                    if (groupInfoSid.EndsWith(userId.ToString()))
                    {
                        groups.Add(groupInfo.Name);
                    }
                }
            }

            return string.Join(", ", groups);
        }
    }

    public class GroupInfo
    {
        public GroupInfo(string name, string desc, int memberCount)
        {
            Sids = new List<string>();

            Name = name;
            Description = desc;
            MemberCount = memberCount;
        }

        public string Name { get; }
        public string Description { get; }
        public int MemberCount { get; }

        public List<string> Sids { get; }
    }
}
