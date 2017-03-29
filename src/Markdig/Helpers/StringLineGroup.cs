// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Markdig.Extensions.Tables;

namespace Markdig.Helpers
{
    /// <summary>
    /// A group of <see cref="StringLine"/>.
    /// </summary>
    /// <seealso cref="System.Collections.IEnumerable" />
    public struct StringLineGroup : IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLineGroup"/> class.
        /// </summary>
        /// <param name="capacity"></param>
        public StringLineGroup(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            Lines = new StringLine[capacity];
            Count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLineGroup"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public StringLineGroup(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Lines = new StringLine[1];
            Count = 0;
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
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public void Add(ref StringLine line)
        {
            if (Count == Lines.Length) IncreaseCapacity();
            Lines[Count++] = line;
        }

        /// <summary>
        /// Adds the specified slice to this instance.
        /// </summary>
        /// <param name="slice">The slice.</param>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
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
        public StringSlice ToSlice(List<LineOffset> lineOffsets = null)
        {
            // Optimization case when no lines
            if (Count == 0)
            {
                return new StringSlice(string.Empty);
            }

            // Optimization case for a single line.
            if (Count == 1)
            {
                if (lineOffsets != null)
                {
                    lineOffsets.Add(new LineOffset(Lines[0].Position, Lines[0].Column, Lines[0].Slice.Start - Lines[0].Position, Lines[0].Slice.Start, Lines[0].Slice.End + 1));
                }
                return Lines[0];
            }

            // Else use a builder
            var builder = StringBuilderCache.Local();
            int previousStartOfLine = 0;
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    if (lineOffsets != null)
                    {
                        lineOffsets.Add(new LineOffset(Lines[i - 1].Position, Lines[i - 1].Column, Lines[i - 1].Slice.Start - Lines[i - 1].Position, previousStartOfLine, builder.Length));
                    }
                    builder.Append('\n');
                    previousStartOfLine = builder.Length;
                }
                if (!Lines[i].Slice.IsEmpty)
                {
                    builder.Append(Lines[i].Slice.Text, Lines[i].Slice.Start, Lines[i].Slice.Length);
                }
            }
            if (lineOffsets != null)
            {
                lineOffsets.Add(new LineOffset(Lines[Count - 1].Position, Lines[Count - 1].Column, Lines[Count - 1].Slice.Start - Lines[Count - 1].Position, previousStartOfLine, builder.Length));
            }
            var str = builder.ToString();
            builder.Length = 0;
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

        /// <summary>
        /// Trims each lines of the specified <see cref="StringLineGroup"/>.
        /// </summary>
        public void Trim()
        {
            for (int i = 0; i < Count; i++)
            {
                Lines[i].Slice.Trim();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Lines.GetEnumerator();
        }

        private void IncreaseCapacity()
        {
            var newItems = new StringLine[Lines.Length * 2];
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

            public char PeekChar()
            {
                if (Start + 1 > End)
                {
                    return '\0';
                }

                var slice = (StringSlice)lines.Lines[SliceIndex];
                if (offset + 1 >= slice.Length)
                {
                    return '\n';
                }

                return slice[slice.Start + offset + 1];
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

        public struct LineOffset
        {
            public LineOffset(int linePosition, int column, int offset, int start, int end)
            {
                LinePosition = linePosition;
                Column = column;
                Offset = offset;
                Start = start;
                End = end;
            }

            public readonly int LinePosition;

            public readonly int Column;

            public readonly int Offset;

            public readonly int Start;

            public readonly int End;
        }
    }
}