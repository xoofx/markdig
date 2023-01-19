using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestFencedCodeBlock
{
    [TestCase("```\nc\n```")]
    [TestCase("```\nc\n```\n")]
    [TestCase("\n```\nc\n```")]
    [TestCase("\n\n```\nc\n```")]
    [TestCase("```\nc\n```\n")]
    [TestCase("```\nc\n```\n\n")]
    [TestCase("\n```\nc\n```\n")]
    [TestCase("\n```\nc\n```\n\n")]
    [TestCase("\n\n```\nc\n```\n")]
    [TestCase("\n\n```\nc\n```\n\n")]

    [TestCase(" ```\nc\n````")]
    [TestCase("```\nc\n````")]
    [TestCase("p\n\n```\nc\n```")]

    [TestCase("```\n c\n```")]
    [TestCase("```\nc \n```")]
    [TestCase("```\n c \n```")]

    [TestCase(" ``` \n c \n ``` ")]
    [TestCase("\t```\t\n\tc\t\n\t```\t")]
    [TestCase("\v```\v\n\vc\v\n\v```\v")]
    [TestCase("\f```\f\n\fc\f\n\f```\f")]
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase("~~~ aa ``` ~~~\nfoo\n~~~")]
    [TestCase("~~~ aa ``` ~~~\nfoo\n~~~ ")]
    public void TestTilde(string value)
    {
        RoundTrip(value);
    }

    [TestCase("```\n c \n```")]
    [TestCase("```\n c \r```")]
    [TestCase("```\n c \r\n```")]
    [TestCase("```\r c \n```")]
    [TestCase("```\r c \r```")]
    [TestCase("```\r c \r\n```")]
    [TestCase("```\r\n c \n```")]
    [TestCase("```\r\n c \r```")]
    [TestCase("```\r\n c \r\n```")]

    [TestCase("```\n c \n```\n")]
    [TestCase("```\n c \r```\n")]
    [TestCase("```\n c \r\n```\n")]
    [TestCase("```\r c \n```\n")]
    [TestCase("```\r c \r```\n")]
    [TestCase("```\r c \r\n```\n")]
    [TestCase("```\r\n c \n```\n")]
    [TestCase("```\r\n c \r```\n")]
    [TestCase("```\r\n c \r\n```\n")]

    [TestCase("```\n c \n```\r")]
    [TestCase("```\n c \r```\r")]
    [TestCase("```\n c \r\n```\r")]
    [TestCase("```\r c \n```\r")]
    [TestCase("```\r c \r```\r")]
    [TestCase("```\r c \r\n```\r")]
    [TestCase("```\r\n c \n```\r")]
    [TestCase("```\r\n c \r```\r")]
    [TestCase("```\r\n c \r\n```\r")]

    [TestCase("```\n c \n```\r\n")]
    [TestCase("```\n c \r```\r\n")]
    [TestCase("```\n c \r\n```\r\n")]
    [TestCase("```\r c \n```\r\n")]
    [TestCase("```\r c \r```\r\n")]
    [TestCase("```\r c \r\n```\r\n")]
    [TestCase("```\r\n c \n```\r\n")]
    [TestCase("```\r\n c \r```\r\n")]
    [TestCase("```\r\n c \r\n```\r\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }

    [TestCase("```i a\n```")]
    [TestCase("```i a a2\n```")]
    [TestCase("```i a a2 a3\n```")]
    [TestCase("```i a a2 a3 a4\n```")]

    [TestCase("```i\ta\n```")]
    [TestCase("```i\ta a2\n```")]
    [TestCase("```i\ta a2 a3\n```")]
    [TestCase("```i\ta a2 a3 a4\n```")]

    [TestCase("```i\ta \n```")]
    [TestCase("```i\ta a2 \n```")]
    [TestCase("```i\ta a2 a3 \n```")]
    [TestCase("```i\ta a2 a3 a4 \n```")]
    public void TestInfoArguments(string value)
    {
        RoundTrip(value);
    }
}
