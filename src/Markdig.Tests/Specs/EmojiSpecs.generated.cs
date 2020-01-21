// Generated: 2020-01-13 21:08:58

// --------------------------------
//               Emoji
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Emoji
{
    [TestFixture]
    public class TestExtensionsEmoji
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Emoji
        // 
        // Emoji shortcodes and smileys can be converted to their respective unicode characters:
        [Test]
        public void ExtensionsEmoji_Example001()
        {
            // Example 1
            // Section: Extensions / Emoji
            //
            // The following Markdown:
            //     This is a test with a :) and a :angry: smiley
            //
            // Should be rendered as:
            //     <p>This is a test with a 😃 and a 😠 smiley</p>

            Console.WriteLine("Example 1\nSection Extensions / Emoji\n");
            TestParser.TestSpec("This is a test with a :) and a :angry: smiley", "<p>This is a test with a 😃 and a 😠 smiley</p>", "emojis|advanced+emojis");
        }

        // An emoji needs to be preceded by a space:
        [Test]
        public void ExtensionsEmoji_Example002()
        {
            // Example 2
            // Section: Extensions / Emoji
            //
            // The following Markdown:
            //     These are not:) an emoji with a:) x:angry:x
            //
            // Should be rendered as:
            //     <p>These are not:) an emoji with a:) x:angry:x</p>

            Console.WriteLine("Example 2\nSection Extensions / Emoji\n");
            TestParser.TestSpec("These are not:) an emoji with a:) x:angry:x", "<p>These are not:) an emoji with a:) x:angry:x</p>", "emojis|advanced+emojis");
        }

        // Emojis can be followed by close punctuation (or any other characters):
        [Test]
        public void ExtensionsEmoji_Example003()
        {
            // Example 3
            // Section: Extensions / Emoji
            //
            // The following Markdown:
            //     We all need :), it makes us :muscle:. (and :ok_hand:).
            //
            // Should be rendered as:
            //     <p>We all need 😃, it makes us 💪. (and 👌).</p>

            Console.WriteLine("Example 3\nSection Extensions / Emoji\n");
            TestParser.TestSpec("We all need :), it makes us :muscle:. (and :ok_hand:).", "<p>We all need 😃, it makes us 💪. (and 👌).</p>", "emojis|advanced+emojis");
        }

        // Sentences can end with emojis:
        [Test]
        public void ExtensionsEmoji_Example004()
        {
            // Example 4
            // Section: Extensions / Emoji
            //
            // The following Markdown:
            //     This is a sentence :ok_hand:
            //     and keeps going to the next line :)
            //
            // Should be rendered as:
            //     <p>This is a sentence 👌
            //     and keeps going to the next line 😃</p>

            Console.WriteLine("Example 4\nSection Extensions / Emoji\n");
            TestParser.TestSpec("This is a sentence :ok_hand:\nand keeps going to the next line :)", "<p>This is a sentence 👌\nand keeps going to the next line 😃</p>", "emojis|advanced+emojis");
        }
    }
}
