using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Syntax
{
    public class StringLineGroup : IEnumerable
    {
        private static readonly StringLine[] Empty = new StringLine[0];

        public StringLineGroup()
        {
            Lines = Empty;
        }

        public StringLineGroup(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Lines = Empty;
            Add(new StringSlice(text));
        }

        public StringLine[] Lines { get; private set; }

        public int Count { get; private set; }

        public void Clear()
        {
            Array.Clear(Lines, 0, Lines.Length);
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
                Array.Copy(Lines, index + 1, Lines, index, Count - index - 1);
                Lines[Count - 1] = new StringLine();
                Count--;
            }
        }

        public void Add(ref StringLine line)
        {
            if (Count == Lines.Length) IncreaseCapacity();
            Lines[Count++] = line;
        }

        public void Add(StringSlice slice)
        {
            if (Count == Lines.Length) IncreaseCapacity();
            Lines[Count++] = new StringLine(ref slice);
        }

        private void IncreaseCapacity()
        {
            int newCapacity = Lines.Length == 0 ? 4 : Lines.Length * 2;
            var newItems = new StringLine[newCapacity];
            if (Count > 0)
            {
                Array.Copy(Lines, 0, newItems, 0, Count);
            }
            Lines = newItems;
        }

        public override string ToString()
        {
            return ToSlice().ToString();
        }

        public StringSlice ToSlice(List<int> lineOffsets = null)
        {
            if (Count == 0)
            {
                if (lineOffsets != null)
                {
                    lineOffsets.Add(1);
                }
                return new StringSlice(string.Empty);
            }

            if (Count == 1)
            {
                if (lineOffsets != null)
                {
                    lineOffsets.Add(Lines[0].Slice.End + 1);
                }
                return Lines[0];
            }

            var builder = StringBuilderCache.Local();
            int lineOffset = 0;
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    if (lineOffsets != null)
                    {
                        lineOffsets.Add(builder.Length + 2); // Add 1 for \n and 1 for next line
                    }
                    builder.Append('\n');
                }
                if (!Lines[i].Slice.IsEmpty)
                {
                    builder.Append(Lines[i].Slice.Text, Lines[i].Slice.Start, Lines[i].Slice.Length);
                }
            }
            if (lineOffsets != null)
            {
                lineOffsets.Add(builder.Length + 1); // Add 1 for \0
            }
            var str = builder.ToString();
            builder.Clear();
            return new StringSlice(str);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Lines.GetEnumerator();
        }

        public Iterator ToCharIterator()
        {
            return new Iterator(this);
        }

        public struct Iterator : ICharIterator
        {
            private readonly StringLineGroup lines;
            private int offset;

            public Iterator(StringLineGroup lines)
            {
                this.lines = lines;
                Start = -1;
                offset = -1;
                SliceIndex = 0;
                CurrentChar = '\0';
                End = -2; 
                for (int i = 0; i < lines.Count; i++)
                {
                    End += lines.Lines[i].Slice.Length + 1; // Add chars
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
                    var slice = (StringSlice)lines.Lines[SliceIndex];
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