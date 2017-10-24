// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Renderers;

namespace Markdig.Extensions.ListExtras
{
    /// <summary>
    /// Extension for adding new type of list items (a., A., i., I.)
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class ListExtraExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var parser = pipeline.BlockParsers.Find<ListBlockParser>();
            if (parser != null)
            {
                parser.ItemParsers.AddIfNotAlready<ListExtraItemParser>();
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }
    }
}