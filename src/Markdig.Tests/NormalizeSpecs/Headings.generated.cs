// Generated: 2019-04-05 16:06:14

// --------------------------------
//             Headings
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Normalize.Headings
{
    [TestFixture]
    public class TestHeadings
    {
        // # Headings
        [Test]
        public void Headings_Example001()
        {
            // Example 1
            // Section: Headings
            //
            // The following Markdown:
            //     # Heading 1
            //     
            //     ## Heading 2
            //     
            //     ### Heading 3
            //     
            //     #### Heading 4
            //     
            //     ##### Heading 5
            //     
            //     ###### Heading 6
            //
            // Should be rendered as:
            //     # Heading 1
            //     
            //     ## Heading 2
            //     
            //     ### Heading 3
            //     
            //     #### Heading 4
            //     
            //     ##### Heading 5
            //     
            //     ###### Heading 6

            Console.WriteLine("Example 1\nSection Headings\n");
            TestNormalize.TestSpec("# Heading 1\n\n## Heading 2\n\n### Heading 3\n\n#### Heading 4\n\n##### Heading 5\n\n###### Heading 6", "# Heading 1\n\n## Heading 2\n\n### Heading 3\n\n#### Heading 4\n\n##### Heading 5\n\n###### Heading 6", "");
        }

        [Test]
        public void Headings_Example002()
        {
            // Example 2
            // Section: Headings
            //
            // The following Markdown:
            //     ###### Heading
            //     
            //     Text after two newlines
            //
            // Should be rendered as:
            //     ###### Heading
            //     
            //     Text after two newlines

            Console.WriteLine("Example 2\nSection Headings\n");
            TestNormalize.TestSpec("###### Heading\n\nText after two newlines", "###### Heading\n\nText after two newlines", "");
        }

        [Test]
        public void Headings_Example003()
        {
            // Example 3
            // Section: Headings
            //
            // The following Markdown:
            //     Heading
            //     =======
            //     
            //     Text after two newlines
            //
            // Should be rendered as:
            //     # Heading
            //     
            //     Text after two newlines

            Console.WriteLine("Example 3\nSection Headings\n");
            TestNormalize.TestSpec("Heading\n=======\n\nText after two newlines", "# Heading\n\nText after two newlines", "");
        }
    }
}
