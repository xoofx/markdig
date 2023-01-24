using Markdig.Helpers;

namespace Markdig.Tests;

public class TestLazySubstring
{
    [Theory]
    [TestCase("")]
    [TestCase("a")]
    [TestCase("foo")]
    public void LazySubstring_ReturnsCorrectSubstring(string text)
    {
        var substring = new LazySubstring(text);
        Assert.AreEqual(0, substring.Offset);
        Assert.AreEqual(text.Length, substring.Length);

        Assert.AreEqual(text, substring.AsSpan().ToString());
        Assert.AreEqual(text, substring.AsSpan().ToString());
        Assert.AreEqual(0, substring.Offset);
        Assert.AreEqual(text.Length, substring.Length);

        Assert.AreSame(substring.ToString(), substring.ToString());
        Assert.AreEqual(text, substring.ToString());
        Assert.AreEqual(0, substring.Offset);
        Assert.AreEqual(text.Length, substring.Length);

        Assert.AreEqual(text, substring.AsSpan().ToString());
        Assert.AreEqual(text, substring.AsSpan().ToString());
        Assert.AreEqual(0, substring.Offset);
        Assert.AreEqual(text.Length, substring.Length);
    }

    [Theory]
    [TestCase("", 0, 0)]
    [TestCase("a", 0, 0)]
    [TestCase("a", 1, 0)]
    [TestCase("a", 0, 1)]
    [TestCase("foo", 1, 0)]
    [TestCase("foo", 1, 1)]
    [TestCase("foo", 1, 2)]
    [TestCase("foo", 0, 3)]
    public void LazySubstring_ReturnsCorrectSubstring(string text, int start, int length)
    {
        var substring = new LazySubstring(text, start, length);
        Assert.AreEqual(start, substring.Offset);
        Assert.AreEqual(length, substring.Length);

        string expectedSubstring = text.Substring(start, length);

        Assert.AreEqual(expectedSubstring, substring.AsSpan().ToString());
        Assert.AreEqual(expectedSubstring, substring.AsSpan().ToString());
        Assert.AreEqual(start, substring.Offset);
        Assert.AreEqual(length, substring.Length);

        Assert.AreSame(substring.ToString(), substring.ToString());
        Assert.AreEqual(expectedSubstring, substring.ToString());
        Assert.AreEqual(0, substring.Offset);
        Assert.AreEqual(length, substring.Length);

        Assert.AreEqual(expectedSubstring, substring.AsSpan().ToString());
        Assert.AreEqual(expectedSubstring, substring.AsSpan().ToString());
        Assert.AreEqual(0, substring.Offset);
        Assert.AreEqual(length, substring.Length);
    }
}
