// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Syntax.Inlines;

/// <summary>
/// A base class for container for <see cref="Inline"/>.
/// </summary>
/// <seealso cref="Inline" />
public class ContainerInline : Inline, IEnumerable<Inline>
{
    /// <summary>
    /// Initializes a new instance of the ContainerInline class.
    /// </summary>
    public ContainerInline() : base(dummySkipTypeKind: true)
    {
        SetTypeKind(isInline: true, isContainer: true);
    }

    /// <summary>
    /// Gets the parent block of this inline.
    /// </summary>
    public LeafBlock? ParentBlock { get; internal set; }

    /// <summary>
    /// Gets the first child.
    /// </summary>
    public Inline? FirstChild { get; private set; }

    /// <summary>
    /// Gets the last child.
    /// </summary>
    public Inline? LastChild { get; private set; }

    /// <summary>
    /// Clears this instance by removing all its children.
    /// </summary>
    public void Clear()
    {
        var child = LastChild;
        while (child != null)
        {
            child.Parent = null;
            child = child.PreviousSibling;
        }
        FirstChild = null;
        LastChild = null;
    }

    /// <summary>
    /// Appends a child to this container.
    /// </summary>
    /// <param name="child">The child to append to this container..</param>
    /// <returns>This instance</returns>
    /// <exception cref="ArgumentNullException">If child is null</exception>
    /// <exception cref="ArgumentException">Inline has already a parent</exception>
    public virtual ContainerInline AppendChild(Inline child)
    {
        if (child is null) ThrowHelper.ArgumentNullException(nameof(child));
        if (child.Parent != null)
        {
            ThrowHelper.ArgumentException("Inline has already a parent", nameof(child));
        }

        if (FirstChild is null)
        {
            FirstChild = child;
            LastChild = child;
            child.Parent = this;
        }
        else
        {
            LastChild!.InsertAfter(child);
        }
        return this;
    }

    /// <summary>
    /// Checks if this instance contains the specified child.
    /// </summary>
    /// <param name="childToFind">The child to find.</param>
    /// <returns><c>true</c> if this instance contains the specified child; <c>false</c> otherwise</returns>
    public bool ContainsChild(Inline childToFind)
    {
        var child = FirstChild;
        while (child != null)
        {
            var next = child.NextSibling;
            if (child == childToFind)
            {
                return true;
            }
            child = next;
        }
        return false;
    }

    /// <summary>
    /// Finds all the descendants.
    /// </summary>
    /// <typeparam name="T">Type of the descendants to find</typeparam>
    /// <returns>An enumeration of T</returns>
    public IEnumerable<T> FindDescendants<T>() where T : Inline
    {
        if (FirstChild is null)
        {
            return Array.Empty<T>();
        }
        else
        {
            return FindDescendantsInternal<T>();
        }
    }
    internal IEnumerable<T> FindDescendantsInternal<T>() where T : MarkdownObject
    {
        Debug.Assert(typeof(T).IsSubclassOf(typeof(Inline)));

        var stack = new Stack<Inline>();

        var child = LastChild;
        while (child != null)
        {
            stack.Push(child);
            child = child.PreviousSibling;
        }

        while (stack.Count > 0)
        {
            child = stack.Pop();

            if (child is T childT)
            {
                yield return childT;
            }

            if (child is ContainerInline containerInline)
            {
                child = containerInline.LastChild;
                while (child != null)
                {
                    stack.Push(child);
                    child = child.PreviousSibling;
                }
            }
        }
    }

    /// <summary>
    /// Transfers all children from this container to <paramref name="destination"/>.
    /// </summary>
    /// <param name="destination">The destination container.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="destination"/> is null.</exception>
    public void TransferChildrenTo(ContainerInline destination)
    {
        if (destination is null) ThrowHelper.ArgumentNullException(nameof(destination));

        if (ReferenceEquals(this, destination))
        {
            return;
        }

        var child = FirstChild;
        while (child != null)
        {
            var next = child.NextSibling;
            child.Remove();
            destination.AppendChild(child);
            child = next;
        }
    }

    /// <summary>
    /// Moves all the children of this container after the specified inline.
    /// </summary>
    /// <param name="parent">The parent.</param>
    public void MoveChildrenAfter(Inline parent)
    {
        if (parent is null) ThrowHelper.ArgumentNullException(nameof(parent));
        var child = FirstChild;
        var nextSibling = parent;
        while (child != null)
        {
            var next = child.NextSibling;
            // TODO: optimize this
            child.Remove();
            nextSibling.InsertAfter(child);
            nextSibling = child;
            child = next;
        }
    }

    /// <summary>
    /// Embraces this instance by the specified container.
    /// </summary>
    /// <param name="container">The container to use to embrace this instance.</param>
    /// <exception cref="ArgumentNullException">If the container is null</exception>
    public void EmbraceChildrenBy(ContainerInline container)
    {
        if (container is null) ThrowHelper.ArgumentNullException(nameof(container));
        var child = FirstChild;
        while (child != null)
        {
            var next = child.NextSibling;
            // TODO: optimize this
            child.Remove();
            container.AppendChild(child);
            child = next;
        }
        AppendChild(container);
    }

    /// <summary>
    /// Performs the on child insert operation.
    /// </summary>
    protected override void OnChildInsert(Inline child)
    {
        // A child is inserted before the FirstChild
        if (child.PreviousSibling is null && child.NextSibling == FirstChild)
        {
            FirstChild = child;
        }
        else if (child.NextSibling is null && child.PreviousSibling == LastChild)
        {
            LastChild = child;
        }

        if (LastChild is null)
        {
            LastChild = FirstChild;
        }
        else if (FirstChild is null)
        {
            FirstChild = LastChild;
        }
    }

    /// <summary>
    /// Performs the on child remove operation.
    /// </summary>
    protected override void OnChildRemove(Inline child)
    {
        if (child == FirstChild)
        {
            if (FirstChild == LastChild)
            {
                FirstChild = null;
                LastChild = null;
            }
            else
            {
                FirstChild = child.NextSibling ?? LastChild;
            }
        }
        else if (child == LastChild)
        {
            LastChild = child.PreviousSibling ?? FirstChild;
        }
    }

    /// <summary>
    /// Performs the dump child to operation.
    /// </summary>
    protected override void DumpChildTo(TextWriter writer, int level)
    {
        if (FirstChild != null)
        {
            level++;
            FirstChild.DumpTo(writer, level);
        }
    }

    /// <summary>
    /// Checks whether this container span is valid with respect to child inline spans.
    /// </summary>
    /// <param name="recursive">When <c>true</c>, validates descendant container inline spans recursively.</param>
    /// <returns>
    /// <c>true</c> when this container span contains all direct child spans and recursive checks (if enabled) succeed;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool HasValidSpan(bool recursive = false)
    {
        var child = FirstChild;
        while (child is not null)
        {
            if (!ContainsSpan(Span, child.Span))
            {
                return false;
            }

            if (recursive && child is ContainerInline childContainer && !childContainer.HasValidSpan(recursive: true))
            {
                return false;
            }

            child = child.NextSibling;
        }

        return true;
    }

    /// <summary>
    /// Updates this container span from its child inline spans.
    /// </summary>
    /// <param name="recursive">When <c>true</c>, updates descendant container inline spans recursively before updating this container.</param>
    /// <param name="preserveSelfSpan">
    /// When <c>true</c>, preserves this container current span and only expands it to include children.
    /// When <c>false</c>, recomputes from children only.
    /// </param>
    /// <returns><c>true</c> when this container span changed; otherwise, <c>false</c>.</returns>
    public bool UpdateSpanFromChildren(bool recursive = false, bool preserveSelfSpan = true)
    {
        var updatedSpan = SourceSpan.Empty;
        bool hasUpdatedSpan = false;

        if (preserveSelfSpan && !Span.IsEmpty)
        {
            updatedSpan = Span;
            hasUpdatedSpan = true;
        }

        var child = FirstChild;
        while (child is not null)
        {
            if (recursive && child is ContainerInline childContainer)
            {
                childContainer.UpdateSpanFromChildren(recursive: true, preserveSelfSpan: preserveSelfSpan);
            }

            AppendSpan(ref updatedSpan, ref hasUpdatedSpan, child.Span);
            child = child.NextSibling;
        }

        if (!hasUpdatedSpan)
        {
            updatedSpan = SourceSpan.Empty;
        }

        if (updatedSpan == Span)
        {
            return false;
        }

        Span = updatedSpan;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsSpan(in SourceSpan containerSpan, in SourceSpan childSpan)
    {
        return childSpan.IsEmpty || (!containerSpan.IsEmpty && childSpan.Start >= containerSpan.Start && childSpan.End <= containerSpan.End);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendSpan(ref SourceSpan destinationSpan, ref bool hasDestinationSpan, in SourceSpan spanToAppend)
    {
        if (spanToAppend.IsEmpty)
        {
            return;
        }

        if (!hasDestinationSpan)
        {
            destinationSpan = spanToAppend;
            hasDestinationSpan = true;
            return;
        }

        if (spanToAppend.Start < destinationSpan.Start)
        {
            destinationSpan.Start = spanToAppend.Start;
        }

        if (spanToAppend.End > destinationSpan.End)
        {
            destinationSpan.End = spanToAppend.End;
        }
    }

    /// <summary>
    /// Represents the Enumerator type.
    /// </summary>
    public struct Enumerator : IEnumerator<Inline>
    {
        private readonly ContainerInline container;
        private Inline? currentChild;
        private Inline? nextChild;

        /// <summary>
        /// Initializes a new instance of the Enumerator class.
        /// </summary>
        public Enumerator(ContainerInline container) : this()
        {
            if (container is null) ThrowHelper.ArgumentNullException(nameof(container));
            this.container = container;
            currentChild = nextChild = container.FirstChild;
        }

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        public Inline Current => currentChild!;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Performs the dispose operation.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Performs the move next operation.
        /// </summary>
        public bool MoveNext()
        {
            currentChild = nextChild;
            if (currentChild != null)
            {
                nextChild = currentChild.NextSibling;
                return true;
            }
            nextChild = null;
            return false;
        }

        /// <summary>
        /// Performs the reset operation.
        /// </summary>
        public void Reset()
        {
            currentChild = nextChild = container.FirstChild;
        }
    }

    /// <summary>
    /// Gets enumerator.
    /// </summary>
    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator<Inline> IEnumerable<Inline>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
