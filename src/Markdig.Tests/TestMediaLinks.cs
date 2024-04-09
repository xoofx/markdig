using System.Text.RegularExpressions;

using Markdig.Extensions.MediaLinks;

namespace Markdig.Tests;

[TestFixture]
public class TestMediaLinks
{
    private MarkdownPipeline GetPipeline(MediaOptions options = null)
    {
        return new MarkdownPipelineBuilder()
             .UseMediaLinks(options)
             .Build();
    }

    private MarkdownPipeline GetPipelineWithBootstrap(MediaOptions options = null)
    {
        return new MarkdownPipelineBuilder()
            .UseBootstrap()
            .UseMediaLinks(options)
            .Build();
    }

    [Test]
    [TestCase("![static mp4](https://sample.com/video.mp4)", "<p><video width=\"500\" height=\"281\" controls=\"\"><source type=\"video/mp4\" src=\"https://sample.com/video.mp4\"></source></video></p>\n")]
    [TestCase("![static mp4](//sample.com/video.mp4)", "<p><video width=\"500\" height=\"281\" controls=\"\"><source type=\"video/mp4\" src=\"//sample.com/video.mp4\"></source></video></p>\n")]
    [TestCase(@"![youtube short](https://www.youtube.com/shorts/6BUptHVuvyI?feature=share)", "<p><iframe src=\"https://www.youtube.com/embed/6BUptHVuvyI\" class=\"youtubeshort\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n")]
    [TestCase(@"![youtube.com](https://www.youtube.com/watch?v=mswPy5bt3TQ)", "<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n")]
    [TestCase("![yandex.ru](https://music.yandex.ru/album/411845/track/4402274)", "<p><iframe src=\"https://music.yandex.ru/iframe/#track/4402274/411845/\" class=\"yandex\" width=\"500\" height=\"281\" frameborder=\"0\"></iframe></p>\n")]
    [TestCase("![vimeo](https://vimeo.com/8607834)", "<p><iframe src=\"https://player.vimeo.com/video/8607834\" class=\"vimeo\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n")]
    [TestCase("![ok.ru](https://ok.ru/video/26870090463)", "<p><iframe src=\"https://ok.ru/videoembed/26870090463\" class=\"odnoklassniki\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n")]
    [TestCase("![ok.ru](//ok.ru/video/26870090463)", "<p><iframe src=\"https://ok.ru/videoembed/26870090463\" class=\"odnoklassniki\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n")]
    public void TestBuiltInHosts(string markdown, string expected)
    {
        string html = Markdown.ToHtml(markdown, GetPipeline());
        Assert.AreEqual(expected, html);
    }

    [TestCase("![static video relative path](./video.mp4)",
        "<p><video width=\"500\" height=\"281\" controls=\"\"><source type=\"video/mp4\" src=\"./video.mp4\"></source></video></p>\n")]
    [TestCase("![static audio relative path](./audio.mp3)",
        "<p><audio width=\"500\" controls=\"\"><source type=\"audio/mpeg\" src=\"./audio.mp3\"></source></audio></p>\n")]
    public void TestBuiltInHostsWithRelativePaths(string markdown, string expected)
    {
        string html = Markdown.ToHtml(markdown, GetPipeline());
        Assert.AreEqual(expected, html);
    }
    
    private class TestHostProvider : IHostProvider
    {
        public string Class { get; } = "regex";
        public bool AllowFullScreen { get; }

        public bool TryHandle(Uri mediaUri, bool isSchemaRelative, out string iframeUrl)
        {
            iframeUrl = null;
            var uri = isSchemaRelative ? "//" + mediaUri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Scheme, UriFormat.UriEscaped) : mediaUri.ToString();
            if (!matcher.IsMatch(uri))
                return false;
            iframeUrl = matcher.Replace(uri, replacement);
            return true;
        }

        private Regex matcher;
        private string replacement;

        public TestHostProvider(string provider, string replace)
        {
            matcher = new Regex(provider);
            replacement = replace;
        }
    }

    [Test]
    [TestCase("![p1](https://sample.com/video.mp4)", "<p><iframe src=\"https://example.com/video.mp4\" class=\"regex\" width=\"500\" height=\"281\" frameborder=\"0\"></iframe></p>\n", @"^https?://sample.com/(.+)$", @"https://example.com/$1")]
    [TestCase("![p1](//sample.com/video.mp4)", "<p><iframe src=\"https://example.com/video.mp4\" class=\"regex\" width=\"500\" height=\"281\" frameborder=\"0\"></iframe></p>\n", @"^//sample.com/(.+)$", @"https://example.com/$1")]
    [TestCase("![p1](https://sample.com/video.mp4)", "<p><iframe src=\"https://example.com/video.mp4?token=aaabbb\" class=\"regex\" width=\"500\" height=\"281\" frameborder=\"0\"></iframe></p>\n", @"^https?://sample.com/(.+)$", @"https://example.com/$1?token=aaabbb")]
    public void TestCustomHostProvider(string markdown, string expected, string provider, string replace)
    {
        string html = Markdown.ToHtml(markdown, GetPipeline(new MediaOptions
        {
            Hosts =
            {
                new TestHostProvider(provider, replace),
            }
        }));
        Assert.AreEqual(html, expected);
    }

    [Test]
    [TestCase("![static mp4](//sample.com/video.mp4)", "<p><video width=\"500\" height=\"281\" controls=\"\"><source type=\"video/mp4\" src=\"//sample.com/video.mp4\"></source></video></p>\n", "")]
    [TestCase(@"![youtube.com](https://www.youtube.com/watch?v=mswPy5bt3TQ)", "<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n", "")]
    [TestCase("![static mp4](//sample.com/video.mp4)", "<p><video class=\"k\" width=\"500\" height=\"281\" controls=\"\"><source type=\"video/mp4\" src=\"//sample.com/video.mp4\"></source></video></p>\n", "k")]
    [TestCase(@"![youtube.com](https://www.youtube.com/watch?v=mswPy5bt3TQ)", "<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ\" class=\"k youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n", "k")]
    public void TestCustomClass(string markdown, string expected, string klass)
    {
        string html = Markdown.ToHtml(markdown, GetPipeline(new MediaOptions
        {
            Class = klass,
        }));
        Assert.AreEqual(html, expected);
    }

    [Test]
    [TestCase("![static mp4](//sample.com/video.mp4)", "<p><video class=\"img-fluid\" width=\"500\" height=\"281\" controls=\"\"><source type=\"video/mp4\" src=\"//sample.com/video.mp4\"></source></video></p>\n")]
    [TestCase(@"![youtube.com](https://www.youtube.com/watch?v=mswPy5bt3TQ)", "<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ\" class=\"img-fluid youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n")]
    public void TestWithBootstrap(string markdown, string expected)
    {
        string html = Markdown.ToHtml(markdown, GetPipelineWithBootstrap());
        Assert.AreEqual(html, expected);
    }
}
