// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig
{
    /// <summary>
    /// Base interface for an extension.
    /// </summary>
    public interface IMarkdownExtension
    {
        /// <summary>
        /// Setups this extension for the specified pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        void Setup(MarkdownPipelineBuilder pipeline);

        /// <summary>
        /// Setups this extension for the specified renderer.
        /// </summary>
        /// <param name="pipeline">The pipeline used to parse the document.</param>
        /// <param name="renderer">The renderer.</param>
        void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer);
    }
}