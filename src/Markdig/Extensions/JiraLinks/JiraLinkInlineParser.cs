// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.JiraLinks;

/// <summary>
/// Finds and replaces JIRA links inline
/// </summary>
public class JiraLinkInlineParser : InlineParser
{
    private readonly JiraLinkOptions _options;
    private readonly string _baseUrl;

    public JiraLinkInlineParser(JiraLinkOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _baseUrl = _options.GetUrl();
        //look for uppercase chars at the start (for the project key)
        OpeningCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Allow preceding whitespace or `(`
        var pc = slice.PeekCharExtra(-1);
        if (!pc.IsWhiteSpaceOrZero() && pc != '(')
        {
            return false; 
        }

        var current = slice.CurrentChar;

        var startKey = slice.Start;
        var endKey = slice.Start;

        // the first character of the key can not be a digit.
        if (current.IsDigit())
        {
            return false;
        }

        // read as many uppercase characters or digits as required - project key
        while (current.IsAlphaUpper() || current.IsDigit())
        {
            endKey = slice.Start;
            current = slice.NextChar();
        }

        //require a '-' between key and issue number
        if (!current.Equals('-'))
        {
            return false;
        }

        current = slice.NextChar(); // skip -

        //read as many numbers as required - issue number
        if (!current.IsDigit())
        {
            return false;
        }

        var startIssue = slice.Start;
        var endIssue = slice.Start;

        while (current.IsDigit()) 
        {
            endIssue = slice.Start;
            current = slice.NextChar();
        }

        if (!current.IsWhiteSpaceOrZero() && current != ')') //can be followed only by a whitespace or `)`
        {
            return false;
        }

        var jiraLink = new JiraLink() //create the link at the relevant position
        {
            Span =
            {
                Start = processor.GetSourcePosition(slice.Start, out int line, out int column)
            },
            Line = line,
            Column = column,
            Issue = new StringSlice(slice.Text, startIssue, endIssue),
            ProjectKey = new StringSlice(slice.Text, startKey, endKey),
        };
        jiraLink.Span.End = jiraLink.Span.Start + (endIssue - startKey);

        // Builds the Url
        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        builder.Append(_baseUrl);
        builder.Append('/');
        builder.Append(jiraLink.ProjectKey.AsSpan());
        builder.Append('-');
        builder.Append(jiraLink.Issue.AsSpan());
        jiraLink.Url = builder.AsSpan().ToString();

        // Builds the Label
        builder.Length = 0;
        builder.Append(jiraLink.ProjectKey.AsSpan());
        builder.Append('-');
        builder.Append(jiraLink.Issue.AsSpan());
        jiraLink.AppendChild(new LiteralInline(builder.ToString()));

        if (_options.OpenInNewWindow)
        {
            jiraLink.GetAttributes().AddProperty("target", "_blank");
        }

        processor.Inline = jiraLink;

        return true;
    }
}
