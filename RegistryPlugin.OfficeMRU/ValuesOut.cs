using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.OfficeMRU
{
  public  class ValuesOut
    {

        public ValuesOut(string valueName, DateTimeOffset firstOpened, DateTimeOffset? lastOpened, string fileName)
        {
            ValueName = valueName;
            LastOpened = lastOpened;
            FirstOpened = firstOpened;
            FileName = fileName;
        }
        public string ValueName { get; }
        public DateTimeOffset FirstOpened { get; }
        public DateTimeOffset? LastOpened { get; }
        public string FileName { get; }
    }
}
