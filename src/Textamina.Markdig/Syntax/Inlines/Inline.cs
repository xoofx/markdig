using System;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class Inline
    {
        public Inline Parent { get; internal set; }

        public Inline PreviousSibling { get; private set; }

        public Inline NextSibling { get; internal set; }

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

        protected virtual void OnChildRemove(Inline child)
        {
            
        }

        protected virtual void OnChildInsert(Inline child)
        {
        }

        protected internal virtual void Close(MatchInlineState state)
        {
        }
    }
}