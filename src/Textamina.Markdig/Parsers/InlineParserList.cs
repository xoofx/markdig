// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;

namespace Textamina.Markdig.Parsers
{
    public class InlineParserList : ParserList<InlineParser, InlineParserState>
    {
        public InlineParserList()
        {
        }

        public IDelimiterProcessor[] DelimiterProcessors { get; private set; }

        public override void Initialize(InlineParserState initState)
        {
            // Prepare the list of delimiter processors
            var delimiterProcessors = new List<IDelimiterProcessor>();
            foreach (var parser in this)
            {
                var delimProcessor = parser as IDelimiterProcessor;
                if (delimProcessor != null)
                {
                    delimiterProcessors.Add(delimProcessor);
                }
            }
            DelimiterProcessors = delimiterProcessors.ToArray();

            base.Initialize(initState);
        }
    }
}