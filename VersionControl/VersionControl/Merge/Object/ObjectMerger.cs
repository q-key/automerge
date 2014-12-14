using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionControl.Diff;

namespace VersionControl.Merge.Object
{
    //todo: it can be used to merge lists of objects

    public class ObjectMerger<T> : MergerBase<T>
    {
        protected override bool IsEmptyValue(T value)
        {
            return value == null;
        }

        public ObjectMerger(IComparer<T> comparer)
            : base(comparer)
        { }

        public void MergeLines(T[] originalLines, T[][] versions, MergeResult<T> context)
        {
            var diff = new Diff<T>(Comparer);
            var allCommands = diff.GetDiffFast(originalLines, versions);

            Merge(context, allCommands);
        }


    }
}
