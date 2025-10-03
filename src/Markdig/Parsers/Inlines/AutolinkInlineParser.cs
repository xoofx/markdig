// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines;

/// <summary>
/// An inline parser for parsing <see cref="AutolinkInline"/>.
/// </summary>
/// <seealso cref="InlineParser" />
public class AutolinkInlineParser : InlineParser
{
    public AutolinkInlineParser() : this(new AutolinkOptions())
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutolinkInlineParser"/> class.
    /// </summary>
    public AutolinkInlineParser(AutolinkOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        OpeningCharacters = ['<'];
    }

    public readonly AutolinkOptions Options;

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
            if (Options.OpenInNewWindow)
            {
                processor.Inline.GetAttributes().AddPropertyIfNotExist("target", "_blank");
            }
        }
        else if (Options.EnableHtmlParsing)
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
            if (Options.OpenInNewWindow)
            {
                processor.Inline.GetAttributes().AddPropertyIfNotExist("target", "_blank");
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}