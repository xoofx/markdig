using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Attributes
{
    public class AttributesParser : InlineParser
    {
        public AttributesParser()
        {
            OpeningCharacters = new[] { '{' };
        }

        public static bool TryParse(ref StringSlice slice, out AttributesInline attributes)
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
                    var startName = line.Start;
                    if (!(c.IsAlpha() || c == '_' || c == ':'))
                    {
                        break;
                    }
                    while (true)
                    {
                        c = line.NextChar();
                        if (!(c.IsAlphaNumeric() || c == '_' || c == ':' || c == '.' || c == '-'))
                        {
                            break;
                        }
                    }
                    var endName = line.Start - 1;

                    // Skip any whitespaces
                    line.TrimStart();

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
                    if (c == '\'' || c == '\"')
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

                    var name = slice.Text.Substring(startName, endName - startName + 1);
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
                attributes = new AttributesInline
                {
                    Id = id,
                    Classes = classes,
                    Properties = properties
                };

                // Assign back the current state of the line to 
                slice = line;
            }
            return isValid;
        }


        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            AttributesInline attributes;
            if (TryParse(ref slice, out attributes))
            {
                state.Inline = attributes;

                var attributesCollection = state.Block.GetData(typeof (AttributesInline)) as List<AttributesInline>;
                if (attributesCollection == null)
                {
                    // Add a callback
                    state.Block.ProcessInlinesEnd += BlockOnProcessInlinesEnd;
                    attributesCollection = new List<AttributesInline>(); // TODO: use caching
                    state.Block.SetData(typeof(AttributesInline), attributesCollection);
                }
                attributesCollection.Add(attributes);
                return true;
            }

            return false;
        }

        private static void BlockOnProcessInlinesEnd(InlineParserState state)
        {
            // Remove the callback
            state.Block.ProcessInlinesEnd -= BlockOnProcessInlinesEnd;
            
            // Gets the attributes that we have scanned in the whole block
            var attributesCollection = (List<AttributesInline>)state.Block.GetData(typeof(AttributesInline));

            // Remove the list
            state.Block.RemoveData(typeof (AttributesInline));

            foreach (var attributesInline in attributesCollection)
            {
                var objectToAttach = (MarkdownObject)(attributesInline.PreviousSibling ?? attributesInline.Parent);

                // If the previous sibling object is null or the object is a literal
                // we will attach the attributes to the block instead
                if (objectToAttach == state.Block.Inline || objectToAttach is LiteralInline)
                {
                    objectToAttach = state.Block;
                }

                // Add html attributes to the object
                var htmlAttributes = objectToAttach.GetAttributes();
                attributesInline.CopyTo(htmlAttributes);
                htmlAttributes.Id = attributesInline.Id;

                // Remove the inline as we don't want to keep it in the code
                attributesInline.Remove();
            }
            // TODO: release list to a cache
        }
    }
}