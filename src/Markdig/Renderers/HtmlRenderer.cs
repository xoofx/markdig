// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;

namespace Markdig.Renderers
{
    /// <summary>
    /// Default HTML renderer for a Markdown <see cref="MarkdownDocument"/> object.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.TextRendererBase{Markdig.Renderers.HtmlRenderer}" />
    public class HtmlRenderer : TextRendererBase<HtmlRenderer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlRenderer"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
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
            ObjectRenderers.Add(new LineBreakInlineRenderer());
            ObjectRenderers.Add(new HtmlInlineRenderer());
            ObjectRenderers.Add(new HtmlEntityInlineRenderer());            
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new LiteralInlineRenderer());

            EnableHtmlForInline = true;
            EnableHtmlEscape = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ouput HTML tags when rendering. See remarks.
        /// </summary>
        /// <remarks>
        /// This is used by some renderers to disable HTML tags when rendering some inlines (for image links).
        /// </remarks>
        public bool EnableHtmlForInline { get; set; }

        public bool EnableHtmlEscape { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use implicit paragraph (optional &lt;p&gt;)
        /// </summary>
        public bool ImplicitParagraph { get; set; }

        public bool UseNonAsciiNoEscape { get; set; }

        /// <summary>
        /// Writes the content escaped for HTML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public HtmlRenderer WriteEscape(string content)
        {
            if (string.IsNullOrEmpty(content))
                return this;

            WriteEscape(content, 0, content.Length);
            return this;
        }

        /// <summary>
        /// Writes the content escaped for HTML.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public HtmlRenderer WriteEscape(ref StringSlice slice, bool softEscape = false)
        {
            if (slice.Start > slice.End)
            {
                return this;
            }
            return WriteEscape(slice.Text, slice.Start, slice.Length, softEscape);
        }

        /// <summary>
        /// Writes the content escaped for HTML.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public HtmlRenderer WriteEscape(StringSlice slice, bool softEscape = false)
        {
            return WriteEscape(ref slice, softEscape);
        }

        /// <summary>
        /// Writes the content escaped for HTML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        public HtmlRenderer WriteEscape(string content, int offset, int length, bool softEscape = false)
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
                        if (EnableHtmlEscape)
                        {
                            Write("&lt;");
                        }
                        previousOffset = offset + 1;
                        break;
                    case '>':
                        if (!softEscape)
                        {
                            Write(content, previousOffset, offset - previousOffset);
                            if (EnableHtmlEscape)
                            {
                                Write("&gt;");
                            }
                            previousOffset = offset + 1;
                        }
                        break;
                    case '&':
                        Write(content, previousOffset, offset - previousOffset);
                        if (EnableHtmlEscape)
                        {
                            Write("&amp;");
                        }
                        previousOffset = offset + 1;
                        break;
                    case '"':
                        if (!softEscape)
                        {
                            Write(content, previousOffset, offset - previousOffset);
                            if (EnableHtmlEscape)
                            {
                                Write("&quot;");
                            }
                            previousOffset = offset + 1;
                        }
                        break;
                }
            }

            Write(content, previousOffset, end - previousOffset);
            return this;
        }

        /// <summary>
        /// Writes the URL escaped for HTML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
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

                    // Special case for Edge/IE workaround for MarkdownEditor, don't escape non-ASCII chars to make image links working
                    if (UseNonAsciiNoEscape)
                    {
                        Write(c);
                    }
                    else
                    {
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
            }

            Write(content, previousPosition, length - previousPosition);
            return this;
        }

        /// <summary>
        /// Writes the attached <see cref="HtmlAttributes"/> on the specified <see cref="MarkdownObject"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public HtmlRenderer WriteAttributes(MarkdownObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return WriteAttributes(obj.TryGetAttributes());
        }

        /// <summary>
        /// Writes the specified <see cref="HtmlAttributes"/>.
        /// </summary>
        /// <param name="attributes">The attributes to render.</param>
        /// <param name="classFilter">A class filter used to transform a class into another class at writing time</param>
        /// <returns>This instance</returns>
        public HtmlRenderer WriteAttributes(HtmlAttributes attributes, Func<string, string> classFilter = null)
        {
            if (attributes == null)
            {
                return this;
            }

            if (attributes.Id != null)
            {
                Write(" id=\"").WriteEscape(attributes.Id).Write("\"");
            }

            if (attributes.Classes != null && attributes.Classes.Count > 0)
            {
                Write(" class=\"");
                for (int i = 0; i < attributes.Classes.Count; i++)
                {
                    var cssClass = attributes.Classes[i];
                    if (i > 0)
                    {
                        Write(" ");
                    }
                    WriteEscape(classFilter != null ? classFilter(cssClass) : cssClass);
                }
                Write("\"");
            }

            if (attributes.Properties != null && attributes.Properties.Count > 0)
            {
                foreach (var property in attributes.Properties)
                {
                    Write(" ").Write(property.Key);
                    if (property.Value != null)
                    {
                        Write("=").Write("\"");
                        WriteEscape(property.Value);
                        Write("\"");
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Writes the lines of a <see cref="LeafBlock"/>
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <param name="writeEndOfLines">if set to <c>true</c> write end of lines.</param>
        /// <param name="escape">if set to <c>true</c> escape the content for HTML</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        public HtmlRenderer WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape, bool softEscape = false)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
            if (leafBlock.Lines.Lines != null)
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
                        WriteEscape(ref slices[i].Slice, softEscape);
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