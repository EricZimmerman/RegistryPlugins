using System;

namespace RegistryPlugin.SAM
{
    public class UserOut
    {
        public UserOut(int userId, int invalidLoginCount, int totalLoginCount, DateTimeOffset? lastLogin,
            DateTimeOffset? lastPwChange, DateTimeOffset? lastIncorrectLogin, DateTimeOffset? expiresOn,
            string username,
            string fullName, string comment, string userComment, string homeDir, DateTimeOffset createdOn,
            string groups, string pwHint, bool passwordPolicyRequired)
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
            PasswordPolicyRequired = passwordPolicyRequired;
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
        public bool PasswordPolicyRequired { get; }
    }
}
