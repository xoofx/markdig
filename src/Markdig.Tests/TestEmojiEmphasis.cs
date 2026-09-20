// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Emoji;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Helpers;
using Markdig.Parsers.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public class TestEmojiEmphasis
{
    [TestCase("**Non-goals (explicitly out of scope for this plan):**", "<strong>Non-goals (explicitly out of scope for this plan):</strong>")]
    [TestCase("*Non-goals (explicitly out of scope for this plan):*", "<em>Non-goals (explicitly out of scope for this plan):</em>")]
    [TestCase("***Non-goals (explicitly out of scope for this plan):***", "<em><strong>Non-goals (explicitly out of scope for this plan):</strong></em>")]
    [TestCase("*:*", "<em>:</em>")]
    [TestCase("**:**", "<strong>:</strong>")]
    [TestCase("*outer **inner):***", "<em>outer <strong>inner):</strong></em>")]
    [TestCase("*a* :*", "<em>a</em> 😗")]
    [TestCase(":*", "😗")]
    [TestCase("(:*)", "(😗)")]
    [TestCase(":**", "😗*")]
    [TestCase(":***", "😗**")]
    [TestCase(":*text", "😗text")]
    [TestCase(":*text*", ":<em>text</em>")]
    [TestCase("*text :* more*", "<em>text :</em> more*")]
    [TestCase("*:kissing:*", "<em>😗</em>")]
    [TestCase("**:)**", "<strong>😃</strong>")]
    [TestCase("*text :-* more*", "<em>text 😗more</em>")]
    [TestCase(@"\*text :*", "*text 😗")]
    [TestCase(@"*text :\*", "*text :*")]
    [TestCase("`*text :*`", "<code>*text :*</code>")]
    [TestCase("[*text):*](url)", "<a href=\"url\"><em>text):</em></a>")]
    [TestCase("*text [link](url):*", "<em>text <a href=\"url\">link</a>:</em>")]
    [TestCase("*outside [inside :*](url)", "*outside <a href=\"url\">inside 😗</a>")]
    [TestCase("[*inside](url) :*", "<a href=\"url\">*inside</a> 😗")]
    [TestCase(":* :* **:** :*", "😗 😗 <strong>:</strong> 😗")]
    [TestCase("**:***", "<strong>:</strong>*")]
    [TestCase("*:**", "<em>:</em>*")]
    [TestCase("*a\ntext):*", "<em>a\ntext):</em>")]
    [TestCase(":*\n:*", "😗\n😗")]
    [TestCase("![*text):*](url)", "<img src=\"url\" alt=\"text):\" />")]
    [TestCase("[text :*][missing]", "[text 😗][missing]")]
    public void EmphasisTakesPriority(string markdown, string expected)
    {
        foreach (bool trackTrivia in new[] { false, true })
        {
            var builder = new MarkdownPipelineBuilder().UseEmojiAndSmiley();
            if (trackTrivia) builder.EnableTrackTrivia();
            Assert.AreEqual($"<p>{expected}</p>\n", Markdown.ToHtml(markdown, builder.Build()));
        }
    }

    [TestCase("_text):_", "<em>text):</em>")]
    [TestCase("~~text):~~", "<del>text):</del>")]
    [TestCase("++text):++", "<ins>text):</ins>")]
    [TestCase("==text):==", "<mark>text):</mark>")]
    [TestCase("^text):^", "<sup>text):</sup>")]
    [TestCase(":_ :~ :+ := :^", "emoji emoji emoji emoji emoji")]
    public void CustomMappingsRespectConfiguredEmphasis(string markdown, string expected)
    {
        var mapping = new EmojiMapping(new Dictionary<string, string>
        {
            [":_"] = "emoji", [":~"] = "emoji", [":+"] = "emoji",
            [":="] = "emoji", [":^"] = "emoji"
        }, new Dictionary<string, string>());
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley(customEmojiMapping: mapping)
            .UseEmphasisExtras(EmphasisExtraOptions.Default).Build();
        Assert.AreEqual($"<p>{expected}</p>\n", Markdown.ToHtml(markdown, pipeline));
    }

    [Test]
    public void EmphasisCanBeDisabled()
    {
        var builder = new MarkdownPipelineBuilder().UseEmojiAndSmiley();
        builder.InlineParsers.Remove(builder.InlineParsers.Find<EmphasisInlineParser>());
        // Register explicitly: the extension normally inserts before the emphasis parser.
        builder.InlineParsers.Add(new EmojiParser(new EmojiMapping()));
        Assert.AreEqual("<p>**text)😗*</p>\n", Markdown.ToHtml("**text):**", builder.Build()));
    }

    [Test]
    public void SmileysCanBeDisabled()
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley(enableSmileys: false).Build();
        Assert.AreEqual("<p><em>text):</em> :*</p>\n", Markdown.ToHtml("*text):* :*", pipeline));
    }

    [Test]
    public void EmojiDoesNotChangeEmphasisStructure()
    {
        var plain = new MarkdownPipelineBuilder().UsePreciseSourceLocation().Build();
        var emoji = new MarkdownPipelineBuilder().UsePreciseSourceLocation().UseEmojiAndSmiley().Build();
        string[] runs = ["", "*", "**", "***", "****", "_", "__", "___"];
        string[] contents = ["text):", ":", "text :", "text):* more):", "[text):*](url):", "`code*` :", "text\n:"];
        foreach (var opening in runs)
        foreach (var middle in contents)
        foreach (var closing in runs)
        foreach (var ending in new[] { "", "tail", " tail*", " :*", "&amp;", "&#32;" })
        {
            var markdown = $"prefix {opening}{middle}{closing}{ending}";
            var expected = Markdown.Parse(markdown, plain).Descendants<EmphasisInline>()
                .Select(x => (x.DelimiterChar, x.DelimiterCount, x.Span)).ToArray();
            var actual = Markdown.Parse(markdown, emoji).Descendants<EmphasisInline>()
                .Select(x => (x.DelimiterChar, x.DelimiterCount, x.Span)).ToArray();
            Assert.AreEqual(expected, actual, markdown);
        }
    }

    [TestCase(":* tail", 0)]
    [TestCase("text :** tail", 5)]
    [TestCase("*a* :*", 4)]
    [TestCase("[link :*](url)", 6)]
    public void DeferredEmojiRetainsSourceLocation(string markdown, int start)
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley().UsePreciseSourceLocation().Build();
        var document = Markdown.Parse(markdown, pipeline);
        var emoji = document.Descendants<EmojiInline>().Single();
        Assert.AreEqual(new SourceSpan(start, start + 1), emoji.Span);
        Assert.AreEqual(0, emoji.Line);
        Assert.AreEqual(start, emoji.Column);
        Assert.AreEqual(":*", emoji.Match);
        foreach (var literal in document.Descendants<LiteralInline>().Where(x => x is not EmojiInline))
        {
            Assert.AreEqual(markdown.Substring(literal.Span.Start, literal.Span.Length), literal.Content.ToString());
        }
    }

    [Test]
    public void EmojiAndEmphasisStayWithinTableCells()
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley().UsePipeTables().Build();
        var html = Markdown.ToHtml("| A | B |\n| - | - |\n| *text):* | :* |\n| *text | :* |", pipeline);
        Assert.That(html, Does.Contain("<td><em>text):</em></td>\n<td>😗</td>"));
        Assert.That(html, Does.Contain("<td>*text</td>\n<td>😗</td>"));
    }

    [Test]
    public void UnambiguousDefaultMappingsAreUnchanged()
    {
        var shortcodes = EmojiMapping.GetDefaultEmojiShortcodeToUnicode();
        var smileys = EmojiMapping.GetDefaultSmileyToEmojiShortcode();
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley().Build();
        foreach (var mapping in shortcodes.Concat(smileys.Select(x => new KeyValuePair<string, string>(x.Key, shortcodes[x.Value]))))
        {
            var markdown = $"prefix {mapping.Key} suffix";
            Assert.AreEqual($"<p>prefix {mapping.Value} suffix</p>\n", Markdown.ToHtml(markdown, pipeline), markdown);
        }
    }

    [TestCase(":**", "long")]
    [TestCase("**:**", "<strong>:</strong>")]
    [TestCase("*", "star")]
    public void LongestMatchAndDelimiterOnlyMappingsArePreserved(string markdown, string expected)
    {
        var shortcodes = new Dictionary<string, string>
        {
            [":*"] = "short", [":**"] = "long"
        };
        // Only test delimiter-only mappings in isolation: they intentionally override emphasis.
        if (markdown == "*")
        {
            shortcodes.Add("*", "star");
        }
        var mapping = new EmojiMapping(shortcodes, new Dictionary<string, string>());
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley(customEmojiMapping: mapping).Build();
        Assert.AreEqual($"<p>prefix {expected}</p>\n", Markdown.ToHtml($"prefix {markdown}", pipeline));
    }

    [Test]
    public void OtherRenderersSeeResolvedEmojiAndEmphasis()
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley().Build();
        const string markdown = "*text):* :*";
        Assert.AreEqual("text): 😗", Markdown.ToPlainText(markdown, pipeline).Trim());
        Assert.AreEqual("*text):* 😗", Markdown.Normalize(markdown, pipeline: pipeline).Trim());
    }

    // Attributes bind to the emphasis delimiter, even if it ultimately remains unmatched.
    // Use a named shortcode to attach attributes to the surrounding paragraph instead.
    [TestCase(":*{.kiss}", "<p>😗</p>\n")]
    [TestCase(":kissing:{.kiss}", "<p class=\"kiss\">😗</p>\n")]
    [TestCase("*text):*{.label}", "<p><em class=\"label\">text):</em></p>\n")]
    public void GenericAttributesRespectResolvedSyntax(string markdown, string expected)
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmojiAndSmiley().UseGenericAttributes().Build();
        Assert.AreEqual(expected, Markdown.ToHtml(markdown, pipeline));
    }
}
