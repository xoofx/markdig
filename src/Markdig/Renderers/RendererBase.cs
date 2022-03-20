// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers
{
    /// <summary>
    /// Base class for a <see cref="IMarkdownRenderer"/>.
    /// </summary>
    /// <seealso cref="IMarkdownRenderer" />
    public abstract class RendererBase : IMarkdownRenderer
    {
        private readonly Dictionary<Type, IMarkdownObjectRenderer> _renderersPerType;
        private IMarkdownObjectRenderer? _previousRenderer;
        private Type? _previousObjectType;
        internal int _childrenDepth = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase"/> class.
        /// </summary>
        protected RendererBase()
        {
            ObjectRenderers = new ObjectRendererCollection();
            _renderersPerType = new Dictionary<Type, IMarkdownObjectRenderer>();
        }

        private IMarkdownObjectRenderer? TryGetRenderer(Type objectType)
        {
            for (int i = 0; i < ObjectRenderers.Count; i++)
            {
                var renderer = ObjectRenderers[i];
                if (renderer.Accept(this, objectType))
                {
                    _renderersPerType[objectType] = renderer;
                    _previousObjectType = objectType;
                    _previousRenderer = renderer;
                    return renderer;
                }
            }

            return null;
        }

        public ObjectRendererCollection ObjectRenderers { get; }

        public abstract object Render(MarkdownObject markdownObject);

        public bool IsFirstInContainer { get; private set; }

        public bool IsLastInContainer { get; private set; }

        /// <summary>
        /// Occurs when before writing an object.
        /// </summary>
        public event Action<IMarkdownRenderer, MarkdownObject>? ObjectWriteBefore;

        /// <summary>
        /// Occurs when after writing an object.
        /// </summary>
        public event Action<IMarkdownRenderer, MarkdownObject>? ObjectWriteAfter;

        /// <summary>
        /// Writes the children of the specified <see cref="ContainerBlock"/>.
        /// </summary>
        /// <param name="containerBlock">The container block.</param>
        public void WriteChildren(ContainerBlock containerBlock)
        {
            if (containerBlock is null)
            {
                return;
            }

            ThrowHelper.CheckDepthLimit(_childrenDepth++);

            bool saveIsFirstInContainer = IsFirstInContainer;
            bool saveIsLastInContainer = IsLastInContainer;

            var children = containerBlock;
            for (int i = 0; i < children.Count; i++)
            {
                IsFirstInContainer = i == 0;
                IsLastInContainer = i + 1 == children.Count;
                Write(children[i]);
            }

            IsFirstInContainer = saveIsFirstInContainer;
            IsLastInContainer = saveIsLastInContainer;

            _childrenDepth--;
        }

        /// <summary>
        /// Writes the children of the specified <see cref="ContainerInline"/>.
        /// </summary>
        /// <param name="containerInline">The container inline.</param>
        public void WriteChildren(ContainerInline containerInline)
        {
            if (containerInline is null)
            {
                return;
            }

            ThrowHelper.CheckDepthLimit(_childrenDepth++);

            bool saveIsFirstInContainer = IsFirstInContainer;
            bool saveIsLastInContainer = IsLastInContainer;

            bool isFirst = true;
            var inline = containerInline.FirstChild;
            while (inline != null)
            {
                IsFirstInContainer = isFirst;
                IsLastInContainer = inline.NextSibling is null;

                Write(inline);
                inline = inline.NextSibling;

                isFirst = false;
            }

            IsFirstInContainer = saveIsFirstInContainer;
            IsLastInContainer = saveIsLastInContainer;

            _childrenDepth--;
        }

        /// <summary>
        /// Writes the specified Markdown object.
        /// </summary>
        /// <param name="obj">The Markdown object to write to this renderer.</param>
        public void Write(MarkdownObject obj)
        {
            if (obj is null)
            {
                return;
            }

            // Calls before writing an object
            ObjectWriteBefore?.Invoke(this, obj);

            var objectType = obj.GetType();

            IMarkdownObjectRenderer? renderer;

            // Handle regular renderers
            if (objectType == _previousObjectType)
            {
                renderer = _previousRenderer;
            }
            else if (!_renderersPerType.TryGetValue(objectType, out renderer))
            {
                renderer = TryGetRenderer(objectType);
            }

            if (renderer != null)
            {
                renderer.Write(this, obj);
            }
            else if (obj is ContainerBlock containerBlock)
            {
                WriteChildren(containerBlock);
            }
            else if (obj is ContainerInline containerInline)
            {
                WriteChildren(containerInline);
            }

            // Calls after writing an object
            ObjectWriteAfter?.Invoke(this, obj);
        }
    }
}