using System;

namespace RegistryPlugin.SAM
{
    public class UserOut
    {
        public UserOut(int userId, int invalidLoginCount, int totalLoginCount, DateTimeOffset? lastLogin,
            DateTimeOffset? lastPwChange, DateTimeOffset? lastIncorrectLogin, DateTimeOffset? expiresOn, string username,
            string fullName, string comment, string userComment, string homeDir, DateTimeOffset createdOn)
        {
            UserId = userId;
            InvalidLoginCount = invalidLoginCount;
            TotalLoginCount = totalLoginCount;
            LastLoginTime = lastLogin;
            LastPasswordChange = lastPwChange;
            LastIncorrectPassword = lastIncorrectLogin;
            ExpiresOn = expiresOn;
            UserName = username;
            FullName = fullName;
            Comment = comment;
            UserComment = userComment;
            HomeDirectory = homeDir;
            CreatedOn = createdOn;
        }

        public int UserId { get; }

        /// <summary>
        ///     Current number of invalid login attempts
        ///     <remarks>Resets to 0 on successful login</remarks>
        /// </summary>
        public int InvalidLoginCount { get; }

        public int TotalLoginCount { get; }

        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset? LastLoginTime { get; }
        public DateTimeOffset? LastPasswordChange { get; }
        public DateTimeOffset? LastIncorrectPassword { get; }
        public DateTimeOffset? ExpiresOn { get; }

        public string UserName { get; }
        public string FullName { get; }
        public string Comment { get; }
        public string UserComment { get; }
        public string HomeDirectory { get; }
    }
}