using System;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class ContainerInline : Inline
    {
        public Inline FirstChild { get; private set; }

        public Inline LastChild { get; private set; }

        public bool IsClosed { get; set; }

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

        protected internal override void Close(MatchInlineState state)
        {
            IsClosed = true;
        }
    }
}