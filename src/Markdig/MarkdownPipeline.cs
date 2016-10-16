// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.ObjectModel;
using System.IO;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;

namespace Markdig
{
    /// <summary>
    /// This class is the Markdown pipeline build from a <see cref="MarkdownPipelineBuilder"/>.
    /// </summary>
    public class MarkdownPipeline
    {
        // This class is immutable

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownPipeline" /> class.
        /// </summary>
        internal MarkdownPipeline(OrderedList<IMarkdownExtension> extensions, BlockParserList blockParsers, InlineParserList inlineParsers, StringBuilderCache cache, TextWriter debugLog, ProcessDocumentDelegate documentProcessed)
        {
            if (blockParsers == null) throw new ArgumentNullException(nameof(blockParsers));
            if (inlineParsers == null) throw new ArgumentNullException(nameof(inlineParsers));
            // Add all default parsers
            Extensions = extensions;
            BlockParsers = blockParsers;
            InlineParsers = inlineParsers;
            StringBuilderCache = cache;
            DebugLog = debugLog;
            DocumentProcessed = documentProcessed;
        }

        internal bool PreciseSourceLocation { get; set; }

        /// <summary>
        /// The read-only list of extensions used to build this pipeline.
        /// </summary>
        public OrderedList<IMarkdownExtension> Extensions { get; }

        internal BlockParserList BlockParsers { get; }

        internal InlineParserList InlineParsers { get; }

        internal StringBuilderCache StringBuilderCache { get; }

        // TODO: Move the log to a better place
        internal TextWriter DebugLog { get; }

        internal ProcessDocumentDelegate DocumentProcessed;

        /// <summary>
        /// Allows to setup a <see cref="IMarkdownRenderer"/>.
        /// </summary>
        /// <param name="renderer">The markdown renderer to setup</param>
        public void Setup(IMarkdownRenderer renderer)
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            foreach (var extension in Extensions)
            {
                extension.Setup(this, renderer);
            }
        }
    }
}