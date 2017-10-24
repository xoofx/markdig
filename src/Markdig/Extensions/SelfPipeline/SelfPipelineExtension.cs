// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Renderers;

namespace Markdig.Extensions.SelfPipeline
{
    /// <summary>
    /// Extension to enable SelfPipeline, to configure a Markdown parsing/convertion to HTML automatically 
    /// from an embedded special tag in the input text <code>&lt;!--markdig:extensions--&gt;</code> where extensions is a string
    /// that specifies the extensions to use for the pipeline as exposed by <see cref="MarkdownExtensions.Configure"/> extension method
    /// on the <see cref="MarkdownPipelineBuilder"/>. This extension will invalidate all other extensions and will override them.
    /// </summary>
    public sealed class SelfPipelineExtension : IMarkdownExtension
    {
        public const string DefaultTag = "markdig";

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfPipelineExtension"/> class.
        /// </summary>
        /// <param name="tag">The matching start tag.</param>
        /// <param name="defaultExtensions">The default extensions.</param>
        /// <exception cref="System.ArgumentException">Tag cannot contain `<`  or `>` characters</exception>
        public SelfPipelineExtension(string tag = null, string defaultExtensions = null)
        {
            tag = tag?.Trim();
            tag = string.IsNullOrEmpty(tag) ? DefaultTag : tag;
            if (tag.IndexOfAny(new []{'<', '>'}) >= 0)
            {
                throw new ArgumentException("Tag cannot contain `<`  or `>` characters", nameof(tag));
            }

            if (defaultExtensions != null)
            {
                // Check that this default pipeline is supported
                // Will throw an ArgumentInvalidException if not
                new MarkdownPipelineBuilder().Configure(defaultExtensions);
            }
            DefaultExtensions = defaultExtensions;
            SelfPipelineHintTagStart = "<!--" + tag + ":";
        }

        /// <summary>
        /// Gets the default pipeline to configure if no tag was found in the input text. Default is <c>null</c> (core pipeline).
        /// </summary>
        public string DefaultExtensions { get; }

        /// <summary>
        /// Gets the self pipeline hint tag start that will be matched.
        /// </summary>
        public string SelfPipelineHintTagStart { get; }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Make sure that this pipeline has only one extension (itself)
            if (pipeline.Extensions.Count > 1)
            {
                throw new InvalidOperationException(
                    "The SelfPipeline extension cannot be configured with other extensions");
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        /// <summary>
        /// Creates a pipeline automatically configured from an input markdown based on the presence of the configuration tag.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <returns>The pipeline configured from the input</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MarkdownPipeline CreatePipelineFromInput(string inputText)
        {
            if (inputText == null) throw new ArgumentNullException(nameof(inputText));

            var builder = new MarkdownPipelineBuilder();
            string defaultConfig = DefaultExtensions;
            var indexOfSelfPipeline = inputText.IndexOf(SelfPipelineHintTagStart, StringComparison.OrdinalIgnoreCase);
            if (indexOfSelfPipeline >= 0)
            {
                var optionStart = indexOfSelfPipeline + SelfPipelineHintTagStart.Length;
                var endOfTag = inputText.IndexOf("-->", optionStart, StringComparison.OrdinalIgnoreCase);
                if (endOfTag >= 0)
                {
                    defaultConfig = inputText.Substring(optionStart, endOfTag - optionStart).Trim();
                }
            }

            if (!string.IsNullOrEmpty(defaultConfig))
            {
                builder.Configure(defaultConfig);
            }
            return builder.Build();
        }
    }
}