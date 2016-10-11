// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.AutoLinks
{
    /// <summary>
    /// The inline parser used to for autolinks.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
    public class AutoLinkParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLinkParser"/> class.
        /// </summary>
        public AutoLinkParser()
        {
            OpeningCharacters = new char[]
            {
                'h', // for http:// and https://
                'f', // for ftp://
                'm', // for mailto:
                'w', // for www.
            };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            string match;

            // Previous char must be a whitespace or a punctuation
            var previousChar = slice.PeekCharExtra(-1);
            if (!previousChar.IsAsciiPunctuation() && !previousChar.IsWhiteSpaceOrZero())
            {
                return false;
            }

            var startPosition = slice.Start;

            var c = slice.CurrentChar;
            // Precheck URL
            switch (c)
            {
                case 'h':
                    if (!slice.MatchLowercase("ttp://", 1) && !slice.MatchLowercase("ttps://", 1))
                    {
                        return false;
                    }
                    break;
                case 'f':
                    if (!slice.MatchLowercase("tp://", 1))
                    {
                        return false;
                    }
                    break;
                case 'm':
                    if (!slice.MatchLowercase("ailto:", 1))
                    {
                        return false;
                    }
                    break;

                case 'w':
                    if (!slice.MatchLowercase("ww.", 1) || previousChar == '/') // We won't match http:/www. or /www.xxx
                    {
                        return false;
                    }
                    break;
            }

            // Parse URL
            string link;
            if (!LinkHelper.TryParseUrl(ref slice, out link))
            {
                return false;
            }

            // Post-check URL
            switch (c)
            {
                case 'h':
                    if (string.Equals(link, "http://", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(link, "https://", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                    break;
                case 'f':
                    if (string.Equals(link, "ftp://", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                    break;
                case 'm':
                    if (string.Equals(link, "mailto:", StringComparison.OrdinalIgnoreCase) || !link.Contains("@"))
                    {
                        return false;
                    }
                    break;

                case 'w':
                    // We require at least two .
                    if (link.Length <= "www.x.y".Length || link.IndexOf(".", 4, StringComparison.Ordinal) < 0)
                    {
                        return false;
                    }
                    break;
            }

            int line;
            int column;
            var inline = new LinkInline()
            {
                Span =
                {
                    Start = processor.GetSourcePosition(startPosition, out line, out column),
                },
                Line = line,
                Column = column,
                Url = c == 'w' ? "http://" + link : link,
                IsClosed = true,
            };
            inline.Span.End = inline.Span.Start + link.Length - 1;
            inline.UrlSpan = inline.Span;
            inline.AppendChild(new LiteralInline()
            {
                Span = inline.Span,
                Line = line,
                Column = column,
                Content = new StringSlice(slice.Text, startPosition, startPosition + link.Length - 1),
                IsClosed = true
            });
            processor.Inline = inline;

            return true;
        }
    }
}