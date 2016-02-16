using System;
using System.Collections.Generic;
using System.IO;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters
{
    public class HtmlFormatter
    {
        private readonly TextWriter writer;

        private readonly Dictionary<Type, Action<object>> registeredWriters;

        public HtmlFormatter(TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            this.writer = writer;
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
            };
        }

        public void Write(Document document)
        {
            Write((ContainerBlock) document);
        }

        protected void Write(ListBlock listBlock)
        {
            writer.Write("<ul>\n");
            Write((ContainerBlock)listBlock);
            writer.Write("</ul>\n");
        }

        protected void Write(ListItemBlock listBlockItem)
        {
            writer.Write("<li>");
            Write((ContainerBlock)listBlockItem, true);
            writer.Write("</li>\n");
        }

        protected void Write(FencedCodeBlock fencedCodeBlock)
        {
            Write((CodeBlock) fencedCodeBlock);
        }

        protected void Write(CodeBlock codeBlock)
        {
            writer.Write("<pre><code>");
            Write((LeafBlock) codeBlock);
            writer.Write("\n</code></pre>\n");
        }

        protected void Write(HtmlBlock codeBlock)
        {
            Write((LeafBlock)codeBlock);
            writer.Write("\n");
        }

        protected void Write(HeadingBlock headingBlock)
        {
            writer.Write("<h");
            writer.Write(headingBlock.Level);
            writer.Write(">");
            Write((LeafBlock)headingBlock);
            writer.Write("</h");
            writer.Write(headingBlock.Level);
            writer.Write(">\n");
        }

        protected void Write(BreakBlock breakBlock)
        {
            writer.Write("<hr />\n");
        }

        protected void Write(QuoteBlock quoteBlock)
        {
            writer.Write("<blockquote>\n");
            Write((ContainerBlock) quoteBlock);
            writer.Write("</blockquote>\n");
        }

        protected void Write(ParagraphBlock paragraph)
        {
            writer.Write("<p>");
            Write((LeafBlock)paragraph);
            writer.Write("</p>\n");
        }

        protected void Write(LeafBlock leafBlock)
        {
            if (leafBlock.Inline != null)
            {
                Write(leafBlock.Inline);
            }
        }

        protected void Write(ContainerBlock container, bool simplifyFirstParagraph = false)
        {
            if (container.Children.Count == 1 && simplifyFirstParagraph)
            {
                var paragraph = container.Children[0] as ParagraphBlock;
                if (paragraph != null && paragraph.Inline.Count == 1)
                {
                    Write((LeafBlock)paragraph);
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

        protected void Write(Inline inline)
        {
            for (int i = 0; i < inline.Count; i++)
            {
                if (i > 0)
                {
                    writer.Write("\n");
                }
                var line = inline[i];
                writer.Write(line);
            }
        }
    }
}