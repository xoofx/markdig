// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Extensions.Yaml
{
    /// <summary>
    /// Block parser for a YAML frontmatter. 
    /// </summary>
    /// <seealso cref="Markdig.Parsers.FencedBlockParserBase{YamlFrontMatterBlock}" />
    public class YamlFrontMatterParser : FencedBlockParserBase<YamlFrontMatterBlock>
    {
        // We reuse a FencedCodeBlock parser to grab a frontmatter, only active if it happens on the first line of the document.

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public YamlFrontMatterParser()
        {
            OpeningCharacters = new[] { '-' };
            InfoPrefix = null;
            // We expect only 3 --- at the beginning of the file no more, no less
            MinimumMatchCount = 3;
            MaximumMatchCount = 3;
        }

        protected override YamlFrontMatterBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new YamlFrontMatterBlock(this);
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // Only accept a frontmatter at the beginning of the file
            if (processor.LineIndex != 0)
            {
                return BlockState.None;
            }

            return base.TryOpen(processor);
        }
    }
}