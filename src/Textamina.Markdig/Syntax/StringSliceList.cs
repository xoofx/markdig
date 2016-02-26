using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Syntax
{
    public class StringSliceList : IEnumerable
    {
        private static readonly StringSlice[] Empty = new StringSlice[0];

        public StringSliceList()
        {
            Slices = Empty;
        }

        public StringSliceList(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Slices = Empty;
            Add(new StringSlice(text));
        }

        public StringSlice[] Slices;

        public int Count;

        public void Clear()
        {
            Array.Clear(Slices, 0, Slices.Length);
            Count = 0;
        }

        public void RemoveAt(int index)
        {
            if (Count - 1 == index)
            {
                Count--;
            }
            else
            {
                Array.Copy(Slices, index + 1, Slices, index, Count - index - 1);
                Slices[Count - 1] = new StringSlice();
                Count--;
            }
        }

        public void Add(ref StringSlice slice)
        {
            if (Count == Slices.Length) IncreaseCapacity();
            Slices[Count++] = slice;
        }

        public void Add(StringSlice slice)
        {
            if (Count == Slices.Length) IncreaseCapacity();
            Slices[Count++] = slice;
        }

        private void IncreaseCapacity()
        {
            int newCapacity = Slices.Length == 0 ? 4 : Slices.Length * 2;
            var newItems = new StringSlice[newCapacity];
            if (Count > 0)
            {
                Array.Copy(Slices, 0, newItems, 0, Count);
            }
            Slices = newItems;
        }

        public override string ToString()
        {
            return ToSlice().ToString();
        }

        public StringSlice ToSlice()
        {
            if (Count == 0)
            {
                return new StringSlice(string.Empty);
            }

            if (Count == 1)
            {
                return Slices[0];
            }

            var builder = StringBuilderCache.Local();
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    builder.Append('\n');
                }
                if (!Slices[i].IsEmpty)
                {
                    builder.Append(Slices[i].Text, Slices[i].Start, Slices[i].Length);
                }
            }
            var str = builder.ToString();
            builder.Clear();
            return new StringSlice(str);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Slices.GetEnumerator();
        }

        public Iterator ToCharIterator()
        {
            return new Iterator(this);
        }

        public struct Iterator : ICharIterator
        {
            private readonly StringSliceList lines;
            private int offset;

            public Iterator(StringSliceList lines)
            {
                this.lines = lines;
                Start = -1;
                offset = -1;
                SliceIndex = 0;
                CurrentChar = '\0';
                End = -2; 
                for (int i = 0; i < lines.Count; i++)
                {
                    End += lines.Slices[i].Length + 1; // Add chars
                }
                NextChar();
            }

            public int Start { get; private set; }

            public char CurrentChar { get; private set; }

            public int End { get; private set; }

            public bool IsEmpty => Start > End;

            public int SliceIndex { get; private set; }

            public char NextChar()
            {
                Start++;
                offset++;
                if (Start <= End)
                {
                    var slice = lines.Slices[SliceIndex];
                    if (offset < slice.Length)
                    {
                        CurrentChar = slice[slice.Start + offset];
                    }
                    else
                    {
                        CurrentChar = '\n';
                        SliceIndex++;
                        offset = -1;
                    }
                }
                else
                {
                    CurrentChar = '\0';
                    Start = End + 1;
                    SliceIndex = lines.Count;
                    offset--;
                }
                return CurrentChar;
            }

            public bool TrimStart()
            {
                var c = CurrentChar;
                bool hasSpaces = false;
                while (c.IsWhitespace())
                {
                    hasSpaces = true;
                    c = NextChar();
                }
                return hasSpaces;
            }
        }
    }
}