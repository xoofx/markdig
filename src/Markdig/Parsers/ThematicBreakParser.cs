// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers;

/// <summary>
/// A block parser for a <see cref="ThematicBreakBlock"/>.
/// </summary>
/// <seealso cref="BlockParser" />
public class ThematicBreakParser : BlockParser
{
    /// <summary>
    /// A singleton instance used by other parsers.
    /// </summary>
    public static readonly ThematicBreakParser Default = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ThematicBreakParser"/> class.
    /// </summary>
    public ThematicBreakParser()
    {
        OpeningCharacters = ['-', '_', '*'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        var startPosition = processor.Start;
        var line = processor.Line;

        // 4.1 Thematic breaks 
        // A line consisting of 0-3 spaces of indentation, followed by a sequence of three or more matching -, _, or * characters, each followed optionally by any number of spaces
        int breakCharCount = 0;
        var breakChar = line.CurrentChar;
        bool hasSpacesSinceLastMatch = false;
        bool hasInnerSpaces = false;
        var c = breakChar;
        while (c != '\0')
        {
            if (c == breakChar)
            {
                if (hasSpacesSinceLastMatch)
                {
                    hasInnerSpaces = true;
                }

                breakCharCount++;
            }
            else if (c.IsSpaceOrTab())
            {
                hasSpacesSinceLastMatch = true;
            }
            else
            {
                return BlockState.None;
            }

            c = line.NextChar();
        }

        // If it as less than 3 chars or it is a setex heading and we are already in a paragraph, let the paragraph handle it
        var previousParagraph = processor.CurrentBlock as ParagraphBlock;

        var isSetexHeading = previousParagraph != null && breakChar == '-' && !hasInnerSpaces;
        if (isSetexHeading)
        {
            var parent = previousParagraph!.Parent!;
            if (previousParagraph.Column != processor.Column && (parent is QuoteBlock or ListItemBlock))
            {
                isSetexHeading = false;
            }
        }

        if (breakCharCount < 3 || isSetexHeading)
        {
            return BlockState.None;
        }

        // Push a new block
        var thematicBreak = new ThematicBreakBlock(this)
        {
            Column = processor.Column,
            Span = new SourceSpan(startPosition, line.End),
            ThematicChar = breakChar,
            ThematicCharCount = breakCharCount,
            // TODO: should we separate whitespace before/after?
            //BeforeWhitespace = beforeWhitespace,
            //AfterWhitespace = processor.PopBeforeWhitespace(processor.CurrentLineStartPosition),
            Content = new StringSlice(line.Text, processor.TriviaStart, line.End, line.NewLine), //include whitespace for now
        };

        if (processor.TrackTrivia)
        {
            thematicBreak.LinesBefore = processor.UseLinesBefore();
            thematicBreak.NewLine = processor.Line.NewLine;
        }

        processor.NewBlocks.Push(thematicBreak);
        return BlockState.BreakDiscard;
    }
}