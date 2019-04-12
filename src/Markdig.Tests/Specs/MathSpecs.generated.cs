// Generated: 2019-04-05 16:06:14

// --------------------------------
//               Math
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Math
{
    [TestFixture]
    public class TestExtensionsMathInline
    {
        // # Extensions
        // 
        // Adds support for mathematics spans:
        // 
        // ## Math Inline
        //  
        // Allows to define a mathematic block embraced by `$...$`
        [Test]
        public void ExtensionsMathInline_Example001()
        {
            // Example 1
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $math block$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\(math block\)</span></p>

            Console.WriteLine("Example 1\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $math block$", "<p>This is a <span class=\"math\">\\(math block\\)</span></p>", "mathematics|advanced");
        }

        // Or by `$$...$$` embracing it by:
        [Test]
        public void ExtensionsMathInline_Example002()
        {
            // Example 2
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $$math block$$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\(math block\)</span></p>

            Console.WriteLine("Example 2\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $$math block$$", "<p>This is a <span class=\"math\">\\(math block\\)</span></p>", "mathematics|advanced");
        }

        // Newlines inside an inline math are not allowed:
        [Test]
        public void ExtensionsMathInline_Example003()
        {
            // Example 3
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is not a $$math 
            //     block$$ and? this is a $$math block$$
            //
            // Should be rendered as:
            //     <p>This is not a $$math
            //     block$$ and? this is a <span class="math">\(math block\)</span></p>

            Console.WriteLine("Example 3\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is not a $$math \nblock$$ and? this is a $$math block$$", "<p>This is not a $$math\nblock$$ and? this is a <span class=\"math\">\\(math block\\)</span></p>", "mathematics|advanced");
        }

        [Test]
        public void ExtensionsMathInline_Example004()
        {
            // Example 4
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is not a $math 
            //     block$ and? this is a $math block$
            //
            // Should be rendered as:
            //     <p>This is not a $math
            //     block$ and? this is a <span class="math">\(math block\)</span></p>

            Console.WriteLine("Example 4\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is not a $math \nblock$ and? this is a $math block$", "<p>This is not a $math\nblock$ and? this is a <span class=\"math\">\\(math block\\)</span></p>", "mathematics|advanced");
        }

        // An opening `$` can be followed by a space if the closing is also preceded by a space `$`:
        [Test]
        public void ExtensionsMathInline_Example005()
        {
            // Example 5
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $ math block $
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\(math block\)</span></p>

            Console.WriteLine("Example 5\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $ math block $", "<p>This is a <span class=\"math\">\\(math block\\)</span></p>", "mathematics|advanced");
        }

        [Test]
        public void ExtensionsMathInline_Example006()
        {
            // Example 6
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $    math block     $ after
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\(math block\)</span> after</p>

            Console.WriteLine("Example 6\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $    math block     $ after", "<p>This is a <span class=\"math\">\\(math block\\)</span> after</p>", "mathematics|advanced");
        }

        [Test]
        public void ExtensionsMathInline_Example007()
        {
            // Example 7
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $$    math block     $$ after
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\(math block\)</span> after</p>

            Console.WriteLine("Example 7\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $$    math block     $$ after", "<p>This is a <span class=\"math\">\\(math block\\)</span> after</p>", "mathematics|advanced");
        }

        [Test]
        public void ExtensionsMathInline_Example008()
        {
            // Example 8
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a not $ math block$ because there is not a whitespace before the closing
            //
            // Should be rendered as:
            //     <p>This is a not $ math block$ because there is not a whitespace before the closing</p>

            Console.WriteLine("Example 8\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a not $ math block$ because there is not a whitespace before the closing", "<p>This is a not $ math block$ because there is not a whitespace before the closing</p>", "mathematics|advanced");
        }

        // For the opening `$` it requires a space or a punctuation before (but cannot be used within a word):
        [Test]
        public void ExtensionsMathInline_Example009()
        {
            // Example 9
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is not a m$ath block$
            //
            // Should be rendered as:
            //     <p>This is not a m$ath block$</p>

            Console.WriteLine("Example 9\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is not a m$ath block$", "<p>This is not a m$ath block$</p>", "mathematics|advanced");
        }

        // For the closing `$` it requires a space after or a punctuation (but cannot be preceded by a space and cannot be used within a word):
        [Test]
        public void ExtensionsMathInline_Example010()
        {
            // Example 10
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is not a $math bloc$k
            //
            // Should be rendered as:
            //     <p>This is not a $math bloc$k</p>

            Console.WriteLine("Example 10\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is not a $math bloc$k", "<p>This is not a $math bloc$k</p>", "mathematics|advanced");
        }

        // For the closing `$` it requires a space after or a punctuation (but cannot be preceded by a space and cannot be used within a word):
        [Test]
        public void ExtensionsMathInline_Example011()
        {
            // Example 11
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is should not match a 16$ or a $15
            //
            // Should be rendered as:
            //     <p>This is should not match a 16$ or a $15</p>

            Console.WriteLine("Example 11\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is should not match a 16$ or a $15", "<p>This is should not match a 16$ or a $15</p>", "mathematics|advanced");
        }

        // A `$` can be escaped between a math inline block by using the escape `\\` 
        [Test]
        public void ExtensionsMathInline_Example012()
        {
            // Example 12
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $math \$ block$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\(math \$ block\)</span></p>

            Console.WriteLine("Example 12\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $math \\$ block$", "<p>This is a <span class=\"math\">\\(math \\$ block\\)</span></p>", "mathematics|advanced");
        }

        // At most, two `$` will be matched for the opening and closing:
        [Test]
        public void ExtensionsMathInline_Example013()
        {
            // Example 13
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $$$math block$$$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\($math block$\)</span></p>

            Console.WriteLine("Example 13\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $$$math block$$$", "<p>This is a <span class=\"math\">\\($math block$\\)</span></p>", "mathematics|advanced");
        }

        // Regular text can come both before and after the math inline
        [Test]
        public void ExtensionsMathInline_Example014()
        {
            // Example 14
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is a $math block$ with text on both sides.
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">\(math block\)</span> with text on both sides.</p>

            Console.WriteLine("Example 14\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is a $math block$ with text on both sides.", "<p>This is a <span class=\"math\">\\(math block\\)</span> with text on both sides.</p>", "mathematics|advanced");
        }

        // A mathematic block takes precedence over standard emphasis `*` `_`:
        [Test]
        public void ExtensionsMathInline_Example015()
        {
            // Example 15
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     This is *a $math* block$
            //
            // Should be rendered as:
            //     <p>This is *a <span class="math">\(math* block\)</span></p>

            Console.WriteLine("Example 15\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("This is *a $math* block$", "<p>This is *a <span class=\"math\">\\(math* block\\)</span></p>", "mathematics|advanced");
        }

        // An opening $$ at the beginning of a line should not be interpreted as a Math block:
        [Test]
        public void ExtensionsMathInline_Example016()
        {
            // Example 16
            // Section: Extensions / Math Inline
            //
            // The following Markdown:
            //     $$ math $$ starting at a line
            //
            // Should be rendered as:
            //     <p><span class="math">\(math\)</span> starting at a line</p>

            Console.WriteLine("Example 16\nSection Extensions / Math Inline\n");
            TestParser.TestSpec("$$ math $$ starting at a line", "<p><span class=\"math\">\\(math\\)</span> starting at a line</p>", "mathematics|advanced");
        }
    }

    [TestFixture]
    public class TestExtensionsMathBlock
    {
        // ## Math Block
        // 
        // The match block can spawn on multiple lines by having a $$ starting on a line.
        // It is working as a fenced code block.
        [Test]
        public void ExtensionsMathBlock_Example017()
        {
            // Example 17
            // Section: Extensions / Math Block
            //
            // The following Markdown:
            //     $$
            //     \begin{equation}
            //       \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
            //       \label{eq:sample}
            //     \end{equation}
            //     $$
            //
            // Should be rendered as:
            //     <div class="math">
            //     \[
            //     \begin{equation}
            //       \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
            //       \label{eq:sample}
            //     \end{equation}
            //     \]</div>

            Console.WriteLine("Example 17\nSection Extensions / Math Block\n");
            TestParser.TestSpec("$$\n\\begin{equation}\n  \\int_0^\\infty \\frac{x^3}{e^x-1}\\,dx = \\frac{\\pi^4}{15}\n  \\label{eq:sample}\n\\end{equation}\n$$", "<div class=\"math\">\n\\[\n\\begin{equation}\n  \\int_0^\\infty \\frac{x^3}{e^x-1}\\,dx = \\frac{\\pi^4}{15}\n  \\label{eq:sample}\n\\end{equation}\n\\]</div>", "mathematics|advanced");
        }
    }
}
