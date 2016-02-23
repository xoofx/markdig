using System;
using System.Collections.Generic;
using System.IO;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class ContainerInline : Inline
    {
        public Inline FirstChild { get; private set; }

        public Inline LastChild { get; private set; }

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

        public virtual void AppendChild(Inline child)
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
        }

        public Inline FindChild(Inline childToFind)
        {
            var child = FirstChild;
            while (child != null)
            {
                var next = child.NextSibling;
                if (child == childToFind)
                {
                    return child;
                }
                child = next;
            }
            return null;
        }

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

        public void MoveChildrenAfter(Inline parent)
        {
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

        public void EmbraceChildrenBy(ContainerInline container)
        {
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
    }
}