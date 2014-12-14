using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionControl.Merge.Text;

namespace VersionControl.Merge.File
{
    public class TextFileMerger : MergerBase<string>
    {
        public TextFileMerger()
            : base(new StringValueComparer())
        {
        }

        public MergeResult<string> Merge(string originalFilePath, params string[] versionPaths)
        {
            var context = new MergeResult<string>
            {
                Lines = System.IO.File.ReadAllLines(originalFilePath).ToList()             
            };

            var diff = new Diff.FileDiff();
            var allCommands = diff.GetDiffFast(originalFilePath, versionPaths);

            Merge(context, allCommands);

            return context;
        }

        protected override bool IsEmptyValue(string value)
        {
            return string.IsNullOrWhiteSpace(value.Trim());
        }
    }
}
