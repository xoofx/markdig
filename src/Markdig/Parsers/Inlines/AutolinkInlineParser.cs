// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines;

/// <summary>
/// An inline parser for parsing <see cref="AutolinkInline"/>.
/// </summary>
/// <seealso cref="InlineParser" />
public class AutolinkInlineParser : InlineParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutolinkInlineParser"/> class.
    /// </summary>
    public AutolinkInlineParser()
    {
        OpeningCharacters = ['<'];
        EnableHtmlParsing = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable HTML parsing. Default is <c>true</c>
    /// </summary>
    public bool EnableHtmlParsing { get; set; }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var saved = slice;
        int line;
        int column;
        if (LinkHelper.TryParseAutolink(ref slice, out string? link, out bool isEmail))
        {
            processor.Inline = new AutolinkInline(link)
            {
                IsEmail = isEmail,
                Span = new SourceSpan(processor.GetSourcePosition(saved.Start, out line, out column), processor.GetSourcePosition(slice.Start - 1)),
                Line = line,
                Column = column
            };
        }
        else if (EnableHtmlParsing)
        {
            slice = saved;
            if (!HtmlHelper.TryParseHtmlTag(ref slice, out string? htmlTag))
            {
                return false;
            }

            processor.Inline = new HtmlInline(htmlTag)
            {
                Span = new SourceSpan(processor.GetSourcePosition(saved.Start, out line, out column), processor.GetSourcePosition(slice.Start - 1)),
                Line = line,
                Column = column
            };
        }
        else
        {
            return false;
        }

        return true;
    }
}