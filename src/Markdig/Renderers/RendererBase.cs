// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers;

/// <summary>
/// Base class for a <see cref="IMarkdownRenderer"/>.
/// </summary>
/// <seealso cref="IMarkdownRenderer" />
public abstract class RendererBase : IMarkdownRenderer
{
    private const int SubTableCount = 32;

    private readonly struct RendererEntry
    {
        public readonly IntPtr Key;
        public readonly IMarkdownObjectRenderer? Renderer;

        public RendererEntry(IntPtr key, IMarkdownObjectRenderer? renderer)
        {
            Key = key;
            Renderer = renderer;
        }
    }

    private readonly RendererEntry[][] _renderersPerType;

    internal int _childrenDepth = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IntPtr GetKeyForType(MarkdownObject obj) => Type.GetTypeHandle(obj).Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SubTableIndex(IntPtr key) => (int)((((ulong)key) / 64) & (SubTableCount - 1));

    /// <summary>
    /// Initializes a new instance of the <see cref="RendererBase"/> class.
    /// </summary>
    protected RendererBase()
    {
        var entries = _renderersPerType = new RendererEntry[SubTableCount][];
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i] ??= [];
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private IMarkdownObjectRenderer? GetRendererInstance(MarkdownObject obj)
    {
        Type objectType = obj.GetType();
        IMarkdownObjectRenderer? renderer = null;

        foreach (var potentialRenderer in ObjectRenderers)
        {
            if (potentialRenderer.Accept(this, objectType))
            {
                renderer = potentialRenderer;
                break;
            }
        }

        IntPtr key = GetKeyForType(obj);

        ref RendererEntry[] entries = ref _renderersPerType[SubTableIndex(key)];
        Array.Resize(ref entries, entries.Length + 1);
        entries[entries.Length - 1] = new RendererEntry(key, renderer);

        return renderer;
    }

    public ObjectRendererCollection ObjectRenderers { get; } = new();

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

        for (int i = 0; i < containerBlock.Count; i++)
        {
            IsFirstInContainer = i == 0;
            IsLastInContainer = i + 1 == containerBlock.Count;
            Write(containerBlock[i]);
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

        IMarkdownObjectRenderer? renderer = null;
        IntPtr key = GetKeyForType(obj);

#if NETFRAMEWORK || NETSTANDARD
        RendererEntry[] renderers = _renderersPerType[SubTableIndex(key)];
#else
        RendererEntry[] renderers = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_renderersPerType), SubTableIndex(key));
#endif

        foreach (RendererEntry entry in renderers)
        {
            if (key == entry.Key)
            {
                renderer = entry.Renderer;
                goto Render;
            }
        }

        renderer = GetRendererInstance(obj);

    Render:
        if (renderer is not null)
        {
            renderer.Write(this, obj);
        }
        else if (obj.IsContainerInline)
        {
            WriteChildren(Unsafe.As<ContainerInline>(obj));
        }
        else if (obj.IsContainerBlock)
        {
            WriteChildren(Unsafe.As<ContainerBlock>(obj));
        }

        // Calls after writing an object
        ObjectWriteAfter?.Invoke(this, obj);
    }
}