using System;

namespace RegistryPlugin.RecentDocs
{
    public class RecentDoc
    {
        public RecentDoc(int mruPos, string valueName, string targetName, ulong? mftEntry, int? mftseq, string mftInfo,
            DateTimeOffset? created, DateTimeOffset? lastAccess, string lnkName, string extension,
            DateTimeOffset? openedOn, DateTimeOffset? extensionLastOpened)
        {
            LnkName = lnkName;
            MruPosition = mruPos;
            ValueName = valueName;
            TargetName = targetName;
            //     MFTEntryNumber = mftEntry;
            //      MFTSequenceNumber = mftseq;
            //     MFTInfo = mftInfo;
            //      CreatedOn = created;
            //      LastAccess = lastAccess;
            //     LastAccess = lastAccess;
            Extension = extension;
            OpenedOn = openedOn?.UtcDateTime;
            ExtensionLastOpened = extensionLastOpened?.UtcDateTime;
        }

        public string Extension { get; }
        public string ValueName { get; }
        public string TargetName { get; }
        public string LnkName { get; }
        public int MruPosition { get; }

        public DateTime? OpenedOn { get; }
        public DateTime? ExtensionLastOpened { get; }

        //       public ulong? MFTEntryNumber { get; }

        //     public int? MFTSequenceNumber { get; }
        //      public string MFTInfo { get; }

        //      public DateTimeOffset? CreatedOn { get; }
        //       public DateTimeOffset? LastAccess { get; }
    }
}