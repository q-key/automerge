using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl.Diff
{
    public abstract class DiffBase<T>
    {
        private readonly IComparer<T> _comparer;

        protected DiffBase(IComparer<T> comparer)
        {
            this._comparer = comparer;
        }

        protected bool AreEquel(T s1, T s2)
        {
            return _comparer.Compare(s1, s2) == 0;
        }

        protected delegate IEnumerable<Command<T>> GetCommandsDelegate(int versionIndex);
        protected static List<Command<T>> GetCommandsInParallel(int versionsCount, GetCommandsDelegate d)
        {
            var lockObj = new object();

            //materialized to avoid executing the whole algo
            var result = new List<Command<T>>();

            //make it parallel not to slow down when have many versions to compare
            Parallel.For<List<Command<T>>>(0, versionsCount,

             () => new List<Command<T>>(),

             (versionIndex, state, previousCommands) =>
             {
                 var diffCommands = d(versionIndex);
                 previousCommands.AddRange(diffCommands);
                 return previousCommands;
             },

             (localList) => { lock (lockObj) result.AddRange(localList); }
            );

            return result;
        }

    }
}
