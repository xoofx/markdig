// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// This block parsers for pipe tables is used to by-pass list items that could start by a single '-'
    /// and would disallow to detect a pipe tables at inline parsing time, so we are basically forcing a line
    /// that starts by a '-' and have at least a '|' (and have optional spaces) and is a continuation of a
    /// paragraph.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class PipeTableBlockParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTableBlockParser"/> class.
        /// </summary>
        public PipeTableBlockParser()
        {
            OpeningCharacters = new[] {'-'};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // Only if we have already a paragraph
            var paragraph = processor.CurrentBlock as ParagraphBlock;
            if (processor.IsCodeIndent || paragraph  == null)
            {
                return BlockState.None;
            }

            // We require at least a pipe (and we allow only : - | and space characters)
            var line = processor.Line;
            var countPipe = 0;
            while (true)
            {
                var c = line.NextChar();
                if (c == '\0')
                {
                    if (countPipe > 0)
                    {
                        // Mark the paragraph as open (important, otherwise we would have an infinite loop)
                        paragraph.AppendLine(ref processor.Line, processor.Column, processor.LineIndex, processor.Line.Start);
                        paragraph.IsOpen = true;
                        return BlockState.BreakDiscard;
                    }

                    return BlockState.None;
                }
                
                if (c.IsSpace() || c == '-' || c == '|' || c == ':')
                {
                    if (c == '|')
                    {
                        countPipe++;
                    }
                    continue;
                }

                return BlockState.None;
            }
        }
    }
}