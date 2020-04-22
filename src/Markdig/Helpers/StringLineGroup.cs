// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers
{
    /// <summary>
    /// A group of <see cref="StringLine"/>.
    /// </summary>
    /// <seealso cref="IEnumerable" />
    public struct StringLineGroup : IEnumerable
    {
        // Feel free to change these numbers if you see a positive change
        private static readonly CustomArrayPool<StringLine> _pool
            = new CustomArrayPool<StringLine>(512, 386, 128, 64);

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLineGroup"/> class.
        /// </summary>
        /// <param name="capacity"></param>
        public StringLineGroup(int capacity)
        {
            if (capacity <= 0) ThrowHelper.ArgumentOutOfRangeException(nameof(capacity));
            Lines = _pool.Rent(capacity);
            Count = 0;
        }
        internal StringLineGroup(int capacity, bool willRelease)
        {
            if (capacity <= 0) ThrowHelper.ArgumentOutOfRangeException(nameof(capacity));
            Lines = _pool.Rent(willRelease ? Math.Max(8, capacity) : capacity);
            Count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLineGroup"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public StringLineGroup(string text)
        {
            if (text == null) ThrowHelper.ArgumentNullException_text();
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
            if (index != Count - 1)
            {
                Array.Copy(Lines, index + 1, Lines, index, Count - index - 1);
            }

            Lines[Count - 1] = new StringLine();
            Count--;
        }

        /// <summary>
        /// Adds the specified line to this instance.
        /// </summary>
        /// <param name="line">The line.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ref StringLine line)
        {
            if (Count == Lines.Length) IncreaseCapacity();
            Lines[Count++] = line;
        }

        /// <summary>
        /// Adds the specified slice to this instance.
        /// </summary>
        /// <param name="slice">The slice.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(StringSlice slice)
        {
            if (Count == Lines.Length) IncreaseCapacity();
            Lines[Count++] = new StringLine(ref slice);
        }

        public readonly override string ToString()
        {
            return ToSlice().ToString();
        }

        /// <summary>
        /// Converts the lines to a single <see cref="StringSlice"/> by concatenating the lines.
        /// </summary>
        /// <param name="lineOffsets">The position of the `\n` line offsets from the beginning of the returned slice.</param>
        /// <returns>A single slice concatenating the lines of this instance</returns>
        public readonly StringSlice ToSlice(List<LineOffset> lineOffsets = null)
        {
            // Optimization case for a single line.
            if (Count == 1)
            {
                lineOffsets?.Add(new LineOffset(Lines[0].Position, Lines[0].Column, Lines[0].Slice.Start - Lines[0].Position, Lines[0].Slice.Start, Lines[0].Slice.End + 1));
                return Lines[0];
            }

            // Optimization case when no lines
            if (Count == 0)
            {
                return new StringSlice(string.Empty);
            }

            if (lineOffsets != null && lineOffsets.Capacity < lineOffsets.Count + Count)
            {
                lineOffsets.Capacity = Math.Max(lineOffsets.Count + Count, lineOffsets.Capacity * 2);
            }

            // Else use a builder
            var builder = StringBuilderCache.Local();
            int previousStartOfLine = 0;
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    builder.Append('\n');
                    previousStartOfLine = builder.Length;
                }
                ref var line = ref Lines[i];
                if (!line.Slice.IsEmpty)
                {
                    builder.Append(line.Slice.Text, line.Slice.Start, line.Slice.Length);
                }

                lineOffsets?.Add(new LineOffset(line.Position, line.Column, line.Slice.Start - line.Position, previousStartOfLine, builder.Length));
            }
            return new StringSlice(builder.GetStringAndReset());
        }

        /// <summary>
        /// Converts this instance into a <see cref="ICharIterator"/>.
        /// </summary>
        /// <returns></returns>
        public readonly Iterator ToCharIterator()
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
            var newItems = _pool.Rent(Lines.Length * 2);
            if (Count > 0)
            {
                Array.Copy(Lines, 0, newItems, 0, Count);
                Array.Clear(Lines, 0, Count);
            }
            _pool.Return(Lines);
            Lines = newItems;
        }

        internal void Release()
        {
            Array.Clear(Lines, 0, Count);
            _pool.Return(Lines);
            Lines = null;
            Count = -1;
        }

        /// <summary>
        /// The iterator used to iterate other the lines.
        /// </summary>
        /// <seealso cref="ICharIterator" />
        public struct Iterator : ICharIterator
        {
            private readonly StringLineGroup _lines;
            private int _offset;

            public Iterator(StringLineGroup lines)
            {
                this._lines = lines;
                Start = -1;
                _offset = -1;
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

            public readonly bool IsEmpty => Start > End;

            public int SliceIndex { get; private set; }

            public StringLineGroup Remaining()
            {
                var lines = _lines;
                if (CurrentChar == '\0')
                {
                    lines.Clear();
                }
                else
                {
                    for (int i = SliceIndex - 1; i >= 0; i--)
                    {
                        lines.RemoveAt(i);
                    }

                    if (lines.Count > 0 && _offset > 0)
                    {
                        lines.Lines[0].Column += _offset;
                        lines.Lines[0].Slice.Start += _offset;
                    }
                }

                return lines;
            }

            public char NextChar()
            {
                Start++;
                if (Start <= End)
                {
                    var slice = _lines.Lines[SliceIndex].Slice;
                    _offset++;
                    if (_offset < slice.Length)
                    {
                        CurrentChar = slice[slice.Start + _offset];
                    }
                    else
                    {
                        CurrentChar = '\n';
                        SliceIndex++;
                        _offset = -1;
                    }
                }
                else
                {
                    CurrentChar = '\0';
                    Start = End + 1;
                    SliceIndex = _lines.Count;
                }
                return CurrentChar;
            }

            public readonly char PeekChar(int offset = 1)
            {
                if (offset < 0) ThrowHelper.ArgumentOutOfRangeException("Negative offset are not supported for StringLineGroup", nameof(offset));

                if (Start + offset > End)
                {
                    return '\0';
                }

                offset += _offset;

                int sliceIndex = SliceIndex;
                var slice = _lines.Lines[sliceIndex].Slice;

                while (offset > slice.Length)
                {
                    // We are not peeking at the same line
                    offset -= slice.Length + 1; // + 1 for new line

                    Debug.Assert(sliceIndex + 1 < _lines.Lines.Length, "'Start + offset > End' check above should prevent us from indexing out of range");
                    slice = _lines.Lines[++sliceIndex].Slice;
                }

                if (offset == slice.Length)
                {
                    return '\n';
                }

                Debug.Assert(offset < slice.Length);
                return slice[slice.Start + offset];
            }

            public bool TrimStart()
            {
                var c = CurrentChar;
                while (c.IsWhitespace())
                {
                    c = NextChar();
                }
                return IsEmpty;
            }
        }

        public readonly struct LineOffset
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