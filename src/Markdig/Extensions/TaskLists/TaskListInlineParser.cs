// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
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
            ListClass = "contains-task-list";
            ListItemClass = "task-list-item";
        }

        /// <summary>
        /// Gets or sets the list class used for a task list.
        /// </summary>
        public string ListClass { get; set; }

        /// <summary>
        /// Gets or sets the list item class used for a task list.
        /// </summary>
        public string ListItemClass { get; set; }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // A tasklist is either
            // [ ]
            // or [x] or [X]

            var listItemBlock = processor.Block.Parent as ListItemBlock;

            if (listItemBlock == null)
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

            // Add proper class for task list
            if (!string.IsNullOrEmpty(ListItemClass))
            {
                listItemBlock.GetAttributes().AddClass(ListItemClass);
            }

            var listBlock = (ListBlock) listItemBlock.Parent;
            if (!string.IsNullOrEmpty(ListClass))
            {
                listBlock.GetAttributes().AddClass(ListClass);
            }

            return true;
        }
    }
}