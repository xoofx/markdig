// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Renderers;

namespace Markdig.Extensions.Globalization
{
    /// <summary>
    /// The heading block extension
    /// </summary>
    public class HeadingBlockExtension : IMarkdownExtension
    {
        private readonly int maxLeadingCount = 6;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadingBlockExtension"/>
        /// </summary>
        /// <param name="maxLeadingCount"></param>
        public HeadingBlockExtension(int maxLeadingCount)
        {
            this.maxLeadingCount = maxLeadingCount;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            foreach (var parser in pipeline.BlockParsers)
            {
                if (parser is HeadingBlockParser headingBlockParser)
                {
                    headingBlockParser.MaxLeadingCount = maxLeadingCount;
                    break;
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {

        }
    }
}
