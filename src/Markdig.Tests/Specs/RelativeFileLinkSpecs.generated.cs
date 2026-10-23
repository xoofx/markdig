
// --------------------------------
//        Relative File Links
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.RelativeFileLinks
{
    [TestFixture]
    public class TestExtensionsRelativeFileLinks
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Relative File Links
        // 
        // The relative file link extension rewrites links to Markdown files into links to the HTML pages generated from them, so that the same files can be browsed on GitHub and as a generated site. It is enabled with `UseRelativeFileLinks()`.
        // 
        // A link whose path ends with `.md` or `.markdown`, compared case-insensitively, has that extension replaced with `.html`:
        [Test]
        public void ExtensionsRelativeFileLinks_Example001()
        {
            // Example 1
            // Section: Extensions / Relative File Links
            //
            // The following Markdown:
            //     [Usage](usage.md)
            //     
            //     [Read me](README.MD)
            //     
            //     [Guide](guide.markdown)
            //
            // Should be rendered as:
            //     <p><a href="usage.html">Usage</a></p>
            //     <p><a href="README.html">Read me</a></p>
            //     <p><a href="guide.html">Guide</a></p>

            TestParser.TestSpec("[Usage](usage.md)\n\n[Read me](README.MD)\n\n[Guide](guide.markdown)", "<p><a href=\"usage.html\">Usage</a></p>\n<p><a href=\"README.html\">Read me</a></p>\n<p><a href=\"guide.html\">Guide</a></p>", "relativefilelinks|advanced+relativefilelinks", context: "Example 1\nSection Extensions / Relative File Links\n");
        }

        // The query and the fragment are kept:
        [Test]
        public void ExtensionsRelativeFileLinks_Example002()
        {
            // Example 2
            // Section: Extensions / Relative File Links
            //
            // The following Markdown:
            //     [Preview](usage.md#preview)
            //     
            //     [Version](usage.md?v=1#preview)
            //
            // Should be rendered as:
            //     <p><a href="usage.html#preview">Preview</a></p>
            //     <p><a href="usage.html?v=1#preview">Version</a></p>

            TestParser.TestSpec("[Preview](usage.md#preview)\n\n[Version](usage.md?v=1#preview)", "<p><a href=\"usage.html#preview\">Preview</a></p>\n<p><a href=\"usage.html?v=1#preview\">Version</a></p>", "relativefilelinks|advanced+relativefilelinks", context: "Example 2\nSection Extensions / Relative File Links\n");
        }

        // Relative and root-relative paths are rewritten, and so are the targets of reference links:
        [Test]
        public void ExtensionsRelativeFileLinks_Example003()
        {
            // Example 3
            // Section: Extensions / Relative File Links
            //
            // The following Markdown:
            //     [Usage](./usage.md)
            //     
            //     [Install](../guide/install.md)
            //     
            //     [Docs](/docs/usage.md)
            //     
            //     [Linux][linux]
            //     
            //     [linux]: guide/install.md#linux
            //
            // Should be rendered as:
            //     <p><a href="./usage.html">Usage</a></p>
            //     <p><a href="../guide/install.html">Install</a></p>
            //     <p><a href="/docs/usage.html">Docs</a></p>
            //     <p><a href="guide/install.html#linux">Linux</a></p>

            TestParser.TestSpec("[Usage](./usage.md)\n\n[Install](../guide/install.md)\n\n[Docs](/docs/usage.md)\n\n[Linux][linux]\n\n[linux]: guide/install.md#linux", "<p><a href=\"./usage.html\">Usage</a></p>\n<p><a href=\"../guide/install.html\">Install</a></p>\n<p><a href=\"/docs/usage.html\">Docs</a></p>\n<p><a href=\"guide/install.html#linux\">Linux</a></p>", "relativefilelinks|advanced+relativefilelinks", context: "Example 3\nSection Extensions / Relative File Links\n");
        }

        // Only the link target changes. The link text and the title stay as written, and the target is escaped as usual:
        [Test]
        public void ExtensionsRelativeFileLinks_Example004()
        {
            // Example 4
            // Section: Extensions / Relative File Links
            //
            // The following Markdown:
            //     [usage.md](<my usage.md> "usage.md")
            //
            // Should be rendered as:
            //     <p><a href="my%20usage.html" title="usage.md">usage.md</a></p>

            TestParser.TestSpec("[usage.md](<my usage.md> \"usage.md\")", "<p><a href=\"my%20usage.html\" title=\"usage.md\">usage.md</a></p>", "relativefilelinks|advanced+relativefilelinks", context: "Example 4\nSection Extensions / Relative File Links\n");
        }

        // Links with a scheme and protocol-relative links point outside the site and are left unchanged, email autolinks included:
        [Test]
        public void ExtensionsRelativeFileLinks_Example005()
        {
            // Example 5
            // Section: Extensions / Relative File Links
            //
            // The following Markdown:
            //     [Source](https://github.com/xoofx/markdig/blob/main/readme.md)
            //     
            //     [CDN](//cdn.example.com/usage.md)
            //     
            //     [Mail](mailto:info@example.md)
            //     
            //     <info@example.md>
            //
            // Should be rendered as:
            //     <p><a href="https://github.com/xoofx/markdig/blob/main/readme.md">Source</a></p>
            //     <p><a href="//cdn.example.com/usage.md">CDN</a></p>
            //     <p><a href="mailto:info@example.md">Mail</a></p>
            //     <p><a href="mailto:info@example.md">info@example.md</a></p>

            TestParser.TestSpec("[Source](https://github.com/xoofx/markdig/blob/main/readme.md)\n\n[CDN](//cdn.example.com/usage.md)\n\n[Mail](mailto:info@example.md)\n\n<info@example.md>", "<p><a href=\"https://github.com/xoofx/markdig/blob/main/readme.md\">Source</a></p>\n<p><a href=\"//cdn.example.com/usage.md\">CDN</a></p>\n<p><a href=\"mailto:info@example.md\">Mail</a></p>\n<p><a href=\"mailto:info@example.md\">info@example.md</a></p>", "relativefilelinks|advanced+relativefilelinks", context: "Example 5\nSection Extensions / Relative File Links\n");
        }

        // A link is left unchanged when its path does not end with a Markdown extension, even if the query or the fragment does:
        [Test]
        public void ExtensionsRelativeFileLinks_Example006()
        {
            // Example 6
            // Section: Extensions / Relative File Links
            //
            // The following Markdown:
            //     [Notes](notes.html#usage.md)
            //     
            //     [Download](get.html?file=usage.md)
            //     
            //     [Anchor](#usage.md)
            //     
            //     [MDX](usage.mdx)
            //     
            //     [Folder](docs.md/)
            //
            // Should be rendered as:
            //     <p><a href="notes.html#usage.md">Notes</a></p>
            //     <p><a href="get.html?file=usage.md">Download</a></p>
            //     <p><a href="#usage.md">Anchor</a></p>
            //     <p><a href="usage.mdx">MDX</a></p>
            //     <p><a href="docs.md/">Folder</a></p>

            TestParser.TestSpec("[Notes](notes.html#usage.md)\n\n[Download](get.html?file=usage.md)\n\n[Anchor](#usage.md)\n\n[MDX](usage.mdx)\n\n[Folder](docs.md/)", "<p><a href=\"notes.html#usage.md\">Notes</a></p>\n<p><a href=\"get.html?file=usage.md\">Download</a></p>\n<p><a href=\"#usage.md\">Anchor</a></p>\n<p><a href=\"usage.mdx\">MDX</a></p>\n<p><a href=\"docs.md/\">Folder</a></p>", "relativefilelinks|advanced+relativefilelinks", context: "Example 6\nSection Extensions / Relative File Links\n");
        }

        // Links written as raw HTML are not rewritten:
        [Test]
        public void ExtensionsRelativeFileLinks_Example007()
        {
            // Example 7
            // Section: Extensions / Relative File Links
            //
            // The following Markdown:
            //     <a href="usage.md">Usage</a>
            //
            // Should be rendered as:
            //     <p><a href="usage.md">Usage</a></p>

            TestParser.TestSpec("<a href=\"usage.md\">Usage</a>", "<p><a href=\"usage.md\">Usage</a></p>", "relativefilelinks|advanced+relativefilelinks", context: "Example 7\nSection Extensions / Relative File Links\n");
        }
    }
}
