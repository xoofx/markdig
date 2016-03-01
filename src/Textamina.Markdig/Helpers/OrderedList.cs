using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Helpers
{
    public class OrderedList<T> : List<T>
    {
        public bool InsertBefore<TElement>(T element) where TElement : T
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    Insert(i, element);
                    return true;
                }
            }
            return false;
        }

        public TElement Find<TElement>() where TElement : T
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    return (TElement)this[i];
                }
            }
            return default(TElement);
        }

        public bool InsertAfter<TElement>(T element) where TElement : T
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    Insert(i + 1, element);
                    return true;
                }
            }
            return false;
        }

        public bool Contains<TElement>() where TElement : T
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ReplacyBy<TElement>(T element) where TElement : T
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    this[i] = element;
                    return true;
                }
            }
            return false;
        }
    }
}