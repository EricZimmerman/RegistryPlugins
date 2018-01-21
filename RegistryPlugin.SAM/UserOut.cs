using System;

namespace RegistryPlugin.SAM
{
    public class UserOut
    {
        public UserOut(int userId, int invalidLoginCount, int totalLoginCount, DateTimeOffset? lastLogin,
            DateTimeOffset? lastPwChange, DateTimeOffset? lastIncorrectLogin, DateTimeOffset? expiresOn,
            string username,
            string fullName, string comment, string userComment, string homeDir, DateTimeOffset createdOn,
            string groups, string pwHint, AccountFlags parsedAccountFlags)
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

        private AccountFlags AccountFlagsEnum { get; }
        public bool AccountDisabled         => this.HasFlag(AccountFlags.AccountDisabled);
        public bool HomeDirectoryRequired   => this.HasFlag(AccountFlags.HomeDirectoryRequired);
        public bool PasswordNotRequired     => this.HasFlag(AccountFlags.PasswordNotRequired);
        public bool TempDuplicateAccount    => this.HasFlag(AccountFlags.TempDuplicateAccount);
        public bool NormalUserAccount       => this.HasFlag(AccountFlags.NormalUserAccount);
        public bool MnsLogonAccount         => this.HasFlag(AccountFlags.MnsLogonAccount);
        public bool InterdomainTrustAccount => this.HasFlag(AccountFlags.InterdomainTrustAccount);
        public bool WorkstationTrustAccount => this.HasFlag(AccountFlags.WorkstationTrustAccount);
        public bool ServerTrustAccount      => this.HasFlag(AccountFlags.ServerTrustAccount);
        public bool PasswordDoesNotExpire   => this.HasFlag(AccountFlags.PasswordDoesNotExpire);
        public bool AutoLocked              => this.HasFlag(AccountFlags.AutoLocked);

        // old way using builtin Enum.HasFlags() --> Too slow!
        //public bool AccountDisabled { 
        //    get {
        //        return AccountFlagsEnum.HasFlag(AccountFlags.AccountDisabled);
        //    }
        //}

        private bool HasFlag(AccountFlags flag)
        {
            return ((AccountFlagsEnum & flag) == flag);
        }
    }
}
