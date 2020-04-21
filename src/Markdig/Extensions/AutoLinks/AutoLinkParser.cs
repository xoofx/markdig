// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.AutoLinks
{
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

            OpeningCharacters = new char[]
            {
                'h', // for http:// and https://
                'f', // for ftp://
                'm', // for mailto:
                'w', // for www.
            };

            _listOfCharCache = new ListOfCharCache();
        }

        public readonly AutoLinkOptions Options;

        private readonly ListOfCharCache _listOfCharCache;

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // Previous char must be a whitespace or a punctuation
            var previousChar = slice.PeekCharExtra(-1);
            if (!previousChar.IsWhiteSpaceOrZero() && Options.ValidPreviousCharacters.IndexOf(previousChar) == -1)
            {
                return false;
            }

            List<char> pendingEmphasis = _listOfCharCache.Get();
            try
            {
                // Check that an autolink is possible in the current context
                if (!IsAutoLinkValidInCurrentContext(processor, pendingEmphasis))
                {
                    return false;
                }

                var startPosition = slice.Start;
                int domainOffset = 0;

                var c = slice.CurrentChar;
                // Precheck URL
                switch (c)
                {
                    case 'h':
                        if (slice.MatchLowercase("ttp://", 1))
                        {
                            domainOffset = 7; // http://
                        }
                        else if (slice.MatchLowercase("ttps://", 1))
                        {
                            domainOffset = 8; // https://
                        }
                        else return false;
                        break;
                    case 'f':
                        if (!slice.MatchLowercase("tp://", 1))
                        {
                            return false;
                        }
                        domainOffset = 6; // ftp://
                        break;
                    case 'm':
                        if (!slice.MatchLowercase("ailto:", 1))
                        {
                            return false;
                        }
                        break;

                    case 'w':
                        if (!slice.MatchLowercase("ww.", 1)) // We won't match http:/www. or /www.xxx
                        {
                            return false;
                        }
                        domainOffset = 4; // www.
                        break;
                }

                // Parse URL
                if (!LinkHelper.TryParseUrl(ref slice, out string link, true))
                {
                    return false;
                }


                // If we have any pending emphasis, remove any pending emphasis characters from the end of the link
                if (pendingEmphasis.Count > 0)
                {
                    for (int i = link.Length - 1; i >= 0; i--)
                    {
                        if (pendingEmphasis.Contains(link[i]))
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
                        int atIndex = link.IndexOf('@');
                        if (atIndex == -1 ||
                            atIndex == 7) // mailto:@ - no email part
                        {
                            return false;
                        }
                        domainOffset = atIndex + 1;
                        break;
                }

                if (!LinkHelper.IsValidDomain(link, domainOffset))
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

                var skipFromBeginning = c == 'm' ? 7 : 0; // For mailto: skip "mailto:" for content

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
                    inline.GetAttributes().AddPropertyIfNotExist("target", "blank");
                }

                return true;
            }
            finally
            {
                _listOfCharCache.Release(pendingEmphasis);
            }
        }

        private bool IsAutoLinkValidInCurrentContext(InlineProcessor processor, List<char> pendingEmphasis)
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
                        if (!pendingEmphasis.Contains(emphasisDelimiter.DelimiterChar))
                        {
                            pendingEmphasis.Add(emphasisDelimiter.DelimiterChar);
                        }
                    }
                }
                currentInline = currentInline.Parent;
            }

            return countBrackets <= 0;
        }

        private sealed class ListOfCharCache : DefaultObjectCache<List<char>>
        {
            protected override void Reset(List<char> instance)
            {
                instance.Clear();
            }
        }
    }
}