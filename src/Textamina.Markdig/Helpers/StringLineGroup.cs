// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Textamina.Markdig.Helpers
{
    /// <summary>
    /// A group of <see cref="StringLine"/>.
    /// </summary>
    /// <seealso cref="System.Collections.IEnumerable" />
    public class StringLineGroup : IEnumerable
    {
        private static readonly StringLine[] Empty = new StringLine[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLineGroup"/> class.
        /// </summary>
        public StringLineGroup()
        {
            Lines = Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLineGroup"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public StringLineGroup(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Lines = Empty;
            Add(new StringSlice(text));
        }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        public StringLine[] Lines { get; private set; }

        /// <summary>
        /// Gets the number of lines.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Array.Clear(Lines, 0, Lines.Length);
            Count = 0;
        }

        /// <summary>
        /// Removes the line at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
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

        /// <summary>
        /// Adds the specified line to this instance.
        /// </summary>
        /// <param name="line">The line.</param>
        public void Add(ref StringLine line)
        {
            if (Count == Lines.Length) IncreaseCapacity();
            Lines[Count++] = line;
        }

        /// <summary>
        /// Adds the specified slice to this instance.
        /// </summary>
        /// <param name="slice">The slice.</param>
        public void Add(StringSlice slice)
        {
            if (Count == Lines.Length) IncreaseCapacity();
            Lines[Count++] = new StringLine(ref slice);
        }

        public override string ToString()
        {
            return ToSlice().ToString();
        }

        /// <summary>
        /// Converts the lines to a single <see cref="StringSlice"/> by concatenating the lines.
        /// </summary>
        /// <param name="lineOffsets">The position of the `\n` line offsets from the beginning of the returned slice.</param>
        /// <returns>A single slice concatenating the lines of this instance</returns>
        public StringSlice ToSlice(List<int> lineOffsets = null)
        {
            // Optimization case when no lines
            if (Count == 0)
            {
                if (lineOffsets != null)
                {
                    lineOffsets.Add(1);
                }
                return new StringSlice(string.Empty);
            }

            // Optimization case for a single line.
            if (Count == 1)
            {
                if (lineOffsets != null)
                {
                    lineOffsets.Add(Lines[0].Slice.End + 1);
                }
                return Lines[0];
            }

            // Else use a builder
            var builder = StringBuilderCache.Local();
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    if (lineOffsets != null)
                    {
                        lineOffsets.Add(builder.Length + 1); // Add 1 for \n and 1 for next line
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
                lineOffsets.Add(builder.Length); // Add 1 for \0
            }
            var str = builder.ToString();
            builder.Clear();
            return new StringSlice(str);
        }

        /// <summary>
        /// Converts this instance into a <see cref="ICharIterator"/>.
        /// </summary>
        /// <returns></returns>
        public Iterator ToCharIterator()
        {
            return new Iterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Lines.GetEnumerator();
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

        /// <summary>
        /// The iterator used to iterate other the lines.
        /// </summary>
        /// <seealso cref="ICharIterator" />
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