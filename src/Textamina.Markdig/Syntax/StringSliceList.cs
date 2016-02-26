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
            if (Count == 0)
            {
                return string.Empty;
            }

            if (Count == 1)
            {
                return !Slices[0].IsEndOfSlice ? Slices[0].Text.Substring(Slices[0].Start, Slices[0].Length) : string.Empty;
            }

            var builder = StringBuilderCache.Local();
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    builder.Append('\n');
                }
                if (!Slices[i].IsEndOfSlice)
                {
                    builder.Append(Slices[i].Text, Slices[i].Start, Slices[i].Length);
                }
            }
            var str = builder.ToString();
            builder.Clear();
            return str;
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
                while (c.IsWhitespace())
                {
                    c = NextChar();
                }
                return c == '\0';
            }

            public bool TrimStart(out int spaceCount)
            {
                spaceCount = 0;
                var c = CurrentChar;
                while (c.IsWhitespace())
                {
                    c = NextChar();
                    spaceCount++;
                }
                return c == '\0';
            }
        }
    }
}