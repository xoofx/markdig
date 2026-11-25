// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using System.Buffers;
using System.Diagnostics;

namespace Markdig.Extensions.AutoLinks;

/// <summary>
/// The inline parser used to for autolinks.
/// </summary>
/// <seealso cref="InlineParser" />
public class AutoLinkParser : InlineParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoLinkParser"/> class.
    /// </summary>
    public AutoLinkParser(AutoLinkOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        OpeningCharacters =
        [
            'h', // for http:// and https://
            'f', // for ftp://
            'm', // for mailto:
            't', // for tel:
            'w', // for www.
        ];

        _validPreviousCharacters = SearchValues.Create(options.ValidPreviousCharacters);
    }

    /// <summary>
    /// Gets or sets the options.
    /// </summary>
    public readonly AutoLinkOptions Options;

    private readonly SearchValues<char> _validPreviousCharacters;

    /// <summary>
    /// Attempts to match the parser at the current position.
    /// </summary>
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        return PrecheckMatch(slice) && MatchCore(processor, ref slice);
    }

    // The opening characters are common letters, so the literal parser asks for the possible matches where it would
    // otherwise stop at each of them. A derived parser can override Match, so only this exact type offers the search.
    internal override bool CanSearchPossibleMatch => GetType() == typeof(AutoLinkParser);

    internal override int IndexOfPossibleMatch(string text, int start, int end)
    {
        // A link starts with "www." or with a scheme and its ':'. Both characters are rare in prose, unlike the
        // opening characters.
        int position = start;

        while (position <= end)
        {
            int anchor = text.AsSpan(position, end - position + 1).IndexOfAny(':', 'w');
            if (anchor < 0)
            {
                break;
            }

            anchor += position;
            int candidate = anchor;

            if (text[anchor] == ':')
            {
                ReadOnlySpan<char> scheme = text.AsSpan(start, anchor - start);
                candidate = scheme.EndsWith("http", StringComparison.Ordinal) ? anchor - 4
                    : scheme.EndsWith("https", StringComparison.Ordinal) ? anchor - 5
                    : scheme.EndsWith("mailto", StringComparison.Ordinal) ? anchor - 6
                    : scheme.EndsWith("ftp", StringComparison.Ordinal) ? anchor - 3
                    : scheme.EndsWith("tel", StringComparison.Ordinal) ? anchor - 3
                    : -1;
            }
            else if (anchor == end || text[anchor + 1] != 'w')
            {
                candidate = -1;
            }

            if (candidate >= 0 && PrecheckMatch(new StringSlice(text, candidate, end)))
            {
                return candidate;
            }

            position = anchor + 1;
        }

        return -1;
    }

    private bool PrecheckMatch(in StringSlice slice)
    {
        ReadOnlySpan<char> span = slice.AsSpan();

        Debug.Assert(span[0] is 'h' or 'f' or 'm' or 't' or 'w');

        // Precheck URL before the previous char, as it rejects almost every word. These comparisons are almost never
        // true, which the CPU predicts better than a switch on the first letter.
        if (!span.StartsWith("https://", StringComparison.Ordinal) &&
            !span.StartsWith("http://", StringComparison.Ordinal) &&
            !span.StartsWith("www.", StringComparison.Ordinal) && // We won't match http:/www. or /www.xxx
            !span.StartsWith("ftp://", StringComparison.Ordinal) &&
            !span.StartsWith("mailto:", StringComparison.Ordinal) &&
            !span.StartsWith("tel:", StringComparison.Ordinal))
        {
            return false;
        }

        // Previous char must be a whitespace or a punctuation
        var previousChar = slice.PeekCharExtra(-1);
        return previousChar.IsWhiteSpaceOrZero() || _validPreviousCharacters.Contains(previousChar);
    }

    private bool MatchCore(InlineProcessor processor, ref StringSlice slice)
    {
        char c = slice.CurrentChar;
        var startPosition = slice.Start;

        // We don't bother disposing the builder as it'll realistically never grow beyond the initial stack size.
        var pendingEmphasis = new ValueStringBuilder(stackalloc char[32]);

        // Check that an autolink is possible in the current context
        if (!IsAutoLinkValidInCurrentContext(processor, ref pendingEmphasis))
        {
            return false;
        }

        // Parse URL
        if (!LinkHelper.TryParseUrl(ref slice, out string? link, out _, true))
        {
            return false;
        }

        // If we have any pending emphasis, remove any pending emphasis characters from the end of the link
        if (pendingEmphasis.Length > 0)
        {
            for (int i = link.Length - 1; i >= 0; i--)
            {
                if (pendingEmphasis.AsSpan().Contains(link[i]))
                {
                    slice.Start--;
                }
                else
                {
                    if (i < link.Length - 1)
                    {
                        link = link.Substring(0, i + 1);
                    }
                    break;
                }
            }
        }

        int domainOffset = 0;

        // Post-check URL
        switch (c)
        {
            case 'h':
                if (string.Equals(link, "http://", StringComparison.Ordinal) ||
                    string.Equals(link, "https://", StringComparison.Ordinal))
                {
                    return false;
                }
                domainOffset = link[4] == 's' ? 8 : 7; // https:// or http://
                break;

            case 'w':
                domainOffset = 4; // www.
                break;

            case 'f':
                if (string.Equals(link, "ftp://", StringComparison.Ordinal))
                {
                    return false;
                }
                domainOffset = 6; // ftp://
                break;

            case 't':
                if (string.Equals(link, "tel", StringComparison.Ordinal))
                {
                    return false;
                }
                break;

            case 'm':
                int atIndex = link.IndexOf('@');
                if (atIndex == -1 ||
                    atIndex == 7) // mailto:@ - no email part
                {
                    return false;
                }
                domainOffset = atIndex + 1;
                break;
        }

        // Do not need to check if a telephone number is a valid domain
        if (c != 't' && !LinkHelper.IsValidDomain(link, domainOffset, Options.AllowDomainWithoutPeriod))
        {
            return false;
        }

        var inline = new LinkInline()
        {
            Span =
            {
                Start = processor.GetSourcePosition(startPosition, out int line, out int column),
            },
            Line = line,
            Column = column,
            Url = c == 'w' ? ((Options.UseHttpsForWWWLinks ? "https://" : "http://") + link) : link,
            IsClosed = true,
            IsAutoLink = true,
        };

        int skipFromBeginning = c switch
        {
            'm' => 7, // For mailto: skip "mailto:" for content
            't' => 4, // Same but for tel:
            _ => 0
        };

        inline.Span.End = inline.Span.Start + link.Length - 1;
        inline.UrlSpan = inline.Span;
        inline.AppendChild(new LiteralInline()
        {
            Span = inline.Span,
            Line = line,
            Column = column,
            Content = new StringSlice(slice.Text, startPosition + skipFromBeginning, startPosition + link.Length - 1),
            IsClosed = true
        });
        processor.Inline = inline;

        if (Options.OpenInNewWindow)
        {
            inline.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }

        return true;
    }

    private static bool IsAutoLinkValidInCurrentContext(InlineProcessor processor, ref ValueStringBuilder pendingEmphasis)
    {
        // Case where there is a pending HtmlInline <a>
        var currentInline = processor.Inline;
        while (currentInline != null)
        {
            if (currentInline is HtmlInline htmlInline)
            {
                // If we have a </a> we don't expect nested <a>
                if (htmlInline.Tag.StartsWith("</a", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // If there is a pending <a>, we can't allow a link
                if (htmlInline.Tag.StartsWith("<a", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            // Check previous sibling and parents in the tree
            currentInline = currentInline.PreviousSibling ?? currentInline.Parent;
        }

        // Check that we don't have any pending brackets opened (where we could have a possible markdown link)
        // NOTE: This assume that [ and ] are used for links, otherwise autolink will not work properly
        currentInline = processor.Inline;
        int countBrackets = 0;
        while (currentInline != null)
        {
            if (currentInline is LinkDelimiterInline linkDelimiterInline && linkDelimiterInline.IsActive)
            {
                if (linkDelimiterInline.Type == DelimiterType.Open)
                {
                    countBrackets++;
                }
                else if (linkDelimiterInline.Type == DelimiterType.Close)
                {
                    countBrackets--;
                }
            }
            else
            {
                // Record all pending characters for emphasis
                if (currentInline is EmphasisDelimiterInline emphasisDelimiter)
                {
                    if (!pendingEmphasis.AsSpan().Contains(emphasisDelimiter.DelimiterChar))
                    {
                        pendingEmphasis.Append(emphasisDelimiter.DelimiterChar);
                    }
                }
            }
            currentInline = currentInline.Parent;
        }

        return countBrackets <= 0;
    }
}
