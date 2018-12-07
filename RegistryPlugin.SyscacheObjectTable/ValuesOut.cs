using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.SyscacheObjectTable
{
   public class ValuesOut
    {

        public ValuesOut(long objectId, long usn, long usnJournalId, int mftEntryNumber, int mftSequenceNumber, string sha1, string aeProgramId, long objectLru, DateTimeOffset lastWriteTime, string keyPath)
        {
            ObjectId = objectId;
            Usn = usn;
            UsnJournalId = usnJournalId;
            MftEntryNumber = mftEntryNumber;
            MftSequenceNumber = mftSequenceNumber;
            Sha1 = sha1;
            AeProgramId = aeProgramId;
            ObjectLru = objectLru;
            LastWriteTime = lastWriteTime;
            KeyPath = keyPath;
        }

        
        public string KeyPath { get; }
        public long ObjectId { get; }
        public long ObjectLru { get; }
        public long Usn { get; }
        public long UsnJournalId { get; }
        public int MftEntryNumber { get; }
        public int MftSequenceNumber { get; }

        public string Sha1 { get; }
        public string AeProgramId { get; }
        public DateTimeOffset LastWriteTime { get; }
        
    }
}
