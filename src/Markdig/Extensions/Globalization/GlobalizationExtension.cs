using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Collections.Generic;
using System.Linq;

namespace Markdig.Extensions.Globalization
{
    /// <summary>
    /// Extension to add support for RTL content.
    /// </summary>
    public class GlobalizationExtension : IMarkdownExtension
    {
        private const string Direction = "dir";
        private const string RightToLeft = "rtl";

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Make sure we don't have a delegate twice
            pipeline.DocumentProcessed -= Pipeline_DocumentProcessed;
            pipeline.DocumentProcessed += Pipeline_DocumentProcessed;
        }

        private void Pipeline_DocumentProcessed(MarkdownDocument document)
        {
            foreach (var node in document.Descendants())
            {
                if (node is TableRow || node is TableCell || node is ListItemBlock)
                    continue;

                var firstStrongChar = GetFirstStrongCharacter(node) ?? '\0';
                if (IsRightToLeft(firstStrongChar))
                {
                    var attributes = node.GetAttributes();
                    attributes.AddPropertyIfNotExist(Direction, RightToLeft);

                    if (node is Table table)
                    {
                        attributes.AddPropertyIfNotExist("align", "right");
                    }
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {

        }

        private char? GetFirstStrongCharacter(MarkdownObject item)
        {
            if (item is IEnumerable<MarkdownObject> container)
            {
                foreach (var child in container)
                {
                    var firstStrongChar = GetFirstStrongCharacter(child);
                    if (firstStrongChar != null)
                        return firstStrongChar;
                }
            }
            else if (item is LeafBlock leaf)
            {
                return GetFirstStrongCharacter(leaf.Inline);
            }
            else if (item is LiteralInline literal)
            {
                return literal.ToString().Trim().FirstOrDefault();
            }

            foreach (var descendant in item.Descendants())
            {
                if (descendant is ParagraphBlock p)
                {
                    foreach (var i in p.Inline)
                    {
                        if (i is LiteralInline l)
                        {
                            return l.ToString().FirstOrDefault();
                        }
                    }
                }
            }

            return null;
        }

        private bool IsRightToLeft(char c)
        {
            // Original tabel from: http://www.ietf.org/rfc/rfc3454.txt
            // D. Bidirectional tables
            return c >= 0x05D0 && c <= 0x05EA ||
                   c >= 0x05F0 && c <= 0x05F4 ||
                   c >= 0x0621 && c <= 0x063A ||
                   c >= 0x0640 && c <= 0x064A ||
                   c >= 0x066D && c <= 0x066F ||
                   c >= 0x0671 && c <= 0x06D5 ||
                   c >= 0x06E5 && c <= 0x06E6 ||
                   c >= 0x06FA && c <= 0x06FE ||
                   c >= 0x0700 && c <= 0x070D ||
                   c >= 0x0712 && c <= 0x072C ||
                   c >= 0x0780 && c <= 0x07A5 ||
                   c >= 0xFB1F && c <= 0xFB28 ||
                   c >= 0xFB2A && c <= 0xFB36 ||
                   c >= 0xFB38 && c <= 0xFB3C ||
                   c >= 0xFB40 && c <= 0xFB41 ||
                   c >= 0xFB43 && c <= 0xFB44 ||
                   c >= 0xFB46 && c <= 0xFBB1 ||
                   c >= 0xFBD3 && c <= 0xFD3D ||
                   c >= 0xFD50 && c <= 0xFD8F ||
                   c >= 0xFD92 && c <= 0xFDC7 ||
                   c >= 0xFDF0 && c <= 0xFDFC ||
                   c >= 0xFE70 && c <= 0xFE74 ||
                   c >= 0xFE76 && c <= 0xFEFC ||
                   c == 0x05BE || c == 0x05C0 ||
                   c == 0x05C3 || c == 0x061B ||
                   c == 0x061F || c == 0x06DD ||
                   c == 0x0710 || c == 0x07B1 ||
                   c == 0x200F || c == 0xFB1D ||
                   c == 0xFB3E;
        }
    }
}
