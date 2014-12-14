using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl.Diff
{
    public class FileDiff : DiffBase<string>
    {
        public FileDiff()
            : base(new StringValueComparer())
        {
        }

        /// <summary>
        /// Get diff between each version and original file
        /// </summary>
        public List<Command<string>> GetDiffFast(string originalPath, string[] versionsPaths)
        {
            return GetCommandsInParallel(versionsPaths.Length, (versionIndex) => GetDiff(originalPath, versionsPaths, versionIndex));
        }

        private List<Command<string>> GetDiff(string originalPath, string[] versionsPaths, int versionIndex)
        {
            var versionPath = versionsPaths[versionIndex];
            var dst = File.OpenRead(versionPath);
            var src = File.OpenRead(originalPath);

            var diffCommands = GetFileDiff("version" + versionIndex, src, dst).ToList();//remove toList
            return diffCommands;
        }

        public IEnumerable<Command<string>> GetDiff(string originalPath, string[] versionsPaths)
        {
            for (var versionIndex = 0; versionIndex < versionsPaths.Length; versionIndex++)
            {
                var versionPath = versionsPaths[versionIndex];
                var dst = File.OpenRead(versionPath);
                var src = File.OpenRead(originalPath);

                var diffCommands = GetFileDiff("version" + versionIndex, src, dst).ToList();
                foreach (var cmd in diffCommands)
                    yield return cmd;
            }
        }

        public IEnumerable<Command<string>> GetFileDiff(string version, FileStream src, FileStream dst)
        {
            var dstIndex = 0;
            var srcIndex = -1;

            while (!StreamReader.IsEnd(src))
            {
                srcIndex++;

                var original = StreamReader.ReadLine(src);

                if (StreamReader.IsEnd(dst))
                {
                    yield return new RemoveCommand<string>(version, srcIndex);
                    continue;
                }

                //find oldLine? if found next line - it was removed   
                var srcLineIsRemoved = false;
                var insertionStartedIn = dstIndex;

                var start = dst.Position;

                var i = dstIndex;
                while (!StreamReader.IsEnd(dst))
                {
                    var nextNewLine = StreamReader.ReadLine(dst);

                    if (!AreEquel(original, nextNewLine))
                    {
                        srcLineIsRemoved = true;
                    }
                    else
                    {
                        srcLineIsRemoved = false;
                        dstIndex = i;
                        dstIndex++;//for the next iteration

                        //reset Position for the next iteration
                        dst.Position = start;

                        for (var tmpIndex = insertionStartedIn; tmpIndex < i; tmpIndex++)
                            yield return new InsertCommand<string>(version, srcIndex + (tmpIndex - i), StreamReader.ReadLine(dst));

                        StreamReader.ReadLine(dst);//next offset

                        break;
                    }

                    i++;
                }

                if (srcLineIsRemoved)
                {
                    //reset Position
                    dst.Position = start;
                    yield return new RemoveCommand<string>(version, srcIndex);
                }
            }

            //these line from the end of new version doesn't exist in the original file
            while (!StreamReader.IsEnd(dst))
                yield return new AppendLineCommand<string>(version, StreamReader.ReadLine(dst));
        }


        //these are not up to SRP - move to a class:

        static  class StreamReader
        {
            public static string ReadLine(FileStream fs)
            {
                var sb = new StringBuilder();

                while (!IsEnd(fs))
                {
                    var b = fs.ReadByte();
                    if (b == '\r') continue;
                    if (b == '\n') break;

                    sb.Append((char)b);
                }

                var result = sb.ToString();
                return result;
            }

            public static bool IsEnd(FileStream fs)
            {
                return fs.Position == fs.Length;
            }
        }
    }
}
