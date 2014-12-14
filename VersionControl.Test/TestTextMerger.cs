using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VersionControl.Merge.Text;

namespace VersionControl.Test
{
    [TestClass]
    public class TestTextMerger
    {
        //1.1
        //1st developer: didn't change anything
        //2nd developer: didn't change anything
        [TestMethod]
        public void NothingIsChanged()
        {
            var original = new string[] { };
            var v1 = original;
            var v2 = original;
            var expectedStr = string.Join("\n", original);

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(0, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }


        // 1.2
        //1st dev added  ' ' to 1st line
        //1nd dev added  ' ' to 2st line
        //result: ' ' added to both lines
        [TestMethod]
        public void WhitespaceChanges()
        {
            var original = new[] { "1" };
            var v1 = new[] { "1 " };
            var v2 = v1;

            var expectedStr = string.Join("\n", original);

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(0, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }



        //2 
        //1st developer: changed the file
        //2nd developer:  didn't change the file
        //result: no conflicts, applied changes of 1st developer
        //result: ' ' added to both lines
        [TestMethod]
        public void Changed_vs_NotChanged()
        {
            var original = new string[] { "1", "2" };
            var v1 = new string[] { "0", "1v1", "2v2", "3", "4" };
            var v2 = original;
            var expectedStr = string.Join("\n", v1);

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(0, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }
        

        //4 -  cleaned by 1st dev
        //1st developer: didn't change the file
        //2nd developer:  removed all lines
        //result: no conflicts, the file is empty
        [TestMethod]
        public void Cleaned_vs_NotChanged()
        {
            var original = new[] { "1", "2" };
            var v1 = original;
            var v2 = new string[] { };
            var expectedStr = string.Join("\n", v2);

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(0, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }

        //5 -  clean by both devs
        //1st developer: removed all lines
        //2nd developer:  removed all lines
        //result: no conflicts, the file is empty
        [TestMethod]
        public void BothAreCleaned()
        {
            var original = new[] { "1", "2" };
            var v1 = new string[] { };
            var v2 = new string[] { };
            var expectedStr = "";

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(0, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }


        //6 -  cleaned by 1st and changed by 2nd
        //1st developer: removed all lines
        //2nd developer:  changed one line
        //result: one-line conflict, other lines were removed?? confirm?
        [TestMethod]
        public void Cleaned_vs_Changed()
        {
            var original = new[] { "1", "2", "3" };
            var v1 = new[] { "1", "3" };
            var v2 = new[] { "1", "2changed" };
            var expectedStr = string.Join("\n", new string[] { "1", "2changed" });

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(1, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }

        //7
        //a block of lines was added
        //result: no conflict
        [TestMethod]
        public void BothAreChangedAround()
        {
            var original = new[] { "1", "2", "3", "4" };
            var v1 = new[] { "1v1", "2", "3", "4v1" };
            var v2 = new[] { "1", "2v2", "3", "4" };
            var expectedStr = string.Join("\n", new[] { "1v1", "2v2", "3", "4v1" });

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(0, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }

        // 8 
        //a line is changed in both versions
        //result: conflict
        [TestMethod]
        public void SameLinesAreChanged()
        {
            var original = new[] { "1", "2", "3", "4" };
            var v1 = new[] { "1", "2v1", "3", "4" };
            var v2 = new[] { "1", "2v2", "3", "4" };

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2);

            Assert.AreEqual(1, result.Conflicts.Count());
        }

        [TestMethod]
        public void MergeFewVersions()
        {
            var original = new[] { "1", "2", "3", "4" };
            var v1 = new[] { "1", "2v1", "3", "4" };
            var v2 = new[] { "1", "2", "3v2", "4" };
            var v3 = new[] { "2", "3", "4v3" };
            var v4 = new[] { "-2v4", "1", "-1v4", "2", "3", "4", "5v4", "6v4" };

            var expectedStr = string.Join("\n", new[] { "-2v4", "-1v4", "2v1", "3v2", "4v3", "5v4", "6v4" });

            var merger = new TextMerger();
            var result = merger.Merge(original, v1, v2, v3, v4);

            Assert.AreEqual(0, result.Conflicts.Count());
            Assert.AreEqual(expectedStr, result.Text);
        }

    }
}
