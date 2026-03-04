
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

            TestParser.TestSpec("This is a test with a :) and a :angry: smiley", "<p>This is a test with a 😃 and a 😠 smiley</p>", "pipetables+emojis|advanced+emojis", context: "Example 1\nSection Extensions / Emoji\n");
        }

        // An emoji must not be preceded by a letter or digit:
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

            TestParser.TestSpec("These are not:) an emoji with a:) x:angry:x", "<p>These are not:) an emoji with a:) x:angry:x</p>", "pipetables+emojis|advanced+emojis", context: "Example 2\nSection Extensions / Emoji\n");
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

            TestParser.TestSpec("We all need :), it makes us :muscle:. (and :ok_hand:).", "<p>We all need 😃, it makes us 💪. (and 👌).</p>", "pipetables+emojis|advanced+emojis", context: "Example 3\nSection Extensions / Emoji\n");
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

            TestParser.TestSpec("This is a sentence :ok_hand:\nand keeps going to the next line :)", "<p>This is a sentence 👌\nand keeps going to the next line 😃</p>", "pipetables+emojis|advanced+emojis", context: "Example 4\nSection Extensions / Emoji\n");
        }

        // Emojis are rendered inside pipe table cells with surrounding spaces:
        [Test]
        public void ExtensionsEmoji_Example005()
        {
            // Example 5
            // Section: Extensions / Emoji
            //
            // The following Markdown:
            //     | header |
            //     |--------|
            //     | :x: |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>header</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>❌</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| header |\n|--------|\n| :x: |", "<table>\n<thead>\n<tr>\n<th>header</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>❌</td>\n</tr>\n</tbody>\n</table>", "pipetables+emojis|advanced+emojis", context: "Example 5\nSection Extensions / Emoji\n");
        }

        // Emojis are rendered inside pipe table cells without surrounding spaces:
        [Test]
        public void ExtensionsEmoji_Example006()
        {
            // Example 6
            // Section: Extensions / Emoji
            //
            // The following Markdown:
            //     | header |
            //     |--------|
            //     |:x:|
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>header</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>❌</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| header |\n|--------|\n|:x:|", "<table>\n<thead>\n<tr>\n<th>header</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>❌</td>\n</tr>\n</tbody>\n</table>", "pipetables+emojis|advanced+emojis", context: "Example 6\nSection Extensions / Emoji\n");
        }
    }
}
