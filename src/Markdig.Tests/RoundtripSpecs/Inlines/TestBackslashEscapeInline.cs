using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestBackslashEscapeInline
    {
        [TestCase(@"\!")]
        [TestCase(@"\""")]
        [TestCase(@"\#")]
        [TestCase(@"\$")]
        [TestCase(@"\&")]
        [TestCase(@"\'")]
        [TestCase(@"\(")]
        [TestCase(@"\)")]
        [TestCase(@"\*")]
        [TestCase(@"\+")]
        [TestCase(@"\,")]
        [TestCase(@"\-")]
        [TestCase(@"\.")]
        [TestCase(@"\/")]
        [TestCase(@"\:")]
        [TestCase(@"\;")]
        [TestCase(@"\<")]
        [TestCase(@"\=")]
        [TestCase(@"\>")]
        [TestCase(@"\?")]
        [TestCase(@"\@")]
        [TestCase(@"\[")]
        [TestCase(@"\\")]
        [TestCase(@"\]")]
        [TestCase(@"\^")]
        [TestCase(@"\_")]
        [TestCase(@"\`")]
        [TestCase(@"\{")]
        [TestCase(@"\|")]
        [TestCase(@"\}")]
        [TestCase(@"\~")]

        // below test breaks visual studio
        //[TestCase(@"\!\""\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\_\`\{\|\}\~")]
        public void Test(string value)
        {
            RoundTrip(value);
        }

        [TestCase(@"# \#\#h1")]
        [TestCase(@"# \#\#h1\#")]
        public void TestHeading(string value)
        {
            RoundTrip(value);
        }

        [TestCase(@"`\``")]
        [TestCase(@"` \``")]
        [TestCase(@"`\` `")]
        [TestCase(@"` \` `")]
        [TestCase(@" ` \` `")]
        [TestCase(@"` \` ` ")]
        [TestCase(@" ` \` ` ")]
        public void TestCodeSpanInline(string value)
        {
            RoundTrip(value);
        }
    }
}
