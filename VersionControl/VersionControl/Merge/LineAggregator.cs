using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VersionControl.Merge
{
    public class LineAggregator<T>
    {
        public int LineNumber;
        public RemoveCommand<T>[] RemoveCommands;
        public InsertCommand<T>[] InsertCommands;
    }
}
