// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using Markdig.Parsers;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// Base class for all syntax tree inlines.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.MarkdownObject" />
    public abstract class Inline : MarkdownObject, IInline
    {
        /// <summary>
        /// Gets the parent container of this inline.
        /// </summary>
        public ContainerInline Parent { get; internal set; }

        /// <summary>
        /// Gets the previous inline.
        /// </summary>
        public Inline PreviousSibling { get; private set; }

        /// <summary>
        /// Gets the next sibling inline.
        /// </summary>
        public Inline NextSibling { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is closed.
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// Inserts the specified inline after this instance.
        /// </summary>
        /// <param name="next">The inline to insert after this instance.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">Inline has already a parent</exception>
        public void InsertAfter(Inline next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            if (next.Parent != null)
            {
                throw new ArgumentException("Inline has already a parent", nameof(next));
            }

            var previousNext = NextSibling;
            if (previousNext != null)
            {
                previousNext.PreviousSibling = next;
            }

            next.PreviousSibling = this;
            next.NextSibling = previousNext;
            NextSibling = next;

            if (Parent != null)
            {
                Parent.OnChildInsert(next);
                next.Parent = Parent;
            }
        }

        /// <summary>
        /// Inserts the specified inline before this instance.
        /// </summary>
        /// <param name="previous">The inlnie previous to insert before this instance.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">Inline has already a parent</exception>
        public void InsertBefore(Inline previous)
        {
            if (previous == null) throw new ArgumentNullException(nameof(previous));
            if (previous.Parent != null)
            {
                throw new ArgumentException("Inline has already a parent", nameof(previous));
            }

            var previousSibling = PreviousSibling;
            if (previousSibling != null)
            {
                previousSibling.NextSibling = previous;
            }

            PreviousSibling = previous;
            previous.NextSibling = this;

            if (Parent != null)
            {
                Parent.OnChildInsert(previous);
                previous.Parent = Parent;
            }
        }

        /// <summary>
        /// Removes this instance from the current list and its parent
        /// </summary>
        public void Remove()
        {
            if (PreviousSibling != null)
            {
                PreviousSibling.NextSibling = NextSibling;
            }

            if (NextSibling != null)
            {
                NextSibling.PreviousSibling = PreviousSibling;
            }

            if (Parent != null)
            {
                Parent.OnChildRemove(this);

                PreviousSibling = null;
                NextSibling = null;
                Parent = null;
            }
        }

        /// <summary>
        /// Replaces this inline by the specified inline.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <param name="copyChildren">if set to <c>true</c> the children of this instance are copied to the specified inline.</param>
        /// <returns>The last children</returns>
        /// <exception cref="System.ArgumentNullException">If inlnie is null</exception>
        public Inline ReplaceBy(Inline inline, bool copyChildren = true)
        {
            if (inline == null) throw new ArgumentNullException(nameof(inline));

            // Save sibling
            var parent = Parent;
            var previousSibling = PreviousSibling;
            var nextSibling = NextSibling;
            Remove();

            if (previousSibling != null)
            {
                previousSibling.InsertAfter(inline);
            }
            else if (nextSibling != null)
            {
                nextSibling.InsertBefore(inline);
            }
            else if (parent != null)
            {
                parent.AppendChild(inline);
            }

            var container = this as ContainerInline;
            if (copyChildren && container != null)
            {
                var newContainer = inline as ContainerInline;
                // Don't append to a closed container
                if (newContainer != null && newContainer.IsClosed)
                {
                    newContainer = null;
                }
                // TODO: This part is not efficient as it is using child.Remove()
                // We need a method to quickly move all children without having to mess Next/Prev sibling
                var child = container.FirstChild;
                var lastChild = inline;
                while (child != null)
                {
                    var nextChild = child.NextSibling;
                    child.Remove();
                    if (newContainer != null)
                    {
                        newContainer.AppendChild(child);
                    }
                    else
                    {
                        lastChild.InsertAfter(child);
                    }
                    lastChild = child;
                    child = nextChild;
                }

                return lastChild;
            }

            return inline;
        }

        /// <summary>
        /// Determines whether this instance contains a parent of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the parent to check</typeparam>
        /// <returns><c>true</c> if this instance contains a parent of the specified type; <c>false</c> otherwise</returns>
        public bool ContainsParentOfType<T>() where T : Inline
        {
            var delimiter = this as T;
            if (delimiter != null)
            {
                return true;
            }

            if (Parent != null)
            {
                return Parent.ContainsParentOfType<T>();
            }

            return false;
        }

        /// <summary>
        /// Iterates on parents of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the parent to iterate over</typeparam>
        /// <returns>An enumeration on the parents of the specified type</returns>
        public IEnumerable<T> FindParentOfType<T>() where T : Inline
        {
            var parent = Parent;
            var delimiter = this as T;
            if (delimiter != null)
            {
                yield return delimiter;
            }

            if (parent != null)
            {
                foreach (var previous in parent.FindParentOfType<T>())
                {
                    yield return previous;
                }
            }
        }

        public Inline FindBestParent()
        {
            var current = this;

            while (current.Parent != null || current.PreviousSibling != null)
            {
                if (current.Parent != null)
                {
                    current = current.Parent;
                    continue;
                }

                current = current.PreviousSibling;
            }

            return current;
        }

        protected virtual void OnChildRemove(Inline child)
        {

        }

        protected virtual void OnChildInsert(Inline child)
        {
        }

        /// <summary>
        /// Dumps this instance to <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DumpTo(TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            DumpTo(writer, 0);
        }

        /// <summary>
        /// Dumps this instance to <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="level">The level of indent.</param>
        /// <exception cref="System.ArgumentNullException">if writer is null</exception>
        public void DumpTo(TextWriter writer, int level)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            for (int i = 0; i < level; i++)
            {
                writer.Write(' ');
            }

            writer.WriteLine("-> " + this.GetType().Name + " = " + this);

            DumpChildTo(writer, level + 1);

            if (NextSibling != null)
            {
                NextSibling.DumpTo(writer, level);
            }
        }

        protected virtual void DumpChildTo(TextWriter writer, int level)
        {
        }
    }
}