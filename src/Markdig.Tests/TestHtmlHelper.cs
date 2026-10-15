// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;

namespace Markdig.Tests;

[TestFixture]
public class TestHtmlHelper
{
    [Test]
    public void TestParseHtmlTagSimple()
    {
        var inputTag = "<a>";
        var text = new StringSlice(inputTag);
        Assert.True(HtmlHelper.TryParseHtmlTag(ref text, out string outputTag));
        Assert.AreEqual(inputTag, outputTag);
    }

    [Test]
    public void TestParseHtmlTagSimpleWithAttribute()
    {
        var inputTag = "<a href='http://google.com'>";
        var text = new StringSlice(inputTag);
        Assert.True(HtmlHelper.TryParseHtmlTag(ref text, out string outputTag));
        Assert.AreEqual(inputTag, outputTag);
    }

    [TestCase("</a>", true)]
    [TestCase("</A-b9 >", true)]
    [TestCase("</del\t>", true)]
    [TestCase("</del \t \t>", true)]
    [TestCase("</del\n>", true)]
    [TestCase("</del\r>", true)]
    [TestCase("</del\r\n>", true)]
    [TestCase("</del \t\r\n\t >", true)]
    [TestCase("</del/>", false)]
    [TestCase("</del attr>", false)]
    [TestCase("</de\tl>", false)]
    [TestCase("</1del>", false)]
    [TestCase("</é>", false)]
    [TestCase("</ del>", false)]
    [TestCase("</del", false)]
    [TestCase("</del\t", false)]
    [TestCase("</del\n\n>", false)]
    [TestCase("</del\r\r>", false)]
    [TestCase("</del\n\r>", false)]
    [TestCase("</del\r\n\n>", false)]
    [TestCase("</del\r\r\n>", false)]
    [TestCase("</del\r\n\r\n>", false)]
    [TestCase("</del\f>", false)]
    [TestCase("</del\v>", false)]
    [TestCase("</del\u0085>", false)]
    [TestCase("</del\u00a0>", false)]
    [TestCase("</del \u00a0>", false)]
    [TestCase("</del\u2003>", false)]
    [TestCase("</del\u2028>", false)]
    [TestCase("</del\u2029>", false)]
    public void ClosingTagsFollowCommonMarkGrammar(string inputTag, bool expected)
    {
        var text = new StringSlice(inputTag + "tail");

        Assert.AreEqual(expected, HtmlHelper.TryParseHtmlTag(ref text, out string outputTag));
        if (expected)
        {
            Assert.AreEqual(inputTag, outputTag);
            Assert.AreEqual(inputTag.Length, text.Start);
        }
        else
        {
            Assert.IsNull(outputTag);
        }
    }

    [TestCase("</del")]
    [TestCase("</del\t")]
    [TestCase("</del\r")]
    [TestCase("</del\r\n")]
    public void ClosingTagsRespectSliceEnd(string prefix)
    {
        var text = new StringSlice(prefix + "\n>tail", 0, prefix.Length - 1);

        Assert.False(HtmlHelper.TryParseHtmlTag(ref text, out string outputTag));
        Assert.IsNull(outputTag);
    }
}
