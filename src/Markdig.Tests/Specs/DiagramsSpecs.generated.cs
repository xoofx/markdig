// Generated: 2019-04-05 16:06:14

// --------------------------------
//             Diagrams
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Diagrams
{
    [TestFixture]
    public class TestExtensionsMermaidDiagrams
    {
        // # Extensions
        // 
        // Adds support for diagrams extension:
        // 
        // ## Mermaid diagrams
        //  
        // Using a fenced code block with the `mermaid` language info will output a `<div class='mermaid'>` instead of a `pre/code` block:
        [Test]
        public void ExtensionsMermaidDiagrams_Example001()
        {
            // Example 1
            // Section: Extensions / Mermaid diagrams
            //
            // The following Markdown:
            //     ```mermaid
            //     graph TD;
            //         A-->B;
            //         A-->C;
            //         B-->D;
            //         C-->D;
            //     ```
            //
            // Should be rendered as:
            //     <div class="mermaid">graph TD;
            //         A-->B;
            //         A-->C;
            //         B-->D;
            //         C-->D;
            //     </div>

            Console.WriteLine("Example 1\nSection Extensions / Mermaid diagrams\n");
            TestParser.TestSpec("```mermaid\ngraph TD;\n    A-->B;\n    A-->C;\n    B-->D;\n    C-->D;\n```", "<div class=\"mermaid\">graph TD;\n    A-->B;\n    A-->C;\n    B-->D;\n    C-->D;\n</div>", "diagrams|advanced");
        }
    }

    [TestFixture]
    public class TestExtensionsNomnomlDiagrams
    {
        // ## nomnoml diagrams
        // 
        // Using a fenced code block with the `nomnoml` language info will output a `<div class='nomnoml'>` instead of a `pre/code` block:
        [Test]
        public void ExtensionsNomnomlDiagrams_Example002()
        {
            // Example 2
            // Section: Extensions / nomnoml diagrams
            //
            // The following Markdown:
            //     ```nomnoml
            //     [example|
            //       propertyA: Int
            //       propertyB: string
            //     |
            //       methodA()
            //       methodB()
            //     |
            //       [subA]--[subB]
            //       [subA]-:>[sub C]
            //     ]
            //     ```
            //
            // Should be rendered as:
            //     <div class="nomnoml">[example|
            //       propertyA: Int
            //       propertyB: string
            //     |
            //       methodA()
            //       methodB()
            //     |
            //       [subA]--[subB]
            //       [subA]-:>[sub C]
            //     ]
            //     </div>

            Console.WriteLine("Example 2\nSection Extensions / nomnoml diagrams\n");
            TestParser.TestSpec("```nomnoml\n[example|\n  propertyA: Int\n  propertyB: string\n|\n  methodA()\n  methodB()\n|\n  [subA]--[subB]\n  [subA]-:>[sub C]\n]\n```", "<div class=\"nomnoml\">[example|\n  propertyA: Int\n  propertyB: string\n|\n  methodA()\n  methodB()\n|\n  [subA]--[subB]\n  [subA]-:>[sub C]\n]\n</div>", "diagrams|advanced");
        }
        // TODO: Add other text diagram languages
    }
}
