using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Tests
{
    [TestFixture]
    public class TestStringLine
    {
        // TODO: Add tests for StringLine
        // TODO: Add more tests for StringLineGroup

        [Test]
        public void TestStringLineGroupSimple()
        {
            var text = new StringLineGroup()
            {
                new StringLine("ABC"),
                new StringLine("E"),
                new StringLine("F")
            };

            var chars = ToString(text);
            TextAssert.AreEqual("ABC\nE\nF", chars.ToString());
        }

        private static string ToString(StringLineGroup text)
        {
            var chars = new StringBuilder();
            while (text.Current != '\0')
            {
                chars.Append(text.Current);
                text.NextChar();
            }
            return chars.ToString();
        }

        [Test]
        public void TestStringLineGroupSaveAndRestore()
        {
            var text = new StringLineGroup()
            {
                new StringLine("ABCD"),
                new StringLine("EF"),
            };

            text.NextChar(); // B

            var save = text.Save();

            text.NextChar(); // C
            text.NextChar(); // D
            text.NextChar(); // \n
            text.NextChar();
            Assert.AreEqual('E', text.Current);
            Assert.AreEqual(1, text.LinePosition);
            Assert.AreEqual(0, text.ColumnPosition);

            text.Restore(ref save);
            Assert.AreEqual('B', text.Current);
            Assert.AreEqual(0, text.LinePosition);
            Assert.AreEqual(1, text.ColumnPosition);
        }

        [Test]
        public void TestSkipWhitespaces()
        {
            var text = new StringLineGroup("             ABC");
            Assert.True(text.SkipWhiteSpaces());
            Assert.False(text.SkipWhiteSpaces());
            Assert.AreEqual('A', text.Current);
        }

        [Test]
        public void TestStringLineGroupWithModifiedStart()
        {
            var line1 = new StringLine("  ABC");
            line1.NextChar();
            line1.NextChar();

            var line2 = new StringLine("  DEF ");
            line2.Trim();

            var text = new StringLineGroup() {line1, line2};

            var result = ToString(text);
            TextAssert.AreEqual("ABC\nDEF", result);
        }


        [Test]
        public void TestStringLineGroupWithTrim()
        {
            var line1 = new StringLine("  ABC  ");
            line1.NextChar();
            line1.NextChar();

            var line2 = new StringLine("  DEF ");

            var text = new StringLineGroup() { line1, line2 };
            text.Trim();

            var result = ToString(text);
            TextAssert.AreEqual("ABC  \n  DEF", result);
        }


    }
}