using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs
{
    // TODO: RTP: test info strings
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
        public void Test(string value)
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
    }
}
