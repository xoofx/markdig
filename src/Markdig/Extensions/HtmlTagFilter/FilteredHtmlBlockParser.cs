// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Extensions.HtmlTagFilter;

/// <summary>
/// A filtered HTML block parser that applies whitelist/blacklist filtering to HTML tags.
/// </summary>
public class FilteredHtmlBlockParser : HtmlBlockParser
{
    private readonly HtmlTagFilterOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredHtmlBlockParser"/> class.
    /// </summary>
    /// <param name="options">The filter options to apply.</param>
    public FilteredHtmlBlockParser(HtmlTagFilterOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        // Check if we should filter this tag
        if (ShouldFilterTag(processor.Line))
        {
            return BlockState.None;
        }

        return base.TryOpen(processor);
    }

    private bool ShouldFilterTag(StringSlice line)
    {
        // Save the current position
        var savedStart = line.Start;
        
        try
        {
            // Skip the '<' character
            if (line.CurrentChar != '<')
            {
                return false;
            }
            line.SkipChar();

            var c = line.CurrentChar;
            
            // Only allow HTML comments (<!-- -->), filter everything else
            if (c == '!')
            {
                // Check if this is a comment
                if (line.PeekChar() == '-' && line.PeekChar(1) == '-')
                {
                    return false; // Allow HTML comments
                }
                // Block CDATA, declarations, etc.
                return true;
            }
            
            if (c == '?')
            {
                // Block processing instructions
                return true;
            }

            // Skip '/' for close tags
            bool isCloseTag = c == '/';
            if (isCloseTag)
            {
                c = line.NextChar();
            }

            // Extract tag name
            if (!c.IsAlpha())
            {
                return false;
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
                c = line.NextChar();
            }

            if (tagLength == 0)
            {
                return false;
            }

            // Check if tag is allowed
            var tagString = tagName.Slice(0, tagLength).ToString();
            return !_options.IsTagAllowed(tagString);
        }
        finally
        {
            // Restore the original position
            line.Start = savedStart;
        }
    }
}