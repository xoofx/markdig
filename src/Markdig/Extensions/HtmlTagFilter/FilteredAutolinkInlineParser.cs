// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;

namespace Markdig.Extensions.HtmlTagFilter;

/// <summary>
/// A filtered autolink inline parser that applies whitelist/blacklist filtering to inline HTML tags.
/// </summary>
public class FilteredAutolinkInlineParser : AutolinkInlineParser
{
    private readonly HtmlTagFilterOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredAutolinkInlineParser"/> class.
    /// </summary>
    /// <param name="options">The filter options to apply.</param>
    public FilteredAutolinkInlineParser(HtmlTagFilterOptions options) : base()
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredAutolinkInlineParser"/> class.
    /// </summary>
    /// <param name="autolinkOptions">The autolink options.</param>
    /// <param name="filterOptions">The filter options to apply.</param>
    public FilteredAutolinkInlineParser(AutolinkOptions autolinkOptions, HtmlTagFilterOptions filterOptions)
        : base(autolinkOptions)
    {
        _options = filterOptions ?? throw new ArgumentNullException(nameof(filterOptions));
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // If HTML parsing is disabled, use base behavior
        if (!Options.EnableHtmlParsing)
        {
            return base.Match(processor, ref slice);
        }

        // Check if this is an HTML tag and if it should be filtered
        if (slice.CurrentChar == '<' && ShouldFilterTag(slice))
        {
            return false; // Don't parse this as HTML
        }

        return base.Match(processor, ref slice);
    }

    private bool ShouldFilterTag(StringSlice slice)
    {
        // Save the current position
        var savedStart = slice.Start;

        try
        {
            // Skip the '<' character
            if (slice.CurrentChar != '<')
            {
                return false;
            }
            slice.SkipChar();

            var c = slice.CurrentChar;

            // Check if this looks like an autolink (not an HTML tag)
            // Autolinks don't start with /, !, or ?
            if (c is '/' or '!' or '?')
            {
                // This is an HTML tag, not an autolink
            }
            else if (c.IsAlpha())
            {
                // Could be either an HTML tag or an autolink
                // We need to check if it's a valid tag name
            }
            else
            {
                return false; // Not a tag
            }

            // Skip '/' for close tags
            bool isCloseTag = c == '/';
            if (isCloseTag)
            {
                c = slice.NextChar();
            }

            // Extract tag name
            if (!c.IsAlpha())
            {
                return false; // Not a valid tag
            }

            Span<char> tagName = stackalloc char[32];
            int tagLength = 0;

            while (c.IsAlphaNumeric() || c == '-')
            {
                if (tagLength >= tagName.Length)
                {
                    return false; // Tag name too long
                }
                tagName[tagLength++] = c;
                c = slice.NextChar();
            }

            if (tagLength == 0)
            {
                return false;
            }

            // Check if the next character suggests this is an HTML tag
            // HTML tags have whitespace, '>', '/', or attributes after the tag name
            // Autolinks have ':' or '@' 
            if (c == ':' || c == '@')
            {
                return false; // This is likely an autolink, not an HTML tag
            }

            // Check if tag is allowed
            var tagString = tagName.Slice(0, tagLength).ToString();
            return !_options.IsTagAllowed(tagString);
        }
        finally
        {
            // Restore the original position
            slice.Start = savedStart;
        }
    }
}
