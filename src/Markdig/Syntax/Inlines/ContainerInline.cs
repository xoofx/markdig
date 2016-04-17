// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// A base class for container for <see cref="Inline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.Inline" />
    public class ContainerInline : Inline, IEnumerable<Inline>
    {
        /// <summary>
        /// Gets the first child.
        /// </summary>
        public Inline FirstChild { get; private set; }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        public Inline LastChild { get; private set; }

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
        /// <exception cref="System.ArgumentNullException">If child is null</exception>
        /// <exception cref="System.ArgumentException">Inline has already a parent</exception>
        public virtual ContainerInline AppendChild(Inline child)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));
            if (child.Parent != null)
            {
                throw new ArgumentException("Inline has already a parent", nameof(child));
            }

            if (FirstChild == null)
            {
                FirstChild = child;
                LastChild = child;
                child.Parent = this;
            }
            else
            {
                LastChild.InsertAfter(child);
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
            var child = FirstChild;
            while (child != null)
            {
                var next = child.NextSibling;

                if (child  is T)
                {
                    yield return (T)child;
                }

                if (child is ContainerInline)
                {
                    foreach (var subChild in ((ContainerInline) child).FindDescendants<T>())
                    {
                        yield return subChild;
                    }
                }
               
                child = next;
            }
        }

        /// <summary>
        /// Moves all the children of this container after the specified inline.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void MoveChildrenAfter(Inline parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            var child = FirstChild;
            var nextSibliing = parent;
            while (child != null)
            {
                var next = child.NextSibling;
                // TODO: optimize this
                child.Remove();
                nextSibliing.InsertAfter(child);
                nextSibliing = child;
                child = next;
            }
        }

        /// <summary>
        /// Embraces this instance by the specified container.
        /// </summary>
        /// <param name="container">The container to use to embrace this instance.</param>
        /// <exception cref="System.ArgumentNullException">If the container is null</exception>
        public void EmbraceChildrenBy(ContainerInline container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
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
            if (child.PreviousSibling == null && child.NextSibling == FirstChild)
            {
                FirstChild = child;
            }
            else if (child.NextSibling == null && child.PreviousSibling == LastChild)
            {
                LastChild = child;
            }

            if (LastChild == null)
            {
                LastChild = FirstChild;
            }
            else if (FirstChild == null)
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
            private Inline currentChild;
            private Inline nextChild;

            public Enumerator(ContainerInline container) : this()
            {
                if (container == null) throw new ArgumentNullException(nameof(container));
                this.container = container;
                currentChild = nextChild = container.FirstChild;
            }

            public Inline Current => currentChild;

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