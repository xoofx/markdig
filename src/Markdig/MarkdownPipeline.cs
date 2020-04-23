// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
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
        internal MarkdownPipeline(OrderedList<IMarkdownExtension> extensions, BlockParserList blockParsers, InlineParserList inlineParsers, TextWriter debugLog, ProcessDocumentDelegate documentProcessed)
        {
            if (blockParsers == null) ThrowHelper.ArgumentNullException(nameof(blockParsers));
            if (inlineParsers == null) ThrowHelper.ArgumentNullException(nameof(inlineParsers));
            // Add all default parsers
            Extensions = extensions;
            BlockParsers = blockParsers;
            InlineParsers = inlineParsers;
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

        // TODO: Move the log to a better place
        internal TextWriter DebugLog { get; }

        internal ProcessDocumentDelegate DocumentProcessed;

        /// <summary>
        /// Allows to setup a <see cref="IMarkdownRenderer"/>.
        /// </summary>
        /// <param name="renderer">The markdown renderer to setup</param>
        public void Setup(IMarkdownRenderer renderer)
        {
            if (renderer == null) ThrowHelper.ArgumentNullException(nameof(renderer));
            foreach (var extension in Extensions)
            {
                extension.Setup(this, renderer);
            }
        }


        private HtmlRendererCache _rendererCache = null;

        internal HtmlRenderer GetCacheableHtmlRenderer()
        {
            if (_rendererCache is null)
            {
                _rendererCache = new HtmlRendererCache
                {
                    OnNewInstanceCreated = Setup
                };
            }
            return _rendererCache.Get();
        }
        internal void ReleaseCacheableHtmlRenderer(HtmlRenderer renderer)
        {
            _rendererCache.Release(renderer);
        }

        private sealed class HtmlRendererCache : ObjectCache<HtmlRenderer>
        {
            public Action<HtmlRenderer> OnNewInstanceCreated;

            protected override HtmlRenderer NewInstance()
            {
                var writer = new StringWriter();
                var renderer = new HtmlRenderer(writer);
                OnNewInstanceCreated(renderer);
                return renderer;
            }

            protected override void Reset(HtmlRenderer instance)
            {
                instance.Reset();
            }
        }
    }
}