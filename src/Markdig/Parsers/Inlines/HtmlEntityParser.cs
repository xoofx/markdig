// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines;

/// <summary>
/// An inline parser for HTML entities.
/// </summary>
/// <seealso cref="InlineParser" />
public class HtmlEntityParser : InlineParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlEntityParser"/> class.
    /// </summary>
    public HtmlEntityParser()
    {
        OpeningCharacters = ['&'];
    }

    public static bool TryParse(ref StringSlice slice, [NotNullWhen(true)] out string? literal, out int match)
    {
        literal = null;
        match = HtmlHelper.ScanEntity(slice, out int entityValue, out int entityNameStart, out int entityNameLength);
        if (match == 0)
        {
            return false;
        }

        if (entityNameLength > 0)
        {
            literal = EntityHelper.DecodeEntity(slice.Text.AsSpan(entityNameStart, entityNameLength));
        }
        else if (entityValue >= 0)
        {
            literal = EntityHelper.DecodeEntity(entityValue);
        }
        return literal != null;
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        if (!TryParse(ref slice, out string? literal, out int match))
        {
            return false;
        }

        var startPosition = slice.Start;

        if (literal != null)
        {
            var matched = slice;
            matched.End = slice.Start + match - 1;
            processor.Inline = new HtmlEntityInline()
            {
                Original = matched,
                Transcoded = new StringSlice(literal),
                Span = new SourceSpan(processor.GetSourcePosition(startPosition, out int line, out int column), processor.GetSourcePosition(matched.End)),
                Line = line,
                Column = column
            };
            slice.Start = slice.Start + match;
            return true;
        }

        return false;
    }
}