// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.CustomContainers
{
    public class CustomContainerParser : FencedBlockParserBase<CustomContainer>
    {
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