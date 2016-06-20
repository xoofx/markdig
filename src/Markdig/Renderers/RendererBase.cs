// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers
{
    /// <summary>
    /// Base class for a <see cref="IMarkdownRenderer"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.IMarkdownRenderer" />
    public abstract class RendererBase : IMarkdownRenderer
    {
        private readonly Dictionary<Type, IMarkdownObjectRenderer> renderersPerType;
        private IMarkdownObjectRenderer previousRenderer;
        private Type previousObjectType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase"/> class.
        /// </summary>
        protected RendererBase()
        {
            ObjectRenderers = new ObjectRendererCollection();
            renderersPerType = new Dictionary<Type, IMarkdownObjectRenderer>();
        }

        public ObjectRendererCollection ObjectRenderers { get; }

        public abstract object Render(MarkdownObject markdownObject);

        public bool IsFirstInContainer { get; private set; }

        public bool IsLastInContainer { get; private set; }

        /// <summary>
        /// Occurs when before writing an object.
        /// </summary>
        public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteBefore;

        /// <summary>
        /// Occurs when after writing an object.
        /// </summary>
        public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteAfter;

        /// <summary>
        /// Writes the children of the specified <see cref="ContainerBlock"/>.
        /// </summary>
        /// <param name="containerBlock">The container block.</param>
        public void WriteChildren(ContainerBlock containerBlock)
        {
            if (containerBlock == null)
            {
                return;
            }

            var children = containerBlock;
            for (int i = 0; i < children.Count; i++)
            {
                var saveIsFirstInContainer = IsFirstInContainer;
                var saveIsLastInContainer = IsLastInContainer;

                IsFirstInContainer = i == 0;
                IsLastInContainer = i + 1 == children.Count;
                Write(children[i]);

                IsFirstInContainer = saveIsFirstInContainer;
                IsLastInContainer = saveIsLastInContainer;
            }
        }

        /// <summary>
        /// Writes the children of the specified <see cref="ContainerInline"/>.
        /// </summary>
        /// <param name="containerInline">The container inline.</param>
        public void WriteChildren(ContainerInline containerInline)
        {
            if (containerInline == null)
            {
                return;
            }

            bool isFirst = true;
            var inline = containerInline.FirstChild;
            while (inline != null)
            {
                var saveIsFirstInContainer = IsFirstInContainer;
                var saveIsLastInContainer = IsLastInContainer;
                IsFirstInContainer = isFirst;
                IsLastInContainer = inline.NextSibling == null;

                Write(inline);
                inline = inline.NextSibling;

                IsFirstInContainer = saveIsFirstInContainer;
                IsLastInContainer = saveIsLastInContainer;

                isFirst = false;
            }
        }

        /// <summary>
        /// Writes the specified Markdown object.
        /// </summary>
        /// <typeparam name="T">A MarkdownObject type</typeparam>
        /// <param name="obj">The Markdown object to write to this renderer.</param>
        public void Write<T>(T obj) where T : MarkdownObject
        {
            if (obj == null)
            {
                return;
            }

            var objectType = obj.GetType();

            // Calls before writing an object
            var writeBefore = ObjectWriteBefore;
            writeBefore?.Invoke(this, obj);

            // Handle regular renderers
            IMarkdownObjectRenderer renderer = previousObjectType == objectType ? previousRenderer : null;
            if (renderer == null && !renderersPerType.TryGetValue(objectType, out renderer))
            {
                for (int i = 0; i < ObjectRenderers.Count; i++)
                {
                    var testRenderer = ObjectRenderers[i];
                    if (testRenderer.Accept(this, obj))
                    {
                        renderersPerType[objectType] = renderer = testRenderer;
                        break;
                    }
                }
            }
            if (renderer != null)
            {
                renderer.Write(this, obj);
            }
            else
            {
                var containerBlock = obj as ContainerBlock;
                if (containerBlock != null)
                {
                    WriteChildren(containerBlock);
                }
                else
                {
                    var containerInline = obj as ContainerInline;
                    if (containerInline != null)
                    {
                        WriteChildren(containerInline);
                    }
                }
            }

            previousObjectType = objectType;
            previousRenderer = renderer;

            // Calls after writing an object
            var writeAfter = ObjectWriteAfter;
            writeAfter?.Invoke(this, obj);
        }
    }
}