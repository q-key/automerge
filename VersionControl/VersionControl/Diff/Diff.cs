using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionControl.Diff;

namespace VersionControl.Diff
{
    public class Diff<T> : DiffBase<T>
    {

        public Diff(IComparer<T> comparer)
            : base(comparer)
        {
        }



        /// <summary>
        /// Get diff between each version and original file
        /// </summary>
        public List<Command<T>> GetDiffFast(T[] originalLines, T[][] versions)
        {
            return GetCommandsInParallel(versions.Length, (versionIndex) => GetDiff("version" + versionIndex, originalLines, versions[versionIndex]));
        }

        public IEnumerable<Command<T>> GetDiff(T[] originalLines, T[][] versions)
        {
            for (var versionIndex = 0; versionIndex < versions.Length; versionIndex++)
            {
                var diffCommands = GetDiff("version" + versionIndex, originalLines, versions[versionIndex]);
                foreach (var cmd in diffCommands)
                    yield return cmd;
            }
        }


        public IEnumerable<Command<T>> GetDiff(string version, T[] srcLines, T[] dstLines)
        {
            var dstIndex = 0;

            for (var srcIndex = 0; srcIndex < srcLines.Length; srcIndex++)
            {
                var original = srcLines[srcIndex];

                if (dstIndex >= dstLines.Length)
                {
                    yield return new RemoveCommand<T>(version, srcIndex);
                    continue;
                }

                //find oldLine? if found next line - it was removed   
                var srcLineIsRemoved = false;
                var insertionStartedIn = dstIndex;

                for (var i = dstIndex; i < dstLines.Length; i++)
                {
                    var nextNewLine = dstLines[i];
                    if (!AreEquel(original, nextNewLine))
                    {
                        srcLineIsRemoved = true;
                    }
                    else
                    {
                        srcLineIsRemoved = false;
                        dstIndex = i;
                        dstIndex++;//for the next iteration

                        for (var tmpIndex = insertionStartedIn; tmpIndex < i; tmpIndex++)
                            yield return new InsertCommand<T>(version, srcIndex + (tmpIndex - i), dstLines[tmpIndex]);

                        break;
                    }
                }

                if (srcLineIsRemoved)
                {
                    yield return new RemoveCommand<T>(version, srcIndex);
                }
            }

            //these line from the end of new version doesn't exist in the original file
            for (; dstIndex < dstLines.Length; dstIndex++)
                yield return new AppendLineCommand<T>(version, dstLines[dstIndex]);

        }

    }
}
