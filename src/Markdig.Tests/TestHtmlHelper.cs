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
}