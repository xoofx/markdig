using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestCodeInline
    {
        [TestCase("``")]
        [TestCase(" ``")]
        [TestCase("`` ")]
        [TestCase(" `` ")]

        [TestCase("`c`")]
        [TestCase(" `c`")]
        [TestCase("`c` ")]
        [TestCase(" `c` ")]

        [TestCase("` c`")]
        [TestCase(" ` c`")]
        [TestCase("` c` ")]
        [TestCase(" ` c` ")]

        [TestCase("`c `")]
        [TestCase(" `c `")]
        [TestCase("`c ` ")]
        [TestCase(" `c ` ")]

        [TestCase("`c``")] // 1, 2
        [TestCase("``c`")] // 2, 1
        [TestCase("``c``")] // 2, 2

        [TestCase("```c``")] // 2, 3
        [TestCase("``c```")] // 3, 2
        [TestCase("```c```")] // 3, 3

        [TestCase("```c````")] // 3, 4
        [TestCase("````c```")] // 4, 3
        [TestCase("````c````")] // 4, 4

        [TestCase("```a``` p")]
        [TestCase("```a`b`c```")]
        [TestCase("```a``` p\n```a``` p")]

        [TestCase("` a `")]
        [TestCase(" ` a `")]
        [TestCase("` a ` ")]
        [TestCase(" ` a ` ")]
        public void Test(string value)
        {
            RoundTrip(value);
        }

        [TestCase("p `a` p")]
        [TestCase("p ``a`` p")]
        [TestCase("p ```a``` p")]
        [TestCase("p\n\n```a``` p")]
        public void TestParagraph(string value)
        {
            RoundTrip(value);
        }

        [TestCase("`\na\n`")]
        [TestCase("`\na\r`")]
        [TestCase("`\na\r\n`")]
        [TestCase("`\ra\r`")]
        [TestCase("`\ra\n`")]
        [TestCase("`\ra\r\n`")]
        [TestCase("`\r\na\n`")]
        [TestCase("`\r\na\r`")]
        [TestCase("`\r\na\r\n`")]
        public void Test_Newlines(string value)
        {
            RoundTrip(value);
        }
    }
}
