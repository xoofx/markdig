// Generated: 2019-04-05 16:06:14

// --------------------------------
//          Hardline Breaks
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.HardlineBreaks
{
    [TestFixture]
    public class TestExtensionsHardlineBreak
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Hardline break
        // 
        // When this extension is used, a new line in a paragraph block will result in a hardline break `<br>`: 
        [Test]
        public void ExtensionsHardlineBreak_Example001()
        {
            // Example 1
            // Section: Extensions / Hardline break
            //
            // The following Markdown:
            //     This is a paragraph
            //     with a break inside
            //
            // Should be rendered as:
            //     <p>This is a paragraph<br />
            //     with a break inside</p>

            Console.WriteLine("Example 1\nSection Extensions / Hardline break\n");
            TestParser.TestSpec("This is a paragraph\nwith a break inside", "<p>This is a paragraph<br />\nwith a break inside</p>", "hardlinebreak|advanced+hardlinebreak");
        }
    }
}
