// Generated: 2019-04-15 05:23:49

// --------------------------------
//             Bootstrap
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Bootstrap
{
    [TestFixture]
    public class TestExtensionsBootstrap
    {
        // # Extensions
        // 
        // Adds support for outputting bootstrap ready tags:
        // 
        // ## Bootstrap
        //  
        // Adds bootstrap `.table` class to `<table>`:
        [Test]
        public void ExtensionsBootstrap_Example001()
        {
            // Example 1
            // Section: Extensions / Bootstrap
            //
            // The following Markdown:
            //     Name | Value
            //     -----| -----
            //     Abc  | 16
            //
            // Should be rendered as:
            //     <table class="table">
            //     <thead>
            //     <tr>
            //     <th>Name</th>
            //     <th>Value</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>Abc</td>
            //     <td>16</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 1\nSection Extensions / Bootstrap\n");
            TestParser.TestSpec("Name | Value\n-----| -----\nAbc  | 16", "<table class=\"table\">\n<thead>\n<tr>\n<th>Name</th>\n<th>Value</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>Abc</td>\n<td>16</td>\n</tr>\n</tbody>\n</table>", "bootstrap+pipetables+figures+attributes");
        }

        // Adds bootstrap `.blockquote` class to `<blockquote>`:
        [Test]
        public void ExtensionsBootstrap_Example002()
        {
            // Example 2
            // Section: Extensions / Bootstrap
            //
            // The following Markdown:
            //     > This is a blockquote
            //
            // Should be rendered as:
            //     <blockquote class="blockquote">
            //     <p>This is a blockquote</p>
            //     </blockquote>

            Console.WriteLine("Example 2\nSection Extensions / Bootstrap\n");
            TestParser.TestSpec("> This is a blockquote", "<blockquote class=\"blockquote\">\n<p>This is a blockquote</p>\n</blockquote>", "bootstrap+pipetables+figures+attributes");
        }

        // Adds bootstrap `.figure` class to `<figure>` and `.figure-caption` to `<figcaption>`
        [Test]
        public void ExtensionsBootstrap_Example003()
        {
            // Example 3
            // Section: Extensions / Bootstrap
            //
            // The following Markdown:
            //     ^^^
            //     This is a text in a caption
            //     ^^^ This is the caption
            //
            // Should be rendered as:
            //     <figure class="figure">
            //     <p>This is a text in a caption</p>
            //     <figcaption class="figure-caption">This is the caption</figcaption>
            //     </figure>

            Console.WriteLine("Example 3\nSection Extensions / Bootstrap\n");
            TestParser.TestSpec("^^^\nThis is a text in a caption\n^^^ This is the caption", "<figure class=\"figure\">\n<p>This is a text in a caption</p>\n<figcaption class=\"figure-caption\">This is the caption</figcaption>\n</figure>", "bootstrap+pipetables+figures+attributes");
        }

        // Adds the `.img-fluid` class to all image links `<img>`
        [Test]
        public void ExtensionsBootstrap_Example004()
        {
            // Example 4
            // Section: Extensions / Bootstrap
            //
            // The following Markdown:
            //     ![Image Link](/url)
            //
            // Should be rendered as:
            //     <p><img src="/url" class="img-fluid" alt="Image Link" /></p>

            Console.WriteLine("Example 4\nSection Extensions / Bootstrap\n");
            TestParser.TestSpec("![Image Link](/url)", "<p><img src=\"/url\" class=\"img-fluid\" alt=\"Image Link\" /></p>", "bootstrap+pipetables+figures+attributes");
        }
    }
}
