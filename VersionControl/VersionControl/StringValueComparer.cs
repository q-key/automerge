using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl
{
    public class StringValueComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return String.Compare(x.Trim(), y.Trim(), StringComparison.InvariantCulture);
        }
    }
}
