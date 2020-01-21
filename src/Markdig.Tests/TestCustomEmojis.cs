using System.Collections.Generic;
using Markdig.Extensions.Emoji;
using NUnit.Framework;

namespace Markdig.Tests
{
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
    }
}