using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl.Merge
{
    public class Conflict
    {
        public int LineNumber;
        public string[] Versions;
    }
}
