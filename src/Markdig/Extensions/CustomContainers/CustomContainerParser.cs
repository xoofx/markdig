// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Extensions.CustomContainers
{
    /// <summary>
    /// The block parser for a <see cref="CustomContainer"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.FencedBlockParserBase{Markdig.Extensions.CustomContainers.CustomContainer}" />
    public class CustomContainerParser : FencedBlockParserBase<CustomContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomContainerParser"/> class.
        /// </summary>
        public CustomContainerParser()
        {
            OpeningCharacters = new [] {':'};

            // We don't need a prefix
            InfoPrefix = null;
        }

        protected override CustomContainer CreateFencedBlock(BlockProcessor processor)
        {
            return new CustomContainer(this);
        }
    }
}