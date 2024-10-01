// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Renderers;

/// <summary>
/// A base class for rendering <see cref="Block" /> and <see cref="Syntax.Inlines.Inline" /> Markdown objects.
/// </summary>
/// <typeparam name="TRenderer">The type of the renderer.</typeparam>
/// <typeparam name="TObject">The type of the object.</typeparam>
/// <seealso cref="IMarkdownObjectRenderer" />
public abstract class MarkdownObjectRenderer<TRenderer, TObject> : IMarkdownObjectRenderer where TRenderer : RendererBase where TObject : MarkdownObject
{
    private OrderedList<TryWriteDelegate>? _tryWriters;

    protected MarkdownObjectRenderer() { }

    public delegate bool TryWriteDelegate(TRenderer renderer, TObject obj);

    public bool Accept(RendererBase renderer, Type objectType)
    {
        return typeof(TObject).IsAssignableFrom(objectType);
    }

    public virtual void Write(RendererBase renderer, MarkdownObject obj)
    {
        var typedRenderer = (TRenderer)renderer;
        var typedObj = (TObject)obj;

        if (_tryWriters is not null && TryWrite(typedRenderer, typedObj))
        {
            return;
        }

        Write(typedRenderer, typedObj);
    }

    private bool TryWrite(TRenderer renderer, TObject obj)
    {
        for (int i = 0; i < _tryWriters!.Count; i++)
        {
            var tryWriter = _tryWriters[i];
            if (tryWriter(renderer, obj))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the optional writers attached to this instance.
    /// </summary>
    public OrderedList<TryWriteDelegate> TryWriters => _tryWriters ??= new();

    /// <summary>
    /// Writes the specified Markdown object to the renderer.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="obj">The markdown object.</param>
    protected abstract void Write(TRenderer renderer, TObject obj);
}