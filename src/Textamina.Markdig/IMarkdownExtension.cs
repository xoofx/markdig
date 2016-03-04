// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig
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
        void Setup(MarkdownPipeline pipeline);
    }
}