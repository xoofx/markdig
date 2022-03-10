
// --------------------------------
//    Figures, Footers and Cites
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.FiguresFootersAndCites
{
    [TestFixture]
    public class TestExtensionsFigures
    {
        // # Extensions
        // 
        // The following the figure extension:
        // 
        // ## Figures
        //  
        // A figure can be defined by using a pattern equivalent to a fenced code block but with the character `^`
        [Test]
        public void ExtensionsFigures_Example001()
        {
            // Example 1
            // Section: Extensions / Figures
            //
            // The following Markdown:
            //     ^^^
            //     This is a figure
            //     ^^^ This is a *caption*
            //
            // Should be rendered as:
            //     <figure>
            //     <p>This is a figure</p>
            //     <figcaption>This is a <em>caption</em></figcaption>
            //     </figure>

            TestParser.TestSpec("^^^\nThis is a figure\n^^^ This is a *caption*", "<figure>\n<p>This is a figure</p>\n<figcaption>This is a <em>caption</em></figcaption>\n</figure>", "figures+footers+citations|advanced", context: "Example 1\nSection Extensions / Figures\n");
        }
    }

    [TestFixture]
    public class TestExtensionsFooters
    {
        // ## Footers
        // 
        // A footer equivalent to a block quote parsing but starts with double character ^^
        [Test]
        public void ExtensionsFooters_Example002()
        {
            // Example 2
            // Section: Extensions / Footers
            //
            // The following Markdown:
            //     ^^ This is a footer
            //     ^^ multi-line
            //
            // Should be rendered as:
            //     <footer>This is a footer
            //     multi-line</footer>

            TestParser.TestSpec("^^ This is a footer\n^^ multi-line", "<footer>This is a footer\nmulti-line</footer>", "figures+footers+citations|advanced", context: "Example 2\nSection Extensions / Footers\n");
        }
    }

    [TestFixture]
    public class TestExtensionsCite
    {
        // ## Cite
        // 
        // A cite is working like an emphasis but using the double character ""
        [Test]
        public void ExtensionsCite_Example003()
        {
            // Example 3
            // Section: Extensions / Cite
            //
            // The following Markdown:
            //     This is a ""citation of someone""
            //
            // Should be rendered as:
            //     <p>This is a <cite>citation of someone</cite></p>

            TestParser.TestSpec("This is a \"\"citation of someone\"\"", "<p>This is a <cite>citation of someone</cite></p>", "figures+footers+citations|advanced", context: "Example 3\nSection Extensions / Cite\n");
        }
    }
}
