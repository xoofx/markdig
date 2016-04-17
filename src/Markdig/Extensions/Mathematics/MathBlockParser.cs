// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Mathematics
{
    /// <summary>
    /// The block parser for a <see cref="MathBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.FencedBlockParserBase{Markdig.Extensions.Mathematics.MathBlock}" />
    public class MathBlockParser : FencedBlockParserBase<MathBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathBlockParser"/> class.
        /// </summary>
        public MathBlockParser()
        {
            OpeningCharacters = new [] {'$'};
            // We expect to match only a $$, no less, no more
            MinimumMatchCount = 2;
            MaximumMatchCount = 2;

            DefaultClass = "math";

            // We don't need a prefix
            InfoPrefix = null;
        }

        public string DefaultClass { get; set; }

        protected override MathBlock CreateFencedBlock(BlockProcessor processor)
        {
            var block = new MathBlock(this);
            if (DefaultClass != null)
            {
                block.GetAttributes().AddClass(DefaultClass);
            }
            return block;
        }
    }
}