// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.TaskLists
{
    /// <summary>
    /// The inline parser for SmartyPants.
    /// </summary>
    public class TaskListInlineParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskListInlineParser"/> class.
        /// </summary>
        public TaskListInlineParser()
        {
            OpeningCharacters = new[] {'['};
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // A tasklist is either
            // [ ]
            // or [x] or [X]

            if (!(processor.Block.Parent is ListItemBlock))
            {
                return false;
            }

            var startingPosition = slice.Start;
            var c = slice.NextChar();
            if (!c.IsSpace() && c != 'x' && c != 'X')
            {
                return false;
            }
            if (slice.NextChar() != ']')
            {
                return false;
            }
            // Skip last ]
            slice.NextChar();

            // Create the TaskList
            int line;
            int column;
            var taskItem = new TaskList()
            {
                Span = { Start = processor.GetSourcePosition(startingPosition, out line, out column)},
                Line = line,
                Column = column,
                Checked = !c.IsSpace()
            };
            taskItem.Span.End = taskItem.Span.Start + 2;
            processor.Inline = taskItem;
            return true;
        }
    }
}