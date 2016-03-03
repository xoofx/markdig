using System;
using System.Collections.Generic;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers
{
    public abstract class RendererBase : IMarkdownRenderer
    {
        private readonly Dictionary<Type, List<IMarkdownObjectRenderer>> openObjectRenderersPerType;
        private readonly Dictionary<Type, List<IMarkdownObjectRenderer>> closeObjectRenderersPerType;
        private readonly Dictionary<Type, IMarkdownObjectRenderer> renderersPerType;
        private IMarkdownObjectRenderer previousRenderer;
        private Type previousObjectType;

        protected RendererBase()
        {
            OpeningObjectRenderers = new ObjectRendererCollection();
            ObjectRenderers = new ObjectRendererCollection();
            ClosingObjectRenderers = new ObjectRendererCollection();
            openObjectRenderersPerType = new Dictionary<Type, List<IMarkdownObjectRenderer>>();
            closeObjectRenderersPerType = new Dictionary<Type, List<IMarkdownObjectRenderer>>();
            renderersPerType = new Dictionary<Type, IMarkdownObjectRenderer>();
        }

        public ObjectRendererCollection OpeningObjectRenderers { get; }

        public ObjectRendererCollection ObjectRenderers { get; }

        public ObjectRendererCollection ClosingObjectRenderers { get; }

        public abstract object Render(MarkdownObject markdownObject);

        public void WriteChildren(ContainerBlock containerBlock)
        {
            if (containerBlock == null)
            {
                return;
            }

            foreach (var block in containerBlock.Children)
            {
                Write(block);
            }
        }

        public void WriteChildren(ContainerInline containerInline)
        {
            if (containerInline == null)
            {
                return;
            }

            var inline = containerInline.FirstChild;
            while (inline != null)
            {
                Write(inline);
                inline = inline.NextSibling;
            }
        }

        public void Write<T>(T obj) where T : MarkdownObject
        {
            if (obj == null)
            {
                return;
            }

            var objectType = obj.GetType();

            // Handle opening renderers
            HandleOpenCloseRenderers(objectType, obj, true);

            // Handle regular renderers
            IMarkdownObjectRenderer renderer = previousObjectType == objectType ? previousRenderer : null;
            if (renderer == null && !renderersPerType.TryGetValue(objectType, out renderer))
            {
                foreach (var testRenderer in ObjectRenderers)
                {
                    if (testRenderer.Accept(this, objectType))
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
            
            // Handle closing renderers
            HandleOpenCloseRenderers(objectType, obj, false);

            previousObjectType = objectType;
            previousRenderer = renderer;
        }

        private void HandleOpenCloseRenderers(Type objectType, MarkdownObject markdownObject, bool open)
        {
            var map = open ? openObjectRenderersPerType : closeObjectRenderersPerType;
            var list = open ? OpeningObjectRenderers : ClosingObjectRenderers;

            List<IMarkdownObjectRenderer> renderers;

            if (!map.TryGetValue(objectType, out renderers))
            {
                foreach (var renderer in list)
                {
                    if (renderer.Accept(this, objectType))
                    {
                        if (renderers == null)
                        {
                            renderers = new List<IMarkdownObjectRenderer>();
                        }
                        renderers.Add(renderer);
                    }
                }
                map[objectType] = renderers;
            }

            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.Write(this, markdownObject);
                }
            }
        }
    }
}