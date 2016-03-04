using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Renderers.Html.Inlines;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers
{
    public class HtmlRenderer : TextRendererBase<HtmlRenderer>
    {
        public HtmlRenderer(TextWriter writer) : base(writer)
        {
            // Default block renderers
            ObjectRenderers.Add(new CodeBlockRenderer());
            ObjectRenderers.Add(new ListRenderer());
            ObjectRenderers.Add(new HeadingRenderer());
            ObjectRenderers.Add(new HtmlBlockRenderer());
            ObjectRenderers.Add(new ParagraphRenderer());
            ObjectRenderers.Add(new QuoteBlockRenderer());
            ObjectRenderers.Add(new ThematicBreakRenderer());

            // Default inline renderers
            ObjectRenderers.Add(new AutolinkInlineRenderer());
            ObjectRenderers.Add(new CodeInlineRenderer());
            ObjectRenderers.Add(new DelimiterInlineRenderer());
            ObjectRenderers.Add(new EmphasisInlineRenderer());
            ObjectRenderers.Add(new HardlineBreakInlineRenderer());
            ObjectRenderers.Add(new SoftlineBreakInlineRenderer());
            ObjectRenderers.Add(new HtmlInlineRenderer());
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new LiteralInlineRenderer());

            EnableHtmlForInline = true;
        }

        public bool EnableHtmlForInline { get; set; }

        public bool ImplicitParagraph { get; set; }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public HtmlRenderer WriteEscape(string content)
        {
            if (string.IsNullOrEmpty(content))
                return this;

            WriteEscape(content, 0, content.Length);
            return this;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public HtmlRenderer WriteEscape(ref StringSlice slice)
        {
            if (slice.Start > slice.End)
            {
                return this;
            }
            return WriteEscape(slice.Text, slice.Start, slice.Length);
        }

        public HtmlRenderer WriteEscape(string content, int offset, int length)
        {
            if (string.IsNullOrEmpty(content) || length == 0)
                return this;

            var end = offset + length;
            int previousOffset = offset;
            for (;offset < end;  offset++)
            {
                switch (content[offset])
                {
                    case '<':
                        Write(content, previousOffset, offset - previousOffset);
                        Write("&lt;");
                        previousOffset = offset + 1;
                        break;
                    case '>':
                        Write(content, previousOffset, offset - previousOffset);
                        Write("&gt;");
                        previousOffset = offset + 1;
                        break;
                    case '&':
                        Write(content, previousOffset, offset - previousOffset);
                        Write("&amp;");
                        previousOffset = offset + 1;
                        break;
                    case '"':
                        Write(content, previousOffset, offset - previousOffset);
                        Write("&quot;");
                        previousOffset = offset + 1;
                        break;
                }
            }

            Write(content, previousOffset, end - previousOffset);
            return this;
        }

        public HtmlRenderer WriteEscapeUrl(string content)
        {
            if (content == null)
                return this;

            int previousPosition = 0;
            int length = content.Length;

            for (var i = 0; i < length; i++)
            {
                var c = content[i];

                if (c < 128)
                {
                    var escape = HtmlHelper.EscapeUrlCharacter(c);
                    if (escape != null)
                    {
                        Write(content, previousPosition, i - previousPosition);
                        previousPosition = i + 1;
                        Write(escape);
                    }
                }
                else
                {
                    Write(content, previousPosition, i - previousPosition);
                    previousPosition = i + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && previousPosition < length)
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c, content[previousPosition] });
                        // Skip next char as it is decoded above
                        i++;
                        previousPosition = i + 1;
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c });
                    }

                    for (var j = 0; j < bytes.Length; j++)
                    {
                        Write($"%{bytes[j]:X2}");
                    }
                }
            }

            Write(content, previousPosition, length - previousPosition);
            return this;
        }

        public HtmlRenderer WriteAttributes(MarkdownObject obj)
        {
            var attributes = obj.TryGetAttributes();
            if (attributes != null)
            {
                if (attributes.Id != null)
                {
                    Write($" Id=\"{attributes.Id}\"");
                }

                if (attributes.Classes != null && attributes.Classes.Count > 0)
                {
                    Write($" class=\"");
                    for (int i = 0; i < attributes.Classes.Count; i++)
                    {
                        var cssClass = attributes.Classes[i];
                        if (i > 0)
                        {
                            Write(" ");
                        }
                        Write(cssClass);
                    }
                    Write("\"");
                }

                if (attributes.Properties != null && attributes.Properties.Count > 0)
                {
                    foreach (var property in attributes.Properties)
                    {
                        Write(" ");
                        Write(property.Key).Write("=").Write("\"");
                        WriteEscape(property.Value);
                        Write("\"");
                    }
                }
            }

            return this;
        }

        public HtmlRenderer WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape)
        {
            if (leafBlock.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (!writeEndOfLines && i > 0)
                    {
                        WriteLine();
                    }
                    if (escape)
                    {
                        WriteEscape(ref slices[i].Slice);
                    }
                    else
                    {
                        Write(ref slices[i].Slice);
                    }
                    if (writeEndOfLines)
                    {
                        WriteLine();
                    }
                }
            }
            return this;
        }
   }
}