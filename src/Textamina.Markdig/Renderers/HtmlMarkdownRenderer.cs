using System;
using System.IO;
using System.Text;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Renderers.Html.Inlines;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers
{
    public class HtmlMarkdownRenderer : MarkdownRenderer
    {
        // TODO: Move this code to HTMLHelper
        private const string HexCharacters = "0123456789ABCDEF";
        private static readonly char[] EscapeHtmlCharacters = { '&', '<', '>', '"' };
        private static readonly bool[] UrlSafeCharacters =
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false,
            false, true, false, true, true, true, false, false, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true, true, false, true, false, true,
            true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, true,
            false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false
        };

        private readonly TextWriter textWriter;
        private bool previousWasLine;
        private char[] buffer;

        public HtmlMarkdownRenderer(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException(nameof(textWriter));
            this.textWriter = textWriter;

            buffer = new char[4096];

            // Default block renderers
            Renderers.Add(new CodeBlockRenderer());
            Renderers.Add(new ListRenderer());
            Renderers.Add(new HeadingRenderer());
            Renderers.Add(new HtmlBlockRenderer());
            Renderers.Add(new ParagraphRenderer());
            Renderers.Add(new QuoteBlockRenderer());
            Renderers.Add(new ThematicBreakRenderer());

            // Default inline renderers
            Renderers.Add(new AutolinkInlineRenderer());
            Renderers.Add(new CodeInlineRenderer());
            Renderers.Add(new DelimiterInlineRenderer());
            Renderers.Add(new EmphasisInlineRenderer());
            Renderers.Add(new HardlineBreakInlineRenderer());
            Renderers.Add(new SoftlineBreakInlineRenderer());
            Renderers.Add(new HtmlInlineRenderer());
            Renderers.Add(new LinkInlineRenderer());
            Renderers.Add(new LiteralInlineRenderer());

            EnableHtmlForInline = true;
        }

        public bool EnableHtmlForInline { get; set; }

        public bool ImplicitParagraph { get; set; }

        public HtmlMarkdownRenderer EnsureLine()
        {
            if (!previousWasLine)
            {
                WriteLine();
            }
            return this;
        }

        public HtmlMarkdownRenderer Write(string content)
        {
            previousWasLine = false;
            textWriter.Write(content);
            return this;
        }

        public HtmlMarkdownRenderer Write(char content)
        {
            previousWasLine = content == '\n';
            textWriter.Write(content);
            return this;
        }

        public HtmlMarkdownRenderer Write(string content, int offset, int length)
        {
            previousWasLine = false;
            if (length > buffer.Length)
            {
                buffer = content.ToCharArray(offset, length);
            }
            else
            {
                content.CopyTo(offset, buffer, 0, length);
            }

            textWriter.Write(buffer, 0, length);
            return this;
        }

        public HtmlMarkdownRenderer WriteLine()
        {
            textWriter.WriteLine();
            previousWasLine = true;
            return this;
        }

        public HtmlMarkdownRenderer WriteLine(string content)
        {
            previousWasLine = true;
            textWriter.WriteLine(content);
            return this;
        }

        public HtmlMarkdownRenderer WriteEscape(string content)
        {
            previousWasLine = false;
            if (string.IsNullOrEmpty(content))
                return this;

            int pos;
            int lastPos = 0;
            while ((pos = content.IndexOfAny(EscapeHtmlCharacters, lastPos, content.Length - lastPos)) != -1)
            {
                Write(content, lastPos, pos - lastPos);
                lastPos = pos + 1;

                switch (content[pos])
                {
                    case '<':
                        Write("&lt;");
                        break;
                    case '>':
                        Write("&gt;");
                        break;
                    case '&':
                        Write("&amp;");
                        break;
                    case '"':
                        Write("&quot;");
                        break;
                }
            }

            Write(content, lastPos, content.Length - lastPos);
            return this;
        }

        public HtmlMarkdownRenderer WriteEscapeUrl(string content)
        {
            if (content == null)
                return this;

            int previousPosition = 0;
            int len = content.Length;

            for (var i = 0; i < len; i++)
            {
                var c = content[i];

                if (c == '&')
                {
                    Write(content, previousPosition, i - previousPosition);
                    previousPosition = i + 1;
                    Write("&amp;");
                }
                else if (c < 128 && !UrlSafeCharacters[c])
                {
                    Write(content, previousPosition, i - previousPosition);
                    previousPosition = i + 1;

                    Write("%").Write(HexCharacters[c / 16]).Write(HexCharacters[c % 16]);
                }
                else if (c > 127)
                {
                    Write(content, previousPosition, i - previousPosition);
                    previousPosition = i + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && len != previousPosition)
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c, content[previousPosition] });
                        previousPosition = ++i + 1;
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c });
                    }

                    for (var j = 0; j < bytes.Length; j++)
                    {
                        Write('%').Write(HexCharacters[bytes[j] / 16]).Write(HexCharacters[bytes[j] % 16]);
                    }
                }
            }

            Write(content, previousPosition, len - previousPosition);
            return this;
        }

        public HtmlMarkdownRenderer WriteLeafInline(LeafBlock leafBlock)
        {
            var inline = (Inline)leafBlock.Inline;
            if (inline != null)
            {
                while (inline != null)
                {
                    Write(inline);
                    inline = inline.NextSibling;
                }
            }
            return this;
        }

        public HtmlMarkdownRenderer WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape)
        {
            if (leafBlock.Lines != null)
            {
                var lines = leafBlock.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (!writeEndOfLines && i > 0)
                    {
                        WriteLine();
                    }
                    var line = lines.Lines[i];
                    if (escape)
                    {
                        WriteEscape(line.ToString());
                    }
                    else
                    {
                        Write(line.ToString());
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