using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters
{
    public class HtmlFormatter
    {
        private readonly HtmlTextWriter writer;

        private readonly Dictionary<Type, Action<object>> registeredWriters;

        public HtmlFormatter(TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            writer.NewLine = "\n";
            this.writer = new HtmlTextWriter(writer);
            registeredWriters = new Dictionary<Type, Action<object>>
            {
                [typeof(ListBlock)] = o => Write((ListBlock)o),
                [typeof(ListItemBlock)] = o => Write((ListItemBlock)o),
                [typeof(FencedCodeBlock)] = o => Write((FencedCodeBlock)o),
                [typeof(CodeBlock)] = o => Write((CodeBlock)o),
                [typeof(HeadingBlock)] = o => Write((HeadingBlock)o),
                [typeof(BreakBlock)] = o => Write((BreakBlock)o),
                [typeof(QuoteBlock)] = o => Write((QuoteBlock)o),
                [typeof(ParagraphBlock)] = o => Write((ParagraphBlock)o),
                [typeof(HtmlBlock)] = o => Write((HtmlBlock)o),
                [typeof(EscapeInline)] = o => Write((EscapeInline)o),
                [typeof(LiteralInline)] = o => Write((LiteralInline)o),
                [typeof(CodeInline)] = o => Write((CodeInline)o),
                [typeof(TextLinkInline)] = o => Write((TextLinkInline)o),
                [typeof(ContainerInline)] = o => WriteChildren((ContainerInline)o),
            };
        }

        public void Write(Document document)
        {
            WriteContainer((ContainerBlock) document);
        }

        protected void Write(ListBlock listBlock)
        {
            writer.EnsureLine();
            writer.WriteLineConstant("<ul>");
            WriteContainer(listBlock);
            writer.WriteLineConstant("</ul>");
        }

        protected void Write(ListItemBlock listBlockItem)
        {
            writer.EnsureLine();
            writer.WriteConstant("<li>");
            WriteContainer(listBlockItem, true);
            writer.WriteLineConstant("</li>");
        }

        protected void Write(FencedCodeBlock fencedCodeBlock)
        {
            Write((CodeBlock) fencedCodeBlock);
        }

        protected void Write(CodeBlock codeBlock)
        {
            writer.EnsureLine();
            writer.WriteConstant("<pre><code>");
            WriteLeaf(codeBlock, true);
            writer.WriteLineConstant("</code></pre>");
        }

        protected void Write(HtmlBlock codeBlock)
        {
            WriteLeaf(codeBlock, true);
        }

        protected void Write(HeadingBlock headingBlock)
        {
            var heading = headingBlock.Level.ToString(CultureInfo.InvariantCulture);
            writer.WriteConstant("<h");
            writer.WriteConstant(heading);
            writer.WriteConstant(">");
            WriteLeaf(headingBlock, false);
            writer.WriteConstant("</h");
            writer.WriteConstant(heading);
            writer.WriteLineConstant(">");
        }

        protected void Write(BreakBlock breakBlock)
        {
            writer.WriteLineConstant("<hr />");
        }

        protected void Write(QuoteBlock quoteBlock)
        {
            writer.EnsureLine();
            writer.WriteLineConstant("<blockquote>");
            WriteContainer(quoteBlock);
            writer.WriteLineConstant("</blockquote>");
        }

        protected void Write(ParagraphBlock paragraph)
        {
            writer.EnsureLine();
            writer.WriteConstant("<p>");
            WriteLeaf(paragraph, false);
            writer.WriteLineConstant("</p>");
        }

        protected void Write(LiteralInline literal)
        {
            HtmlHelper.EscapeHtml(literal.Content, writer);
        }

        protected void Write(EscapeInline escape)
        {
            writer.WriteConstant(escape.EscapedChar);
        }

        protected void Write(CodeInline code)
        {
            writer.WriteConstant("<code>");
            writer.WriteConstant(code.Content);
            writer.WriteConstant("</code>");
        }

        protected void Write(TextLinkInline link)
        {
            writer.WriteConstant("<a href=\"");
            HtmlHelper.EscapeUrl(link.Url, writer);
            writer.WriteConstant("\"");
            if (!string.IsNullOrEmpty(link.Title))
            {
                writer.WriteConstant(" title=\"");
                HtmlHelper.EscapeHtml(link.Title, writer);
                writer.WriteConstant("\"");
            }
            writer.WriteConstant(">");
            WriteChildren((ContainerInline)link);
            writer.WriteConstant("</a>");
        }

        protected void WriteChildren(ContainerInline containerInline)
        {
            var inline = containerInline.FirstChild;
            while (inline != null)
            {
                Action<object> writerAction;
                if (registeredWriters.TryGetValue(inline.GetType(), out writerAction))
                {
                    writerAction(inline);
                }

                inline = inline.NextSibling;
            }
        }

        protected void WriteLeaf(LeafBlock leafBlock, bool writeEndOfLines)
        {
            var inline = leafBlock.Inline;
            if (inline != null)
            {
                while (inline != null)
                {
                    Action<object> writerAction;
                    if (registeredWriters.TryGetValue(inline.GetType(), out writerAction))
                    {
                        writerAction(inline);
                    }

                    inline = inline.NextSibling;
                }
            }
            else
            {
                var lines = leafBlock.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (!writeEndOfLines && i > 0)
                    {
                        writer.WriteLine();
                    }
                    var line = lines[i];
                    writer.WriteConstant(line.ToString());
                    if (writeEndOfLines)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        protected void WriteContainer(ContainerBlock container, bool simplifyFirstParagraph = false)
        {
            if (container.Children.Count == 1 && simplifyFirstParagraph)
            {
                var paragraph = container.Children[0] as ParagraphBlock;
                if (paragraph != null && paragraph.Lines.Count == 1)
                {
                    WriteLeaf((LeafBlock)paragraph, false);
                    return;
                }
            }

            foreach (var child in container.Children)
            {
                Action<object> writerAction;
                if (registeredWriters.TryGetValue(child.GetType(), out writerAction))
                {
                    writerAction(child);
                }
            }
        }
    }
}