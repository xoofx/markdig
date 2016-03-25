// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Mathematics
{
    /// <summary>
    /// An inline parser for <see cref="MathInline"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.InlineParser" />
    /// <seealso cref="Textamina.Markdig.Parsers.IDelimiterProcessor" />
    public class MathInlineParser : EmphasisInlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathInlineParser"/> class.
        /// </summary>
        public MathInlineParser()
        {
            EmphasisDescriptors.Clear();
            EmphasisDescriptors.Add(new EmphasisDescriptor('$', 1, 1, false));
            CreateEmphasisInline = CreateMathInline;
            DefaultClass = "math";
        }

        public string DefaultClass { get; set; }

        private EmphasisInline CreateMathInline(char emphasisChar, bool isStrong)
        {
            var inline = new MathInline() {DelimiterChar = emphasisChar, IsDouble = isStrong};
            inline.GetAttributes().AddClass(DefaultClass);
            return inline;
        }
    }
}