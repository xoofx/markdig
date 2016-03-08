// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Abbreviations
{
    /// <summary>
    /// A block parser for abbreviations.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.BlockParser" />
    public class AbbreviationParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbbreviationParser"/> class.
        /// </summary>
        public AbbreviationParser()
        {
            OpeningCharacters = new[] {'*'};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // A link must be of the form *[Some Text]: An abbreviation 
            var slice = processor.Line;
            var c = slice.NextChar();
            if (c != '[')
            {
                return BlockState.None;
            }

            string label;
            if (!LinkHelper.TryParseLabel(ref slice, out label))
            {
                return BlockState.None;
            }

            c = slice.CurrentChar;
            if (c != ':')
            {
                return BlockState.None;
            }
            slice.NextChar();

            slice.Trim();

            var abbr = new Abbreviation(this)
            {
                Label = label,
                Text = slice, Line = processor.LineIndex, Column = processor.Column
            };
            if (!processor.Document.HasAbbreviations())
            {
                processor.Document.ProcessInlinesBegin += DocumentOnProcessInlinesBegin;
            }
            processor.Document.AddAbbreviation(abbr.Label, abbr);

            return BlockState.BreakDiscard;
        }

        private void DocumentOnProcessInlinesBegin(InlineProcessor inlineProcessor)
        {
            inlineProcessor.Document.ProcessInlinesBegin -= DocumentOnProcessInlinesBegin;

            var abbreviations = inlineProcessor.Document.GetAbbreviations();
            // Should not happen, but another extension could decide to remove them, so...
            if (abbreviations == null)
            {
                return;
            }

            // Build a text matcher from the abbreviations labels
            var labels = new HashSet<string>(abbreviations.Keys);
            var matcher = new TextMatchHelper(labels);

            inlineProcessor.LiteralInlineParser.PostMatch += (InlineProcessor processor, ref StringSlice slice) =>
            {
                var literal = (LiteralInline) processor.Inline;

                ContainerInline container = null;

                // This is slow, but we don't have much the choice
                var content = literal.Content;
                var text = content.Text;
                for (int i = content.Start; i < content.End; i++)
                {
                    string match;
                    if (matcher.TryMatch(text, i, content.End - i + 1, out match))
                    {
                        // The word matched must be embraced by punctuation or whitespace or \0.
                        var c = content.PeekCharExtra(i - 1);
                        if (!(c == '\0' || c.IsAsciiPunctuation() || c.IsWhitespace()))
                        {
                            continue;
                        }
                        var indexAfterMatch = i + match.Length;
                        c = content.PeekCharExtra(indexAfterMatch);
                        if (!(c == '\0' || c.IsAsciiPunctuation() || c.IsWhitespace()))
                        {
                            continue;
                        }

                        // We should have a match, but in case...
                        Abbreviation abbr;
                        if (!abbreviations.TryGetValue(match, out abbr))
                        {
                            continue;
                        }

                        // If we don't have a container, create a new one
                        if (container == null)
                        {
                            container = new ContainerInline();
                        }

                        var abbrInline = new AbbreviationInline(abbr);

                        // Append the previous literal
                        if (i > content.Start)
                        {
                            container.AppendChild(literal);
                            // Truncate it before the abbreviation
                            literal.Content.End = i - 1;
                        }

                        // Appned the abbreviation
                        container.AppendChild(abbrInline);

                        // If this is the end of the string, clear the literal 
                        // and exit
                        if (content.End == indexAfterMatch - 1)
                        {
                            literal = null;
                            break;
                        }

                        // Process the remaining literal
                        literal = new LiteralInline();
                        content.Start = indexAfterMatch;
                        literal.Content = content;

                        i = indexAfterMatch - 1;
                    }
                }

                if (container != null)
                {
                    processor.Inline = container;
                    // If we have a pending literal, we can add it
                    if (literal != null)
                    {
                        container.AppendChild(literal);
                    }
                }
            };
        }
    }
}