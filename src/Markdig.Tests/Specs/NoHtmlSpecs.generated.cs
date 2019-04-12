// Generated: 2019-04-05 16:06:14

// --------------------------------
//              No Html
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.NoHtml
{
    [TestFixture]
    public class TestExtensionsNoHTML
    {
        // # Extensions
        // 
        // ## NoHTML
        // 
        // The extension DisableHtml allows to disable the parsing of HTML:
        // 
        // For inline HTML:
        [Test]
        public void ExtensionsNoHTML_Example001()
        {
            // Example 1
            // Section: Extensions / NoHTML
            //
            // The following Markdown:
            //     this is some text</td></tr>
            //
            // Should be rendered as:
            //     <p>this is some text&lt;/td&gt;&lt;/tr&gt;</p>

            Console.WriteLine("Example 1\nSection Extensions / NoHTML\n");
            TestParser.TestSpec("this is some text</td></tr>", "<p>this is some text&lt;/td&gt;&lt;/tr&gt;</p>", "nohtml");
        }

        // For Block HTML:
        [Test]
        public void ExtensionsNoHTML_Example002()
        {
            // Example 2
            // Section: Extensions / NoHTML
            //
            // The following Markdown:
            //     <div>
            //     this is some text
            //     </div>
            //
            // Should be rendered as:
            //     <p>&lt;div&gt;
            //     this is some text
            //     &lt;/div&gt;</p>

            Console.WriteLine("Example 2\nSection Extensions / NoHTML\n");
            TestParser.TestSpec("<div>\nthis is some text\n</div>", "<p>&lt;div&gt;\nthis is some text\n&lt;/div&gt;</p>", "nohtml");
        }
    }
}
