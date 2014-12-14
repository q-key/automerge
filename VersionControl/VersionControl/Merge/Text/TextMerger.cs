using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionControl.Diff;

namespace VersionControl.Merge.Text
{

    public class TextMerger : MergerBase<string>
    {
        public TextMerger()
            : base(new StringValueComparer())
        {
        }

        public MergeResult<string> Merge(string original, params string[] versions)
        {
            var linesOfVersions = versions.Select(v => Split(v).ToArray()).ToArray();
            var linesOfOriginal = Split(original).ToArray();
            var result = Merge(linesOfOriginal, linesOfVersions);
            return result;
        }

        public MergeResult<string> Merge(string[] originalLines, params string[][] versions)
        {
            var context = new MergeResult<string>
            {
                Lines = originalLines.ToList()
            };

            MergeLines(originalLines, versions, context);
            return context;
        }



        protected void MergeLines(IEnumerable<string> originalLines, IEnumerable<IEnumerable<string>> versions, MergeResult<string> context)
        {
            var diff = new Diff<string>(Comparer);

            var allCommands = diff.GetDiffFast(originalLines.ToArray(), versions as string[][]).ToList();
            Merge(context, allCommands);
        }

        protected override bool IsEmptyValue(string value)
        {
            return string.IsNullOrWhiteSpace(value.Trim());
        }


        private const char splitKey = '\n';
        private static IEnumerable<string> Split(string s)
        {
            return s.Split(splitKey);
        }
    }
}
