// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.GenericAttributes
{
    /// <summary>
    /// An inline parser used to parse a HTML attributes that can be attached to the previous <see cref="Inline"/> or current <see cref="Block"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
    public class GenericAttributesParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericAttributesParser"/> class.
        /// </summary>
        public GenericAttributesParser()
        {
            OpeningCharacters = new[] { '{' };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            HtmlAttributes attributes;
            var startPosition = slice.Start;
            if (TryParse(ref slice, out attributes))
            {
                var inline = processor.Inline;

                // If the curent object to attach is either a literal or delimiter
                // try to find a suitable parent, otherwise attach the html attributes to the block
                if (inline is LiteralInline)
                {
                    while (true)
                    {
                        inline = inline.Parent;
                        if (!(inline is DelimiterInline))
                        {
                            break;
                        }
                    }
                }
                var objectToAttach = inline == null || inline == processor.Root ? (MarkdownObject) processor.Block : inline;

                // If the current block is a Paragraph, but only the HtmlAttributes is used,
                // Try to attach the attributes to the following block
                var paragraph = objectToAttach as ParagraphBlock;
                if (paragraph != null && paragraph.Inline.FirstChild == null && processor.Inline == null && slice.IsEmptyOrWhitespace())
                {
                    var parent = paragraph.Parent;
                    var indexOfParagraph = parent.IndexOf(paragraph);
                    if (indexOfParagraph + 1 < parent.Count)
                    {
                        objectToAttach = parent[indexOfParagraph + 1];
                        // We can remove the paragraph as it is empty
                        paragraph.RemoveAfterProcessInlines = true;
                    }
                }

                var currentHtmlAttributes = objectToAttach.GetAttributes();
                attributes.CopyTo(currentHtmlAttributes, true, false);

                // Update the position of the attributes
                int line;
                int column;
                currentHtmlAttributes.Span.Start = processor.GetSourcePosition(startPosition, out line, out column);
                currentHtmlAttributes.Line = line;
                currentHtmlAttributes.Column = column;
                currentHtmlAttributes.Span.End = currentHtmlAttributes.Span.Start + slice.Start - startPosition - 1;

                // We don't set the processor.Inline as we don't want to add attach attributes to a particular entity
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to extra from the current position of a slice an HTML attributes {...}
        /// </summary>
        /// <param name="slice">The slice to parse.</param>
        /// <param name="attributes">The output attributes or null if not found or invalid</param>
        /// <returns><c>true</c> if parsing the HTML attributes was succsesfull</returns>
        public static bool TryParse(ref StringSlice slice, out HtmlAttributes attributes)
        {
            attributes = null;
            if (slice.PeekCharExtra(-1) == '{')
            {
                return false;
            }

            var line = slice;

            string id = null;
            List<string> classes = null;
            List<KeyValuePair<string, string>> properties = null;

            bool isValid = false;
            var c = line.NextChar();
            while (true)
            {
                if (c == '}')
                {
                    isValid = true;
                    line.NextChar(); // skip }
                    break;
                }

                if (c == '\0')
                {
                    break;
                }

                bool isClass = c == '.';
                if (c == '#' || isClass)
                {
                    c = line.NextChar(); // Skip #
                    var start = line.Start;
                    // Get all non-whitespace characters following a #
                    // But stop if we found a } or \0
                    while (c != '}' && c != '\0' && !c.IsWhitespace())
                    {
                        c = line.NextChar();
                    }
                    var end = line.Start - 1;
                    if (end == start)
                    {
                        break;
                    }
                    var text = slice.Text.Substring(start, end - start + 1);
                    if (isClass)
                    {
                        if (classes == null)
                        {
                            classes = new List<string>();
                        }
                        classes.Add(text);
                    }
                    else
                    {
                        id = text;
                    }
                    continue;
                }

                if (!c.IsWhitespace())
                {
                    // Parse the attribute name
                    if (!IsStartAttributeName(c))
                    {
                        break;
                    }
                    var startName = line.Start;
                    while (true)
                    {
                        c = line.NextChar();
                        if (!(c.IsAlphaNumeric() || c == '_' || c == ':' || c == '.' || c == '-'))
                        {
                            break;
                        }
                    }
                    var name = slice.Text.Substring(startName, line.Start - startName);

                    var hasSpace = c.IsSpaceOrTab();

                    // Skip any whitespaces
                    line.TrimStart();
                    c = line.CurrentChar;

                    // Handle boolean properties that are not followed by = 
                    if ((hasSpace && (c == '.' || c == '#' || IsStartAttributeName(c))) || c == '}')
                    {
                        if (properties == null)
                        {
                            properties = new List<KeyValuePair<string, string>>();
                        }
                        // Add a null value for the property
                        properties.Add(new KeyValuePair<string, string>(name, null));
                        continue;
                    }
                    
                    // Else we expect a regular property
                    if (line.CurrentChar != '=')
                    {
                        break;
                    }

                    // Go to next char, skip any spaces
                    line.NextChar();
                    line.TrimStart();

                    int startValue = -1;
                    int endValue = -1;

                    c = line.CurrentChar;
                    // Parse a quoted string
                    if (c == '\'' || c == '"')
                    {
                        char openingStringChar = c;
                        startValue = line.Start + 1;
                        while (true)
                        {
                            c = line.NextChar();
                            if (c == '\0')
                            {
                                return false;
                            }
                            if (c == openingStringChar)
                            {
                                break;
                            }
                        }
                        endValue = line.Start - 1;
                        c = line.NextChar(); // Skip closing opening string char
                    }
                    else
                    {
                        // Parse until we match a space or a special html character
                        startValue = line.Start;
                        while (true)
                        {
                            if (c == '\0')
                            {
                                return false;
                            }
                            if (c.IsWhitespace() || c == '}')
                            {
                                break;
                            }
                            c = line.NextChar();
                        }
                        endValue = line.Start - 1;
                        if (endValue == startValue)
                        {
                            break;
                        }
                    }

                    var value = slice.Text.Substring(startValue, endValue - startValue + 1);

                    if (properties == null)
                    {
                        properties = new List<KeyValuePair<string, string>>();
                    }
                    properties.Add(new KeyValuePair<string, string>(name, value));
                    continue;
                }

                c = line.NextChar();
            }

            if (isValid)
            {
                attributes = new HtmlAttributes()
                {
                    Id = id,
                    Classes = classes,
                    Properties = properties
                };

                // Assign back the current processor of the line to 
                slice = line;
            }
            return isValid;
        }

        private static bool IsStartAttributeName(char c)
        {
            return c.IsAlpha() || c == '_' || c == ':';
        }
    }
}