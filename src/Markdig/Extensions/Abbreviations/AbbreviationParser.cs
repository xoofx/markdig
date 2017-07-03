// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Abbreviations
{
    /// <summary>
    /// A block parser for abbreviations.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class AbbreviationParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbbreviationParser"/> class.
        /// </summary>
        public AbbreviationParser()
        {
            OpeningCharacters = new[] { '*' };
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // A link must be of the form *[Some Text]: An abbreviation 
            var slice = processor.Line;
            var startPosition = slice.Start;
            var c = slice.NextChar();
            if (c != '[')
            {
                return BlockState.None;
            }

            SourceSpan labelSpan;
            string label;
            if (!LinkHelper.TryParseLabel(ref slice, out label, out labelSpan))
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
                Text = slice,
                Span = new SourceSpan(startPosition, slice.End),
                Line = processor.LineIndex,
                Column = processor.Column,
                LabelSpan = labelSpan,
            };
            if (!processor.Document.HasAbbreviations())
            {
                processor.Document.ProcessInlinesBegin += DocumentOnProcessInlinesBegin;
            }
            processor.Document.AddAbbreviation(abbr.Label, abbr);

            return BlockState.BreakDiscard;
        }

        private void DocumentOnProcessInlinesBegin(InlineProcessor inlineProcessor, Inline inline)
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
                var literal = (LiteralInline)processor.Inline;
                var originalLiteral = literal;

                ContainerInline container = null;

                // This is slow, but we don't have much the choice
                var content = literal.Content;
                var text = content.Text;
                for (int i = content.Start; i < content.End; i++)
                {
                    string match;
                    if (matcher.TryMatch(text, i, content.End - i + 1, out match) && IsValidAbbreviation(match, content, i))
                    {
                        var indexAfterMatch = i + match.Length;

                        // We should have a match, but in case...
                        Abbreviation abbr;
                        if (!abbreviations.TryGetValue(match, out abbr))
                        {
                            continue;
                        }

                        // If we don't have a container, create a new one
                        if (container == null)
                        {
                            container = literal.Parent ??
                                new ContainerInline
                                {
                                    Span = originalLiteral.Span,
                                    Line = originalLiteral.Line,
                                    Column = originalLiteral.Column,
                                };
                        }

                        int line;
                        int column;
                        var abbrInline = new AbbreviationInline(abbr)
                        {
                            Span =
                            {
                                Start = processor.GetSourcePosition(i, out line, out column),
                            },
                            Line = line,
                            Column = column
                        };
                        abbrInline.Span.End = abbrInline.Span.Start + match.Length - 1;

                        // Append the previous literal
                        if (i > content.Start)
                        {
                            if (literal.Parent == null)
                            {
                                container.AppendChild(literal);
                            }

                        }

                        literal.Span.End = abbrInline.Span.Start - 1;
                        // Truncate it before the abbreviation
                        literal.Content.End = i - 1;


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
                        literal = new LiteralInline()
                        {
                            Span = new SourceSpan(abbrInline.Span.End + 1, literal.Span.End),
                            Line = line,
                            Column = column + match.Length,
                        };
                        content.Start = indexAfterMatch;
                        literal.Content = content;

                        i = indexAfterMatch - 1;
                    }
                }

                if (container != null)
                {
                    if (literal != null)
                    {
                        container.AppendChild(literal);
                    }
                    processor.Inline = container;
                }
            };
        }

        private static bool IsValidAbbreviation(string match, StringSlice content, int matchIndex)
        {
            // The word matched must be embraced by punctuation or whitespace or \0.
            var index = matchIndex - 1;
            while (index >= content.Start)
            {
                var c = content.PeekCharAbsolute(index);
                if (!(c == '\0' || c.IsWhitespace() || c.IsAsciiPunctuation()))
                {
                    return false;
                }

                if (c.IsAlphaNumeric())
                {
                    return false;
                }

                if (!c.IsAsciiPunctuation() || c.IsWhitespace())
                {
                    break;
                }
                index--;
            }

            // This will check if the next char at the end of the StringSlice is whitespace, punctuation or \0.
            var contentNew = content;
            contentNew.End = content.End + 1;
            index = matchIndex + match.Length;
            while (index <= contentNew.End)
            {
                var c = contentNew.PeekCharAbsolute(index);
                if (!(c == '\0' || c.IsWhitespace() || c.IsAsciiPunctuation()))
                {
                    return false;
                }

                if (c.IsAlphaNumeric())
                {
                    return false;
                }

                if (c.IsWhitespace())
                {
                    break;
                }
                index++;
            }
            return true;
        }
    }
}