using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestStringSliceList
    {
        // TODO: Add tests for StringSlice
        // TODO: Add more tests for StringLineGroup

        [Test]
        public void TestStringLineGroupSimple()
        {
            var text = new StringLineGroup(4)
            {
                new StringSlice("ABC"),
                new StringSlice("E"),
                new StringSlice("F")
            };

            var iterator = text.ToCharIterator();
            Assert.AreEqual("ABC\nE\nF".Length, iterator.End - iterator.Start + 1);

            var chars = ToString(text.ToCharIterator());
            TextAssert.AreEqual("ABC\nE\nF", chars.ToString());
        }

        [Test]
        public void TestStringLineGroupWithSlices()
        {
            var text = new StringLineGroup(4)
            {
                new StringSlice("XABC") { Start = 1},
                new StringSlice("YYE") { Start = 2},
                new StringSlice("ZZZF") { Start = 3 }
            };

            var chars = ToString(text.ToCharIterator());
            TextAssert.AreEqual("ABC\nE\nF", chars.ToString());
        }


        private static string ToString(StringLineGroup.Iterator text)
        {
            var chars = new StringBuilder();
            while (text.CurrentChar != '\0')
            {
                chars.Append(text.CurrentChar);
                text.NextChar();
            }
            return chars.ToString();
        }

        [Test]
        public void TestStringLineGroupSaveAndRestore()
        {
            var text = new StringLineGroup(4)
            {
                new StringSlice("ABCD"),
                new StringSlice("EF"),
            }.ToCharIterator();

            Assert.AreEqual('A', text.CurrentChar);
            Assert.AreEqual(0, text.SliceIndex);

            text.NextChar(); // B

            text.NextChar(); // C
            text.NextChar(); // D
            text.NextChar(); // \n
            text.NextChar();
            Assert.AreEqual('E', text.CurrentChar);
            Assert.AreEqual(1, text.SliceIndex);
        }

        [Test]
        public void TestSkipWhitespaces()
        {
            var text = new StringLineGroup("             ABC").ToCharIterator();
            Assert.False(text.TrimStart());
            Assert.AreEqual('A', text.CurrentChar);

            text = new StringLineGroup("        ").ToCharIterator();
            Assert.True(text.TrimStart());
            Assert.AreEqual('\0', text.CurrentChar);

            var slice = new StringSlice("             ABC");
            Assert.False(slice.TrimStart());

            slice = new StringSlice("        ");
            Assert.True(slice.TrimStart());
        }

        [Test]
        public void TestStringLineGroupWithModifiedStart()
        {
            var line1 = new StringSlice("  ABC");
            line1.NextChar();
            line1.NextChar();

            var line2 = new StringSlice("  DEF ");
            line2.Trim();

            var text = new StringLineGroup(4) {line1, line2};

            var result = ToString(text.ToCharIterator());
            TextAssert.AreEqual("ABC\nDEF", result);
        }

        [Test]
        public void TestStringLineGroupWithTrim()
        {
            var line1 = new StringSlice("  ABC  ");
            line1.NextChar();
            line1.NextChar();

            var line2 = new StringSlice("  DEF ");

            var text = new StringLineGroup(4) { line1, line2}.ToCharIterator();
            text.TrimStart();

            var result = ToString(text);
            TextAssert.AreEqual("ABC  \n  DEF ", result);
        }
    }
}