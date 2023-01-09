// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Markdig.Helpers;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// A base class for container for <see cref="Inline"/>.
    /// </summary>
    /// <seealso cref="Inline" />
    public class ContainerInline : Inline, IEnumerable<Inline>
    {
        public ContainerInline()
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

        protected override void DumpChildTo(TextWriter writer, int level)
        {
            if (FirstChild != null)
            {
                level++;
                FirstChild.DumpTo(writer, level);
            }
        }

        public struct Enumerator : IEnumerator<Inline>
        {
            private readonly ContainerInline container;
            private Inline? currentChild;
            private Inline? nextChild;

            public Enumerator(ContainerInline container) : this()
            {
                if (container is null) ThrowHelper.ArgumentNullException(nameof(container));
                this.container = container;
                currentChild = nextChild = container.FirstChild;
            }

            public Inline Current => currentChild!;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

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

            public void Reset()
            {
                currentChild = nextChild = container.FirstChild;
            }
        }

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
}