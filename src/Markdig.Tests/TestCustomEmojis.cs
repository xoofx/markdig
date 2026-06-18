using Markdig.Extensions.Emoji;

namespace Markdig.Tests;

[TestFixture]
public class TestCustomEmojis
{
    [Test]
    [TestCase(":smiley:", "<p>♥</p>\n")]
    [TestCase(":confused:", "<p>:confused:</p>\n")] // default emoji does not work
    [TestCase(":/", "<p>:/</p>\n")] // default smiley does not work
    public void TestCustomEmoji(string input, string expected)
    {
        var emojiToUnicode = new Dictionary<string, string>();
        var smileyToEmoji = new Dictionary<string, string>();

        emojiToUnicode[":smiley:"] = "♥";

        var customMapping = new EmojiMapping(emojiToUnicode, smileyToEmoji);

        var pipeline = new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley(customEmojiMapping: customMapping)
            .Build();

        var actual = Markdown.ToHtml(input, pipeline);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase(":testheart:", "<p>♥</p>\n")]
    [TestCase("hello", "<p>♥</p>\n")]
    [TestCase(":confused:", "<p>:confused:</p>\n")] // default emoji does not work
    [TestCase(":/", "<p>:/</p>\n")] // default smiley does not work
    public void TestCustomSmiley(string input, string expected)
    {
        var emojiToUnicode = new Dictionary<string, string>();
        var smileyToEmoji = new Dictionary<string, string>();

        emojiToUnicode[":testheart:"] = "♥";
        smileyToEmoji["hello"] = ":testheart:";

        var customMapping = new EmojiMapping(emojiToUnicode, smileyToEmoji);

        var pipeline = new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley(customEmojiMapping: customMapping)
            .Build();

        var actual = Markdown.ToHtml(input, pipeline);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase(":smiley:", "<p>♥</p>\n")]
    [TestCase(":)", "<p>♥</p>\n")]
    [TestCase(":confused:", "<p>😕</p>\n")] // default emoji still works
    [TestCase(":/", "<p>😕</p>\n")] // default smiley still works
    public void TestOverrideDefaultWithCustomEmoji(string input, string expected)
    {
        var emojiToUnicode = EmojiMapping.GetDefaultEmojiShortcodeToUnicode();
        var smileyToEmoji = EmojiMapping.GetDefaultSmileyToEmojiShortcode();

        emojiToUnicode[":smiley:"] = "♥";

        var customMapping = new EmojiMapping(emojiToUnicode, smileyToEmoji);

        var pipeline = new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley(customEmojiMapping: customMapping)
            .Build();

        var actual = Markdown.ToHtml(input, pipeline);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase(":testheart:", "<p>♥</p>\n")]
    [TestCase("hello", "<p>♥</p>\n")]
    [TestCase(":confused:", "<p>😕</p>\n")] // default emoji still works
    [TestCase(":/", "<p>😕</p>\n")] // default smiley still works
    public void TestOverrideDefaultWithCustomSmiley(string input, string expected)
    {
        var emojiToUnicode = EmojiMapping.GetDefaultEmojiShortcodeToUnicode();
        var smileyToEmoji = EmojiMapping.GetDefaultSmileyToEmojiShortcode();

        emojiToUnicode[":testheart:"] = "♥";
        smileyToEmoji["hello"] = ":testheart:";

        var customMapping = new EmojiMapping(emojiToUnicode, smileyToEmoji);

        var pipeline = new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley(customEmojiMapping: customMapping)
            .Build();

        var actual = Markdown.ToHtml(input, pipeline);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestCustomEmojiValidation()
    {
        var emojiToUnicode = new Dictionary<string, string>();
        var smileyToEmoji = new Dictionary<string, string>();

        Assert.Throws<ArgumentNullException>(() => new EmojiMapping(null, smileyToEmoji));
        Assert.Throws<ArgumentNullException>(() => new EmojiMapping(emojiToUnicode, null));

        emojiToUnicode.Add("null-value", null);
        Assert.Throws<ArgumentException>(() => new EmojiMapping(emojiToUnicode, smileyToEmoji));
        emojiToUnicode.Clear();

        smileyToEmoji.Add("null-value", null);
        Assert.Throws<ArgumentException>(() => new EmojiMapping(emojiToUnicode, smileyToEmoji));
        smileyToEmoji.Clear();

        smileyToEmoji.Add("foo", "something-that-does-not-exist-in-emojiToUnicode");
        Assert.Throws<ArgumentException>(() => new EmojiMapping(emojiToUnicode, smileyToEmoji));
        smileyToEmoji.Clear();

        emojiToUnicode.Add("a", "aaa");
        emojiToUnicode.Add("b", "bbb");
        emojiToUnicode.Add("c", "ccc");
        smileyToEmoji.Add("a", "c"); // "a" already exists in emojiToUnicode
        Assert.Throws<ArgumentException>(() => new EmojiMapping(emojiToUnicode, smileyToEmoji));
    }

    [Test]
    [TestCase("|test|\n|-|\n|:x:|", "<table>\n<thead>\n<tr>\n<th>test</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>❌</td>\n</tr>\n</tbody>\n</table>\n")]
    [TestCase("|test|\n|-|\n| :x: |", "<table>\n<thead>\n<tr>\n<th>test</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>❌</td>\n</tr>\n</tbody>\n</table>\n")]
    [TestCase("|test|\n|-|\n|1:x:|", "<table>\n<thead>\n<tr>\n<th>test</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>1:x:</td>\n</tr>\n</tbody>\n</table>\n")]
    [TestCase("|test|\n|-|\n|w:x:y|", "<table>\n<thead>\n<tr>\n<th>test</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>w:x:y</td>\n</tr>\n</tbody>\n</table>\n")]
    public void TestEmojiInPipeTable(string input, string expected)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley()
            .UsePipeTables()
            .Build();

        var actual = Markdown.ToHtml(input, pipeline);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void TestEmojiDoesNotBreakPipeTableAlignment(bool useEmojiFirst)
    {
        const string input = "| Left | Center | Right |\n| ---- |:------:| -----:|\n| a | b | c |";
        const string expected = "<table>\n<thead>\n<tr>\n<th>Left</th>\n<th style=\"text-align: center;\">Center</th>\n<th style=\"text-align: right;\">Right</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>a</td>\n<td style=\"text-align: center;\">b</td>\n<td style=\"text-align: right;\">c</td>\n</tr>\n</tbody>\n</table>\n";

        var pipelineBuilder = new MarkdownPipelineBuilder();
        if (useEmojiFirst)
        {
            pipelineBuilder.UseEmojiAndSmiley().UsePipeTables();
        }
        else
        {
            pipelineBuilder.UsePipeTables().UseEmojiAndSmiley();
        }

        var pipeline = pipelineBuilder.Build();
        var actual = Markdown.ToHtml(input, pipeline);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestEmojiDoesNotBreakGridTableAlignment()
    {
        const string input = "+-----+:---:+-----+\n|  A  | :x: |  C  |\n+-----+-----+-----+";
        const string expected = "<table>\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<tbody>\n<tr>\n<td>A</td>\n<td style=\"text-align: center;\">❌</td>\n<td>C</td>\n</tr>\n</tbody>\n</table>\n";

        var pipeline = new MarkdownPipelineBuilder()
            .UseGridTables()
            .UseEmojiAndSmiley()
            .Build();

        var actual = Markdown.ToHtml(input, pipeline);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestPipeTableExtensionDoesNotSuppressNeutralFaceInParagraph()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UsePipeTables()
            .UseEmojiAndSmiley()
            .Build();

        var actual = Markdown.ToHtml("text :|", pipeline);
        Assert.AreEqual("<p>text 😐</p>\n", actual);
    }
}
