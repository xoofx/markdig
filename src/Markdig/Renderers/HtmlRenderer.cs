// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Buffers;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;

namespace Markdig.Renderers;

/// <summary>
/// Default HTML renderer for a Markdown <see cref="MarkdownDocument"/> object.
/// </summary>
/// <seealso cref="TextRendererBase{HtmlRenderer}" />
public class HtmlRenderer : TextRendererBase<HtmlRenderer>
{
    private static readonly IdnMapping s_idnMapping = new();

    private static readonly SearchValues<char> s_asciiNonEscapeChars =
        SearchValues.Create("!#$%()*+,-./0123456789:;=?@ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz");

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

        EnableHtmlForBlock = true;
        EnableHtmlForInline = true;
        EnableHtmlEscape = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to output HTML tags when rendering. See remarks.
    /// </summary>
    /// <remarks>
    /// This is used by some renderers to disable HTML tags when rendering some inline elements (for image links).
    /// </remarks>
    public bool EnableHtmlForInline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to output HTML tags when rendering. See remarks.
    /// </summary>
    /// <remarks>
    /// This is used by some renderers to disable HTML tags when rendering some block elements (for image links).
    /// </remarks>
    public bool EnableHtmlForBlock { get; set; }

    public bool EnableHtmlEscape { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use implicit paragraph (optional &lt;p&gt;)
    /// </summary>
    public bool ImplicitParagraph { get; set; }

    public bool UseNonAsciiNoEscape { get; set; }

    /// <summary>
    /// Gets a value to use as the base url for all relative links
    /// </summary>
    public Uri? BaseUrl { get; set; }

    /// <summary>
    /// Allows links to be rewritten
    /// </summary>
    public Func<string, string>? LinkRewriter { get; set; }

    /// <summary>
    /// Writes the content escaped for HTML.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HtmlRenderer WriteEscape(string? content)
    {
        WriteEscape(content.AsSpan());
        return this;
    }

    /// <summary>
    /// Writes the content escaped for HTML.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HtmlRenderer WriteEscape(ref StringSlice slice, bool softEscape = false)
    {
        WriteEscape(slice.AsSpan(), softEscape);
        return this;
    }

    /// <summary>
    /// Writes the content escaped for HTML.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HtmlRenderer WriteEscape(StringSlice slice, bool softEscape = false)
    {
        WriteEscape(slice.AsSpan(), softEscape);
        return this;
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
        WriteEscape(content.AsSpan(offset, length), softEscape);
        return this;
    }

    private static readonly SearchValues<char> s_escapedChars = SearchValues.Create("<>&\"");

    /// <summary>
    /// Writes the content escaped for HTML.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    public void WriteEscape(ReadOnlySpan<char> content, bool softEscape = false)
    {
        if (!content.IsEmpty)
        {
            WriteIndent();

            while (true)
            {
                int indexOfCharToEscape = softEscape
                    ? content.IndexOfAny('<', '&')
                    : content.IndexOfAny(s_escapedChars);

                if ((uint)indexOfCharToEscape >= (uint)content.Length)
                {
                    WriteRaw(content);
                    return;
                }

                WriteRaw(content.Slice(0, indexOfCharToEscape));

                if (EnableHtmlEscape)
                {
                    WriteRaw(content[indexOfCharToEscape] switch
                    {
                        '<' => "&lt;",
                        '>' => "&gt;",
                        '&' => "&amp;",
                        _ => "&quot;",
                    });
                }

                content = content.Slice(indexOfCharToEscape + 1);
            }
        }
    }

    /// <summary>
    /// Writes the URL escaped for HTML.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <returns>This instance</returns>
    public HtmlRenderer WriteEscapeUrl(string? content)
    {
        if (content is null)
            return this;

        if (BaseUrl != null
            // According to https://github.com/dotnet/runtime/issues/22718
            // this is the proper cross-platform way to check whether a uri is absolute or not:
            && Uri.TryCreate(content, UriKind.RelativeOrAbsolute, out var contentUri) && !contentUri.IsAbsoluteUri)
        {
            content = new Uri(BaseUrl, contentUri).AbsoluteUri;
        }

        if (LinkRewriter != null)
        {
            content = LinkRewriter(content);
        }

        if (!Ascii.IsValid(content))
        {
            int schemeOffset = content.IndexOf("://", StringComparison.Ordinal);
            if (schemeOffset > 0) // This is an absolute URL
            {
                schemeOffset += 3; // skip ://

                int domainLength = content.AsSpan(schemeOffset).IndexOfAny("/?#:");
                if (domainLength < 0)
                {
                    domainLength = content.Length - schemeOffset;
                }

                string? domainName = null;

                try
                {
                    domainName = s_idnMapping.GetAscii(content, schemeOffset, domainLength);
                }
                catch { }

                if (domainName is not null)
                {
                    WriteEscapeUrlCore(content.AsSpan(0, schemeOffset));
                    WriteEscapeUrlCore(domainName.AsSpan());
                    WriteEscapeUrlCore(content.AsSpan(schemeOffset + domainLength));
                    return this;
                }

                // Not a valid IDN, fallback to non-punycode encoding
            }
        }

        WriteEscapeUrlCore(content.AsSpan());
        return this;
    }

    private void WriteEscapeUrlCore(ReadOnlySpan<char> content)
    {
        WriteIndent();

        while (true)
        {
            int i = content.IndexOfAnyExcept(s_asciiNonEscapeChars);

            if ((uint)i >= (uint)content.Length)
            {
                WriteRaw(content);
                break;
            }

            WriteRaw(content.Slice(0, i));

            char c = content[i];

            if (c < 128)
            {
                WriteRaw(HtmlHelper.EscapeUrlCharacter(c));
            }
            else if (UseNonAsciiNoEscape)
            {
                // Special case for Edge/IE workaround for MarkdownEditor, don't escape non-ASCII chars to make image links working
                WriteRaw(c);
            }
            else
            {
                i = WriteEscapedUtf8Bytes(this, content, c, i);
            }

            content = content.Slice(i + 1);
        }

        static int WriteEscapedUtf8Bytes(HtmlRenderer renderer, ReadOnlySpan<char> content, char c, int i)
        {
            scoped ReadOnlySpan<char> chars;

            if (CharHelper.IsHighSurrogate(c) && (uint)(i + 1) < (uint)content.Length)
            {
                chars = stackalloc char[] { c, content[i + 1] };
                i++;
            }
            else
            {
                chars = stackalloc char[] { c };
            }

            Span<byte> utf8Buffer = stackalloc byte[4];
            int utf8Length = Encoding.UTF8.GetBytes(chars, utf8Buffer);
            utf8Buffer = utf8Buffer.Slice(0, utf8Length);

            Span<char> escapedBuffer = stackalloc char[3];
            escapedBuffer[0] = '%';

            foreach (byte b in utf8Buffer)
            {
                HexConverter.ToCharsBuffer(b, escapedBuffer, startingIndex: 1);
                renderer.WriteRaw(escapedBuffer);
            }

            return i;
        }
    }

    /// <summary>
    /// Writes the attached <see cref="HtmlAttributes"/> on the specified <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="markdownObject">The object.</param>
    /// <returns></returns>
    public HtmlRenderer WriteAttributes(MarkdownObject markdownObject)
    {
        if (markdownObject is null) ThrowHelper.ArgumentNullException_markdownObject();
        return WriteAttributes(markdownObject.TryGetAttributes());
    }

    /// <summary>
    /// Writes the specified <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="attributes">The attributes to render.</param>
    /// <param name="classFilter">A class filter used to transform a class into another class at writing time</param>
    /// <returns>This instance</returns>
    public HtmlRenderer WriteAttributes(HtmlAttributes? attributes, Func<string, string>? classFilter = null)
    {
        if (attributes is null)
        {
            return this;
        }

        if (attributes.Id != null)
        {
            Write(" id=\"");
            WriteEscape(attributes.Id);
            WriteRaw('"');
        }

        if (attributes.Classes is { Count: > 0 })
        {
            Write(" class=\"");
            for (int i = 0; i < attributes.Classes.Count; i++)
            {
                var cssClass = attributes.Classes[i];
                if (i > 0)
                {
                    WriteRaw(' ');
                }
                WriteEscape(classFilter != null ? classFilter(cssClass) : cssClass);
            }
            WriteRaw('"');
        }

        if (attributes.Properties is { Count: > 0 })
        {
            foreach (var property in attributes.Properties)
            {
                Write(' ');
                WriteRaw(property.Key);
                WriteRaw("=\"");
                WriteEscape(property.Value ?? "");
                WriteRaw('"');
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
        if (leafBlock is null) ThrowHelper.ArgumentNullException_leafBlock();

        var slices = leafBlock.Lines.Lines;
        if (slices is not null)
        {
            for (int i = 0; i < slices.Length; i++)
            {
                ref StringSlice slice = ref slices[i].Slice;
                if (slice.Text is null)
                {
                    break;
                }

                if (!writeEndOfLines && i > 0)
                {
                    WriteLine();
                }

                ReadOnlySpan<char> span = slice.AsSpan();
                if (escape)
                {
                    WriteEscape(span, softEscape);
                }
                else
                {
                    Write(span);
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