// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;

namespace Markdig.Parsers
{
    /// <summary>
    /// A list of <see cref="InlineParser"/>.
    /// </summary>
    /// <seealso cref="ParserList{InlineParser, InlineParserState}" />
    public class InlineParserList : ParserList<InlineParser, InlineProcessor>
    {
        public InlineParserList(IEnumerable<InlineParser> parsers) : base(parsers)
        {
            // Prepare the list of post inline processors
            var postInlineProcessors = new List<IPostInlineProcessor>();
            foreach (var parser in this)
            {
                if (parser is IPostInlineProcessor delimProcessor)
                {
                    postInlineProcessors.Add(delimProcessor);
                }
            }
            PostInlineProcessors = postInlineProcessors.ToArray();
        }

        /// <summary>
        /// Gets the registered post inline processors.
        /// </summary>
        public IPostInlineProcessor[] PostInlineProcessors { get; private set; }
    }
}