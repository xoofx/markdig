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

        protected override void OnChildInsert(Inline child)
        {
            if (child.PreviousSibling == FirstChild && FirstChild == LastChild)
            {
                LastChild = child;
            }
            else if (child.PreviousSibling == LastChild)
            {
                LastChild = child;
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
                    FirstChild = child.NextSibling;
                }
            }
            else if (child == LastChild)
            {
                LastChild = child.NextSibling;
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