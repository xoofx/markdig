
// --------------------------------
//           Smarty Pants
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.SmartyPants
{
    [TestFixture]
    public class TestExtensionsSmartyPantsQuotes
    {
        // # Extensions
        // 
        // Adds support for smarty pants:
        // 
        // ## SmartyPants Quotes
        //  
        // Converts the following character to smarty pants:
        [Test]
        public void ExtensionsSmartyPantsQuotes_Example001()
        {
            // Example 1
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a "text"
            //
            // Should be rendered as:
            //     <p>This is a &ldquo;text&rdquo;</p>

            TestParser.TestSpec("This is a \"text\"", "<p>This is a &ldquo;text&rdquo;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 1\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example002()
        {
            // Example 2
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a 'text'
            //
            // Should be rendered as:
            //     <p>This is a &lsquo;text&rsquo;</p>

            TestParser.TestSpec("This is a 'text'", "<p>This is a &lsquo;text&rsquo;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 2\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example003()
        {
            // Example 3
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a <<text>>
            //
            // Should be rendered as:
            //     <p>This is a &laquo;text&raquo;</p>

            TestParser.TestSpec("This is a <<text>>", "<p>This is a &laquo;text&raquo;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 3\nSection Extensions / SmartyPants Quotes\n");
        }

        // Unbalanced quotes are not changed:
        [Test]
        public void ExtensionsSmartyPantsQuotes_Example004()
        {
            // Example 4
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a "text
            //
            // Should be rendered as:
            //     <p>This is a &quot;text</p>

            TestParser.TestSpec("This is a \"text", "<p>This is a &quot;text</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 4\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example005()
        {
            // Example 5
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a 'text
            //
            // Should be rendered as:
            //     <p>This is a 'text</p>

            TestParser.TestSpec("This is a 'text", "<p>This is a 'text</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 5\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example006()
        {
            // Example 6
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a <<text
            //
            // Should be rendered as:
            //     <p>This is a &lt;&lt;text</p>

            TestParser.TestSpec("This is a <<text", "<p>This is a &lt;&lt;text</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 6\nSection Extensions / SmartyPants Quotes\n");
        }

        // Unbalanced quotes inside other quotes are not changed:
        [Test]
        public void ExtensionsSmartyPantsQuotes_Example007()
        {
            // Example 7
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a "text 'with" a another text'
            //
            // Should be rendered as:
            //     <p>This is a &ldquo;text 'with&rdquo; a another text'</p>

            TestParser.TestSpec("This is a \"text 'with\" a another text'", "<p>This is a &ldquo;text 'with&rdquo; a another text'</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 7\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example008()
        {
            // Example 8
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is 'a "text 'with" a another text'
            //
            // Should be rendered as:
            //     <p>This is &lsquo;a &ldquo;text 'with&rdquo; a another text&rsquo;</p>

            TestParser.TestSpec("This is 'a \"text 'with\" a another text'", "<p>This is &lsquo;a &ldquo;text 'with&rdquo; a another text&rsquo;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 8\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example009()
        {
            // Example 9
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a 'text <<with' a another text>>
            //
            // Should be rendered as:
            //     <p>This is a &lsquo;text &lt;&lt;with&rsquo; a another text&gt;&gt;</p>

            TestParser.TestSpec("This is a 'text <<with' a another text>>", "<p>This is a &lsquo;text &lt;&lt;with&rsquo; a another text&gt;&gt;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 9\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example010()
        {
            // Example 10
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is a <<text 'with>> a another text'
            //
            // Should be rendered as:
            //     <p>This is a &laquo;text 'with&raquo; a another text'</p>

            TestParser.TestSpec("This is a <<text 'with>> a another text'", "<p>This is a &laquo;text 'with&raquo; a another text'</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 10\nSection Extensions / SmartyPants Quotes\n");
        }

        // Quotes requires to have the same rules than emphasis `_` regarding left/right frankling rules:
        [Test]
        public void ExtensionsSmartyPantsQuotes_Example011()
        {
            // Example 11
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     It's not quotes'
            //
            // Should be rendered as:
            //     <p>It's not quotes'</p>

            TestParser.TestSpec("It's not quotes'", "<p>It's not quotes'</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 11\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example012()
        {
            // Example 12
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     They are ' not matching quotes '
            //
            // Should be rendered as:
            //     <p>They are ' not matching quotes '</p>

            TestParser.TestSpec("They are ' not matching quotes '", "<p>They are ' not matching quotes '</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 12\nSection Extensions / SmartyPants Quotes\n");
        }

        [Test]
        public void ExtensionsSmartyPantsQuotes_Example013()
        {
            // Example 13
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     They are' not matching 'quotes
            //
            // Should be rendered as:
            //     <p>They are' not matching 'quotes</p>

            TestParser.TestSpec("They are' not matching 'quotes", "<p>They are' not matching 'quotes</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 13\nSection Extensions / SmartyPants Quotes\n");
        }

        // An emphasis starting inside left/right quotes will span over the right quote:
        [Test]
        public void ExtensionsSmartyPantsQuotes_Example014()
        {
            // Example 14
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     This is "a *text" with an emphasis*
            //
            // Should be rendered as:
            //     <p>This is &ldquo;a <em>text&rdquo; with an emphasis</em></p>

            TestParser.TestSpec("This is \"a *text\" with an emphasis*", "<p>This is &ldquo;a <em>text&rdquo; with an emphasis</em></p>", "pipetables+smartypants|advanced+smartypants", context: "Example 14\nSection Extensions / SmartyPants Quotes\n");
        }

        // Multiple sets of quotes can be used
        [Test]
        public void ExtensionsSmartyPantsQuotes_Example015()
        {
            // Example 15
            // Section: Extensions / SmartyPants Quotes
            //
            // The following Markdown:
            //     "aaa" "bbb" "ccc" "ddd"
            //
            // Should be rendered as:
            //     <p>&ldquo;aaa&rdquo; &ldquo;bbb&rdquo; &ldquo;ccc&rdquo; &ldquo;ddd&rdquo;</p>

            TestParser.TestSpec("\"aaa\" \"bbb\" \"ccc\" \"ddd\"", "<p>&ldquo;aaa&rdquo; &ldquo;bbb&rdquo; &ldquo;ccc&rdquo; &ldquo;ddd&rdquo;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 15\nSection Extensions / SmartyPants Quotes\n");
        }
    }

    [TestFixture]
    public class TestExtensionsSmartyPantsSeparators
    {
        // ## SmartyPants Separators
        [Test]
        public void ExtensionsSmartyPantsSeparators_Example016()
        {
            // Example 16
            // Section: Extensions / SmartyPants Separators
            //
            // The following Markdown:
            //     This is a -- text
            //
            // Should be rendered as:
            //     <p>This is a &ndash; text</p>

            TestParser.TestSpec("This is a -- text", "<p>This is a &ndash; text</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 16\nSection Extensions / SmartyPants Separators\n");
        }

        [Test]
        public void ExtensionsSmartyPantsSeparators_Example017()
        {
            // Example 17
            // Section: Extensions / SmartyPants Separators
            //
            // The following Markdown:
            //     This is a --- text
            //
            // Should be rendered as:
            //     <p>This is a &mdash; text</p>

            TestParser.TestSpec("This is a --- text", "<p>This is a &mdash; text</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 17\nSection Extensions / SmartyPants Separators\n");
        }

        [Test]
        public void ExtensionsSmartyPantsSeparators_Example018()
        {
            // Example 18
            // Section: Extensions / SmartyPants Separators
            //
            // The following Markdown:
            //     This is a en ellipsis...
            //
            // Should be rendered as:
            //     <p>This is a en ellipsis&hellip;</p>

            TestParser.TestSpec("This is a en ellipsis...", "<p>This is a en ellipsis&hellip;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 18\nSection Extensions / SmartyPants Separators\n");
        }

        // Check that a smartypants are not breaking pipetable parsing:
        [Test]
        public void ExtensionsSmartyPantsSeparators_Example019()
        {
            // Example 19
            // Section: Extensions / SmartyPants Separators
            //
            // The following Markdown:
            //     a  | b
            //     -- | --
            //     0  | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a  | b\n-- | --\n0  | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables+smartypants|advanced+smartypants", context: "Example 19\nSection Extensions / SmartyPants Separators\n");
        }

        // Check quotes and dash:
        [Test]
        public void ExtensionsSmartyPantsSeparators_Example020()
        {
            // Example 20
            // Section: Extensions / SmartyPants Separators
            //
            // The following Markdown:
            //     A "quote" with a ---
            //
            // Should be rendered as:
            //     <p>A &ldquo;quote&rdquo; with a &mdash;</p>

            TestParser.TestSpec("A \"quote\" with a ---", "<p>A &ldquo;quote&rdquo; with a &mdash;</p>", "pipetables+smartypants|advanced+smartypants", context: "Example 20\nSection Extensions / SmartyPants Separators\n");
        }
    }
}
