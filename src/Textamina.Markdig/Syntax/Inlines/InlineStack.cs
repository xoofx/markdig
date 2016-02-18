/*
using System;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Describes an element in a stack of possible inline openers.
    /// </summary>
    internal sealed class InlineStack
    {
        /// <summary>
        /// The parser priority if this stack entry.
        /// </summary>
        public InlineStackPriority Priority;

        /// <summary>
        /// Previous entry in the stack. <c>null</c> if this is the last one.
        /// </summary>
        public InlineStack Previous;

        /// <summary>
        /// Next entry in the stack. <c>null</c> if this is the last one.
        /// </summary>
        public InlineStack Next;

        /// <summary>
        /// The at-the-moment text inline that could be transformed into the opener.
        /// </summary>
        public Inline StartingInline;

        /// <summary>
        /// The number of delimiter characters found for this opener.
        /// </summary>
        public int DelimiterCount;

        /// <summary>
        /// The character that was used in the opener.
        /// </summary>
        public char Delimiter;

        /// <summary>
        /// The position in the <see cref="Buffer"/> where this inline element was found.
        /// Used only if the specific parser requires this information.
        /// </summary>
        public StringLineGroup.State StartPosition;

        /// <summary>
        /// The flags set for this stack entry.
        /// </summary>
        public InlineStackFlags Flags;

        [Flags]
        public enum InlineStackFlags : byte
        {
            None = 0,
            Opener = 1,
            Closer = 2,
            ImageLink = 4
        }

        public enum InlineStackPriority : byte
        {
            Emphasis = 0,
            Links = 1,
            Maximum = Links
        }

        public InlineStack FindMatchingOpener(InlineStackPriority priority,
            char delimiter, out bool canClose)
        {
            canClose = true;
            var istack = this;
            while (true)
            {
                if (istack == null)
                {
                    // this cannot be a closer since there is no opener available.
                    canClose = false;
                    return null;
                }

                if (istack.Priority > priority ||
                    (istack.Delimiter == delimiter && 0 != (istack.Flags & InlineStackFlags.Closer)))
                {
                    // there might be a closer further back but we cannot go there yet because a higher priority element is blocking
                    // the other option is that the stack entry could be a closer for the same char - this means
                    // that any opener we might find would first have to be matched against this closer.
                    return null;
                }

                if (istack.Delimiter == delimiter)
                    return istack;

                istack = istack.Previous;
            }
        }

        public void AppendStackEntry(MatchInlineState subj)
        {
            if (subj.LastPendingInline != null)
            {
                Previous = subj.LastPendingInline;
                subj.LastPendingInline.Next = this;
            }

            if (subj.FirstPendingInline == null)
                subj.FirstPendingInline = this;

            subj.LastPendingInline = this;
        }

        /// <summary>
        /// Removes a subset of the stack.
        /// </summary>
        /// <param name="subj">The subject associated with this stack. Can be <c>null</c> if the pointers in the subject should not be updated.</param>
        /// <param name="last">The last entry to be removed. Can be <c>null</c> if everything starting from <paramref name="first" /> has to be removed.</param>
        public void RemoveStackEntry(MatchInlineState subj, InlineStack last)
        {
            var first = this;
            var curPriority = first.Priority;

            if (last == null)
            {
                if (first.Previous != null)
                    first.Previous.Next = null;
                else if (subj != null)
                    subj.FirstPendingInline = null;

                if (subj != null)
                {
                    last = subj.LastPendingInline;
                    subj.LastPendingInline = first.Previous;
                }

                first = first.Next;
            }
            else
            {
                if (first.Previous != null)
                    first.Previous.Next = last.Next;
                else if (subj != null)
                    subj.FirstPendingInline = last.Next;

                if (last.Next != null)
                    last.Next.Previous = first.Previous;
                else if (subj != null)
                    subj.LastPendingInline = first.Previous;

                if (first == last)
                    return;

                first = first.Next;
                last = last.Previous;
            }

            if (last == null || first == null)
                return;

            first.Previous = null;
            last.Next = null;

            // handle case like [*b*] (the whole [..] is being removed but the inner *..* must still be matched).
            // this is not done automatically because the initial * is recognized as a potential closer (assuming
            // potential scenario '*[*' ).
            if (curPriority > 0)
                PostProcessInlineStack(null, first, last, curPriority);
        }

        public static void PostProcessInlineStack(MatchInlineState subj, InlineStack first, InlineStack last,
            InlineStackPriority ignorePriority)
        {
            while (ignorePriority > 0)
            {
                var istack = first;
                while (istack != null)
                {
                    if (istack.Priority >= ignorePriority)
                    {
                        istack.RemoveStackEntry(subj, istack);
                    }
                    else if (0 != (istack.Flags & InlineStackFlags.Closer))
                    {
                        bool canClose;
                        var iopener = FindMatchingOpener(istack.Previous, istack.Priority, istack.Delimiter,
                            out canClose);
                        if (iopener != null)
                        {
                            bool retry = false;
                            if (iopener.Delimiter == '~')
                            {
                                iopener.MatchInlineStack(subj, istack.DelimiterCount, istack, true);
                                if (istack.DelimiterCount > 1)
                                    retry = true;
                            }
                            else
                            {
                                iopener.MatchInlineStack(subj, istack.DelimiterCount, istack, false);
                                if (istack.DelimiterCount > 0)
                                    retry = true;
                            }

                            if (retry)
                            {
                                // remove everything between opened and closer (not inclusive).
                                if (istack.Previous != null && iopener.Next != istack.Previous)
                                    iopener.Next.RemoveStackEntry(subj, istack.Previous);

                                continue;
                            }
                            else
                            {
                                // remove opener, everything in between, and the closer
                                iopener.RemoveStackEntry(subj, istack);
                            }
                        }
                        else if (!canClose)
                        {
                            // this case means that a matching opener does not exist
                            // remove the Closer flag so that a future Opener can be matched against it.
                            istack.Flags &= ~InlineStackFlags.Closer;
                        }
                    }

                    if (istack == last)
                        break;

                    istack = istack.Next;
                }

                ignorePriority--;
            }
        }


        public int MatchInlineStack(MatchInlineState subj, int closingDelimiterCount, InlineStack closer, bool onlySingleCharTag)
        {
            // calculate the actual number of delimiters used from this closer
            int useDelims;
            var openerDelims = this.DelimiterCount;

            if (closingDelimiterCount < 3 || openerDelims < 3)
            {
                useDelims = closingDelimiterCount <= openerDelims ? closingDelimiterCount : openerDelims;
                if (useDelims == 1 && onlySingleCharTag)
                    return 0;
            }
            else if (onlySingleCharTag)
                useDelims = 2;
            else
                useDelims = closingDelimiterCount % 2 == 0 ? 2 : 1;

            Inline inl = this.StartingInline;
            InlineTag tag = useDelims == 1 ? singleCharTag : doubleCharTag;
            if (openerDelims == useDelims)
            {
                // the opener is completely used up - remove the stack entry and reuse the inline element
                inl.Tag = tag;
                inl.LiteralContent = null;
                inl.FirstChild = inl.NextSibling;
                inl.NextSibling = null;

                RemoveStackEntry(subj, closer?.Previous);
            }
            else
            {
                // the opener will only partially be used - stack entry remains (truncated) and a new inline is added.
                this.DelimiterCount -= useDelims;
                inl.LiteralContent = inl.LiteralContent.Substring(0, this.DelimiterCount);
                inl.SourceLastPosition -= useDelims;

                inl.NextSibling = new Inline(tag, inl.NextSibling);
                inl = inl.NextSibling;

                inl.SourcePosition = this.StartingInline.SourcePosition + this.DelimiterCount;
            }

            // there are two callers for this method, distinguished by the `closer` argument.
            // if closer == null it means the method is called during the initial subject parsing and the closer
            //   characters are at the current position in the subject. The main benefit is that there is nothing
            //   parsed that is located after the matched inline element.
            // if closer != null it means the method is called when the second pass for previously unmatched
            //   stack elements is done. The drawback is that there can be other elements after the closer.
            if (closer != null)
            {
                var clInl = closer.StartingInline;
                if ((closer.DelimiterCount -= useDelims) > 0)
                {
                    // a new inline element must be created because the old one has to be the one that
                    // finalizes the children of the emphasis
                    var newCloserInline = new Inline(clInl.LiteralContent.Substring(useDelims));
                    newCloserInline.SourcePosition = inl.SourceLastPosition = clInl.SourcePosition + useDelims;
                    newCloserInline.SourceLength = closer.DelimiterCount;
                    newCloserInline.NextSibling = clInl.NextSibling;

                    clInl.LiteralContent = null;
                    clInl.NextSibling = null;
                    inl.NextSibling = closer.StartingInline = newCloserInline;
                }
                else
                {
                    inl.SourceLastPosition = clInl.SourceLastPosition;

                    clInl.LiteralContent = null;
                    inl.NextSibling = clInl.NextSibling;
                    clInl.NextSibling = null;
                }
            }
            else if (subj != null)
            {
                inl.SourceLastPosition = subj.Position - closingDelimiterCount + useDelims;
                subj.LastInline = inl;
            }

            return useDelims;
        }
    }
}
*/