// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteLinkReferenceDefinition : LinkReferenceDefinition
    {
        public Footnote Footnote { get; set; }
    }
}