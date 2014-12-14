using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionControl;
using VersionControl.Merge;
using VersionControl.Merge.File;
using VersionControl.Merge.Text;

namespace MergeUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = "v1";
            var originalFileName = Path.Combine(folder, "0.txt");
            var fileName1 = Path.Combine(folder, "1.txt");
            var fileName2 = Path.Combine(folder, "2.txt");

            //use in-memory merge
            TextMerge(Path.Combine(folder, "txt-result.txt"), originalFileName, fileName1, fileName2);

            //use disk merge
            FileMerge(Path.Combine(folder, "file-result.txt"), originalFileName, fileName1, fileName2);

            Console.WriteLine("Done.");
        }


        private static void TextMerge(string output, string originalFileName, string fileName1, string fileName2)
        {
            var orignalText = File.ReadAllLines(originalFileName);
            var text1 = File.ReadAllLines(fileName1);
            var text2 = File.ReadAllLines(fileName2);

            var merger = new TextMerger();
            var result = merger.Merge(orignalText, text1, text2);

            Console.WriteLine("Conflicts: " + result.Conflicts.Count());
            File.WriteAllText(output, result.Text);
        }


        private static void FileMerge(string output, string originalFileName, string fileName1, string fileName2)
        {
            var merger = new TextFileMerger();
            var result = merger.Merge(originalFileName, fileName1, fileName2);

            Console.WriteLine("Conflicts: " + result.Conflicts.Count());
            File.WriteAllText(output, result.Text);
        }
    }
}
