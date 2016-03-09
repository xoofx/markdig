// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.CustomContainers
{
    public class CustomContainerParser : FencedCodeBlockParser
    {
        public CustomContainerParser()
        {
            OpeningCharacters = new [] {':'};
            // We don't prefix by LanguagePrefix
            LanguagePrefix = null;
        }

        protected override FencedCodeBlock CreateFencedCodeBlock()
        {
            return new CustomContainer(this);
        }
    }
}