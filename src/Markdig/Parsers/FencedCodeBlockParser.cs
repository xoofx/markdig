// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// Parser for a <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class FencedCodeBlockParser : FencedBlockParserBase<FencedCodeBlock>
    {
        public const string DefaultInfoPrefix = "language-";

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public FencedCodeBlockParser()
        {
            OpeningCharacters = new[] {'`', '~'};
            InfoPrefix = DefaultInfoPrefix;
        }

        protected override FencedCodeBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new FencedCodeBlock(this) {IndentCount = processor.Indent};
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var result = base.TryContinue(processor, block);
            if (result == BlockState.Continue)
            {
                var fence = (FencedCodeBlock)block;
                // Remove any indent spaces
                var c = processor.CurrentChar;
                var indentCount = fence.IndentCount;
                while (indentCount > 0 && c.IsSpace())
                {
                    indentCount--;
                    c = processor.NextChar();
                }
            }

            return result;
        }
    }
}