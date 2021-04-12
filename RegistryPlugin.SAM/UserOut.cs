using System;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.SAM
{
    public class UserOut:IValueOut
    {
        public UserOut(int userId, int invalidLoginCount, int totalLoginCount, DateTimeOffset? lastLogin,
            DateTimeOffset? lastPwChange, DateTimeOffset? lastIncorrectLogin, DateTimeOffset? expiresOn,
            string username,
            string fullName, string comment, string userComment, string homeDir, DateTimeOffset createdOn,
            string groups, string pwHint, AccountFlags parsedAccountFlags, string internetUserName)
        {
            UserId = userId;
            InvalidLoginCount = invalidLoginCount;
            TotalLoginCount = totalLoginCount;
            LastLoginTime = lastLogin?.UtcDateTime;
            LastPasswordChange = lastPwChange?.UtcDateTime;
            LastIncorrectPassword = lastIncorrectLogin?.UtcDateTime;
            ExpiresOn = expiresOn?.UtcDateTime;
            UserName = username;
            FullName = fullName;
            Comment = comment;
            UserComment = userComment;
            HomeDirectory = homeDir;
            CreatedOn = createdOn.UtcDateTime;
            Groups = groups;
            PasswordHint = pwHint;
            AccountFlagsEnum = parsedAccountFlags;
            InternetUserName = internetUserName;
        }

        public int UserId { get; }

        /// <summary>
        ///     Current number of invalid login attempts
        ///     <remarks>Resets to 0 on successful login</remarks>
        /// </summary>
        public int InvalidLoginCount { get; }

        public int TotalLoginCount { get; }

        public DateTime CreatedOn { get; }
        public DateTime? LastLoginTime { get; }
        public DateTime? LastPasswordChange { get; }
        public DateTime? LastIncorrectPassword { get; }
        public DateTime? ExpiresOn { get; }

        public string UserName { get; }
        public string FullName { get; }
        public string PasswordHint { get; }
        public string Groups { get; }
        public string Comment { get; }
        public string UserComment { get; }
        public string HomeDirectory { get; }
        public string InternetUserName { get; }

        private AccountFlags AccountFlagsEnum { get; }
        public bool AccountDisabled         => HasFlag(AccountFlags.AccountDisabled);
        public bool HomeDirectoryRequired   => HasFlag(AccountFlags.HomeDirectoryRequired);
        public bool PasswordNotRequired     => HasFlag(AccountFlags.PasswordNotRequired);
        public bool TempDuplicateAccount    => HasFlag(AccountFlags.TempDuplicateAccount);
        public bool NormalUserAccount       => HasFlag(AccountFlags.NormalUserAccount);
        public bool MnsLogonAccount         => HasFlag(AccountFlags.MnsLogonAccount);
        public bool InterdomainTrustAccount => HasFlag(AccountFlags.InterdomainTrustAccount);
        public bool WorkstationTrustAccount => HasFlag(AccountFlags.WorkstationTrustAccount);
        public bool ServerTrustAccount      => HasFlag(AccountFlags.ServerTrustAccount);
        public bool PasswordDoesNotExpire   => HasFlag(AccountFlags.PasswordDoesNotExpire);
        public bool AutoLocked              => HasFlag(AccountFlags.AutoLocked);

        private bool HasFlag(AccountFlags flag)
        {
            return ((AccountFlagsEnum & flag) == flag);
        }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Username: {UserName} Id: {UserId}";
        public string BatchValueData2 => $"Created: {CreatedOn.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} Last login: {LastLoginTime?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Account flags: {AccountFlagsEnum}";
    }
}
