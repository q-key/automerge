using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionControl.Merge;

namespace VersionControl
{
    public class MergeResult<T>
    {
        public List<Conflict> Conflicts = new List<Conflict>();
        public List<T> Lines;
        internal int Offset;
        public string Text { get { return string.Join("\n", Lines); } }
    }
}
