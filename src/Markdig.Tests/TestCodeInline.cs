namespace Markdig.Tests;

public class TestCodeInline
{
    [Test]
    public void UnpairedCodeInlineWithTrailingChars()
    {
        TestParser.TestSpec("*`\n\f", "<p>*`</p>");
    }
}
