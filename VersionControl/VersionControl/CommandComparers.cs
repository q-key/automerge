
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl
{
    class InsertCommandComparer<T> : IEqualityComparer<InsertCommand<T>>
    {
        private readonly IComparer<T> _comparer;

        public InsertCommandComparer(IComparer<T> comparer)
        {
            this._comparer = comparer;
        }

        public bool Equals(InsertCommand<T> x, InsertCommand<T> y)
        {
            return x.LineNumber == y.LineNumber && _comparer.Compare(x.Value, y.Value) == 0;
        }

        public int GetHashCode(InsertCommand<T> obj)
        {
            return (obj.LineNumber + obj.Value.GetHashCode()).GetHashCode();
        }
    }

    class RemoveCommandComparer<T> : IEqualityComparer<RemoveCommand<T>>
    {
        public bool Equals(RemoveCommand<T> x, RemoveCommand<T> y)
        {
            return x.LineNumber == y.LineNumber;
        }

        public int GetHashCode(RemoveCommand<T> obj)
        {
            return (obj.LineNumber).GetHashCode();
        }
    }
}
