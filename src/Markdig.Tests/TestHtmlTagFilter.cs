using NUnit.Framework;
using Markdig.Extensions.HtmlTagFilter;

namespace Markdig.Tests;

[TestFixture]
public class TestHtmlTagFilter
{
    [Test]
    public void Whitelist_BlockLevelHtml()
    {
        // Whitelist allows only specific tags
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagWhitelist("p", "strong", "em", "br")
            .Build();

        var markdown = """
            <div>
            Allowed content
            </div>

            <script>
            Blocked content
            </script>

            <p>
            Allowed paragraph
            </p>
            """;

        var expected = """
            <p>&lt;div&gt;
            Allowed content
            &lt;/div&gt;</p>
            <p>&lt;script&gt;
            Blocked content
            &lt;/script&gt;</p>
            <p>
            Allowed paragraph
            </p>

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }

    [Test]
    public void Whitelist_InlineHtml()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagWhitelist("strong", "em", "br")
            .Build();

        TestParser.TestSpec(
            "This is <strong>allowed</strong> and <span>blocked</span> and <em>allowed</em>.",
            "<p>This is <strong>allowed</strong> and &lt;span&gt;blocked&lt;/span&gt; and <em>allowed</em>.</p>",
            pipeline);
    }

    [Test]
    public void Whitelist_SelfClosingTags()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagWhitelist("br")
            .Build();

        TestParser.TestSpec(
            "Allowed: <br/>\nBlocked: <hr/>",
            "<p>Allowed: <br/>\nBlocked: &lt;hr/&gt;</p>",
            pipeline);
    }

    [Test]
    public void Whitelist_TopLevelTagsOnly()
    {
        // This test demonstrates that filtering works for top-level tags
        // Tags inside already-opened HTML blocks are not filtered (limitation of parser-level filtering)
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagWhitelist("p", "strong")
            .Build();

        var markdown = """
            <div>Blocked div</div>

            <p>Allowed paragraph</p>

            <script>Blocked script</script>

            Text with <strong>bold</strong> and <script>inline script</script>.
            """;

        var expected = """
            <p>&lt;div&gt;Blocked div&lt;/div&gt;</p>
            <p>Allowed paragraph</p>
            <p>&lt;script&gt;Blocked script&lt;/script&gt;</p>
            <p>Text with <strong>bold</strong> and &lt;script&gt;inline script&lt;/script&gt;.</p>

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }

    [Test]
    public void Blacklist_BlockLevelHtml()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagBlacklist("script", "iframe", "object", "embed")
            .Build();

        var markdown = """
            <div>
            Allowed content
            </div>

            <script>
            Blocked content
            </script>

            <p>
            Allowed paragraph
            </p>
            """;

        var expected = """
            <div>
            Allowed content
            </div>
            <p>&lt;script&gt;
            Blocked content
            &lt;/script&gt;</p>
            <p>
            Allowed paragraph
            </p>

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }

    [Test]
    public void Blacklist_InlineHtml()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagBlacklist("script")
            .Build();

        TestParser.TestSpec(
            "This is <span>allowed</span> and <script>blocked</script> and <em>allowed</em>.",
            "<p>This is <span>allowed</span> and &lt;script&gt;blocked&lt;/script&gt; and <em>allowed</em>.</p>",
            pipeline);
    }

    [Test]
    public void Blacklist_DangerousTags()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagBlacklist("script", "iframe", "object", "embed")
            .Build();

        var markdown = """
            <iframe src="evil.com"></iframe>

            <object data="evil.swf"></object>

            <embed src="evil.swf">

            <script>alert('xss')</script>
            """;

        var expected = """
            <p>&lt;iframe src=&quot;evil.com&quot;&gt;&lt;/iframe&gt;</p>
            <p>&lt;object data=&quot;evil.swf&quot;&gt;&lt;/object&gt;</p>
            <p>&lt;embed src=&quot;evil.swf&quot;&gt;</p>
            <p>&lt;script&gt;alert('xss')&lt;/script&gt;</p>

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }

    [Test]
    public void CaseInsensitivity()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagBlacklist("script")
            .Build();

        var markdown = """
            <SCRIPT>blocked</SCRIPT>
            <Script>blocked</Script>
            <script>blocked</script>
            """;

        var expected = """
            <p>&lt;SCRIPT&gt;blocked&lt;/SCRIPT&gt;
            &lt;Script&gt;blocked&lt;/Script&gt;
            &lt;script&gt;blocked&lt;/script&gt;</p>

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }

    [Test]
    public void HtmlComments_NotFiltered()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagBlacklist("script")
            .Build();

        var markdown = """
            <!-- This is a comment -->
            <script>blocked</script>
            <!-- Another comment -->
            """;

        var expected = """
            <!-- This is a comment -->
            <p>&lt;script&gt;blocked&lt;/script&gt;</p>
            <!-- Another comment -->

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }

    [Test]
    public void CDataAndProcessingInstructions_AllowedByDefault()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagBlacklist("script")
            .Build();

        var markdown = """
            <![CDATA[
            <script>This is in CDATA</script>
            ]]>

            <?xml version="1.0"?>

            <script>blocked</script>
            """;

        // CDATA and processing instructions are special HTML constructs that pass through
        // Only regular HTML tags are filtered
        var expected = """
            <p><![CDATA[
            <script>This is in CDATA</script>
            ]]></p>
            <p><?xml version="1.0"?></p>
            <p>&lt;script&gt;blocked&lt;/script&gt;</p>

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }

    [Test]
    public void CloseTags_FilteredSameAsOpenTags()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseHtmlTagBlacklist("script")
            .Build();

        var markdown = """
            <script>
            content
            </script>

            <div>
            content
            </div>
            """;

        var expected = """
            <p>&lt;script&gt;
            content
            &lt;/script&gt;</p>
            <div>
            content
            </div>

            """;

        TestParser.TestSpec(markdown, expected, pipeline);
    }
}
