using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestCodeInline
    {
        [TestCase("``")]
        [TestCase(" ``")]
        [TestCase("`` ")]
        [TestCase(" `` ")]

        [TestCase("`a`")]
        [TestCase(" `a`")]
        [TestCase("`a` ")]
        [TestCase(" `a` ")]

        [TestCase("` a`")]
        [TestCase(" ` a`")]
        [TestCase("` a` ")]
        [TestCase(" ` a` ")]

        [TestCase("`a `")]
        [TestCase(" `a `")]
        [TestCase("`a ` ")]
        [TestCase(" `a ` ")]

        [TestCase("``a``")]
        [TestCase("- ```a```")]
        [TestCase("p ```a``` p")]

        [TestCase("```c```")]

        // broken
        //[TestCase("```a```")]
        [TestCase("```a``` p")]
        [TestCase("```a`b`c```")]
        //[TestCase("p\n\n```a``` p")]
        //[TestCase("```a``` p\n```a``` p")]

        /// <see cref="CodeInlineParser"/>: intentionally trimmed. TODO: decide on how to handle
        //[TestCase("` a `")]
        //[TestCase(" ` a `")]
        //[TestCase("` a ` ")]
        //[TestCase(" ` a ` ")]
        public void Test_CodeInline(string value)
        {
            RoundTrip(value);
        }
    }
}
