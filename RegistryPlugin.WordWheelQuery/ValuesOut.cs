using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.WordWheelQuery
{
    public class ValuesOut
    {

        public ValuesOut(string searchTerm, int mruPosition)
        {
            SearchTerm = searchTerm;
            MruPosition = mruPosition;
        }

        public string SearchTerm { get; }

        public int MruPosition { get; }
    }
}
