using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestEmphasisInline
    {
        [TestCase("_t_")]
        [TestCase("_t_t")]
        [TestCase("t_t_")]
        [TestCase("_t t_")]
        [TestCase("_t\tt_")]
        [TestCase("*t*")]
        [TestCase("t*t*")]
        [TestCase("*t*t")]
        [TestCase("*t t*")]
        [TestCase("*t\tt*")]

        [TestCase(" _t_")]
        [TestCase(" _t_t")]
        [TestCase(" t_t_")]
        [TestCase(" _t t_")]
        [TestCase(" _t\tt_")]
        [TestCase(" *t*")]
        [TestCase(" t*t*")]
        [TestCase(" *t*t")]
        [TestCase(" *t t*")]
        [TestCase(" *t\tt*")]

        [TestCase("_t_")]
        [TestCase("_t_t ")]
        [TestCase("t_t_ ")]
        [TestCase("_t t_ ")]
        [TestCase("_t\tt_ ")]
        [TestCase("*t* ")]
        [TestCase("t*t* ")]
        [TestCase("*t*t ")]
        [TestCase("*t t* ")]
        [TestCase("*t\tt* ")]

        [TestCase(" _t_")]
        [TestCase(" _t_t ")]
        [TestCase(" t_t_ ")]
        [TestCase(" _t t_ ")]
        [TestCase(" _t\tt_ ")]
        [TestCase(" *t* ")]
        [TestCase(" t*t* ")]
        [TestCase(" *t*t ")]
        [TestCase(" *t t* ")]
        [TestCase(" *t\tt* ")]

        [TestCase("_t_\t")]
        [TestCase("_t_t\t")]
        [TestCase("t_t_\t")]
        [TestCase("_t t_\t")]
        [TestCase("_t\tt_\t")]
        [TestCase("*t*\t")]
        [TestCase("t*t*\t")]
        [TestCase("*t*t\t")]
        [TestCase("*t t*\t")]
        [TestCase("*t\tt*\t")]
        public void Test_Emphasis(string value)
        {
            RoundTrip(value);
        }

        [TestCase("__t__")]
        [TestCase("__t__t")]
        [TestCase("t__t__")]
        [TestCase("__t t__")]
        [TestCase("__t\tt__")]
        [TestCase("**t**")]
        [TestCase("**t**t")]
        [TestCase("t**t**")]
        [TestCase("**t\tt**")]

        [TestCase(" __t__")]
        [TestCase(" __t__t")]
        [TestCase(" t__t__")]
        [TestCase(" __t t__")]
        [TestCase(" __t\tt__")]
        [TestCase(" **t**")]
        [TestCase(" **t**t")]
        [TestCase(" t**t**")]
        [TestCase(" **t\tt**")]

        [TestCase("__t__ ")]
        [TestCase("__t__t ")]
        [TestCase("t__t__ ")]
        [TestCase("__t t__ ")]
        [TestCase("__t\tt__ ")]
        [TestCase("**t** ")]
        [TestCase("**t**t ")]
        [TestCase("t**t** ")]
        [TestCase("**t\tt** ")]

        [TestCase(" __t__ ")]
        [TestCase(" __t__t ")]
        [TestCase(" t__t__ ")]
        [TestCase(" __t t__ ")]
        [TestCase(" __t\tt__ ")]
        [TestCase(" **t** ")]
        [TestCase(" **t** t")]
        [TestCase(" t**t** ")]
        [TestCase(" **t\tt** ")]

        [TestCase("__t__\t")]
        [TestCase("__t__t\t")]
        [TestCase("t__t__\t ")]
        [TestCase("__t t__\t ")]
        [TestCase("__t\tt__\t ")]
        [TestCase("**t**\t ")]
        [TestCase("**t**t\t ")]
        [TestCase("t**t**\t ")]
        [TestCase("**t\tt**\t ")]

        [TestCase(" __t__\t ")]
        [TestCase(" __t__t\t ")]
        [TestCase(" t__t__\t ")]
        [TestCase(" __t t__\t ")]
        [TestCase(" __t\tt__\t ")]
        [TestCase(" **t**\t ")]
        [TestCase(" **t**\t t")]
        [TestCase(" t**t**\t ")]
        [TestCase(" **t\tt**\t ")]
        public void Test_StrongEmphasis(string value)
        {
            RoundTrip(value);
        }
    }
}
