// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers
{
    /// <summary>
    /// Base class for an ordered list item parser.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.ListItemParser" />
    public abstract class OrderedListItemParser : ListItemParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParser"/> class.
        /// </summary>
        protected OrderedListItemParser()
        {
            OrderedDelimiters = new[] { '.', ')' };
        }

        /// <summary>
        /// Gets or sets the ordered delimiters used after a digit/number (by default `.` and `)`)
        /// </summary>
        public char[] OrderedDelimiters { get; set; }

        /// <summary>
        /// Utility method that tries to parse the delimiter coming after an ordered list start (e.g: the `)` after `1)`).
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="orderedDelimiter">The ordered delimiter found if this method is successful.</param>
        /// <returns><c>true</c> if parsing was successful; <c>false</c> otherwise.</returns>
        protected bool TryParseDelimiter(BlockProcessor state, out char orderedDelimiter)
        {
            // Check if we have an ordered delimiter
            orderedDelimiter = state.CurrentChar;
            for (int i = 0; i < OrderedDelimiters.Length; i++)
            {
                if (OrderedDelimiters[i] == orderedDelimiter)
                {
                    state.NextChar();
                    return true;
                }
            }
            return false;
        }
    }
}