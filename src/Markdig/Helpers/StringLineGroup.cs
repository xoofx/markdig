// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers;

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
        if (text is null) ThrowHelper.ArgumentNullException_text();
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

    internal void RemoveStartRange(int toRemove)
    {
        int remaining = Count - toRemove;
        Count = remaining;
        Array.Copy(Lines, toRemove, Lines, 0, remaining);
        Array.Clear(Lines, remaining, toRemove);
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
    public readonly StringSlice ToSlice(List<LineOffset>? lineOffsets = null)
    {
        // Optimization case for a single line.
        if (Count == 1)
        {
            ref StringLine line = ref Lines[0];
            lineOffsets?.Add(new LineOffset(line.Position, line.Column, line.Slice.Start - line.Position, line.Slice.Start, line.Slice.End + 1));
            return Lines[0];
        }

        // Optimization case when no lines
        if (Count == 0)
        {
            return StringSlice.Empty;
        }

        if (lineOffsets != null && lineOffsets.Capacity < lineOffsets.Count + Count)
        {
            lineOffsets.Capacity = Math.Max(lineOffsets.Count + Count, lineOffsets.Capacity * 2);
        }

        // Else use a builder
        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        int previousStartOfLine = 0;
        var newLine = NewLine.None;
        for (int i = 0; i < Count; i++)
        {
            if (i > 0)
            {
                builder.Append(newLine.AsString());
                previousStartOfLine = builder.Length;
            }
            ref StringLine line = ref Lines[i];
            if (!line.Slice.IsEmpty)
            {
                builder.Append(line.Slice.AsSpan());
            }
            newLine = line.NewLine;

            lineOffsets?.Add(new LineOffset(line.Position, line.Column, line.Slice.Start - line.Position, previousStartOfLine, builder.Length));
        }
        return new StringSlice(builder.ToString());
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

    public struct Enumerator(StringLineGroup parent) : IEnumerator
    {
        private readonly StringLineGroup _parent = parent;
        private int _index = -1;

        public object Current => _parent.Lines[_index];

        public bool MoveNext()
        {
            return ++_index < _parent.Count;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
    
    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator() 
    {
        return GetEnumerator();
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
        Lines = null!;
        Count = -1;
    }

    /// <summary>
    /// The iterator used to iterate other the lines.
    /// </summary>
    /// <seealso cref="ICharIterator" />
    public struct Iterator : ICharIterator
    {
        private readonly StringLineGroup _lines;
        private StringSlice _currentSlice;
        private int _offset;

        public Iterator(StringLineGroup stringLineGroup)
        {
            _lines = stringLineGroup;
            Start = -1;
            _offset = -1;
            SliceIndex = 0;
            CurrentChar = '\0';
            End = -1;
            StringLine[] lines = stringLineGroup.Lines;
            for (int i = 0; i < stringLineGroup.Count && i < lines.Length; i++)
            {
                ref StringSlice slice = ref lines[i].Slice;
                End += slice.Length + slice.NewLine.Length(); // Add chars
            }
            _currentSlice = _lines.Lines[0].Slice;
            SkipChar();
        }

        public int Start { get; private set; }

        public char CurrentChar { get; private set; }

        public int End { get; private set; }

        public readonly bool IsEmpty => Start > End;

        public int SliceIndex { get; private set; }

        public StringLineGroup Remaining()
        {
            StringLineGroup lines = _lines;
            if (IsEmpty)
            {
                lines.Clear();
            }
            else
            {
                lines.RemoveStartRange(SliceIndex);

                if (lines.Count > 0 && _offset > 0)
                {
                    ref StringLine line = ref lines.Lines[0];
                    line.Column += _offset;
                    line.Slice.Start += _offset;
                }
            }

            return lines;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char NextChar()
        {
            Start++;
            if (Start <= End)
            {
                ref StringSlice slice = ref _currentSlice;
                _offset++;

                int index = slice.Start + _offset;
                string text = slice.Text;
                if (index <= slice.End && (uint)index < (uint)text.Length)
                {
                    char c = text[index];
                    CurrentChar = c;
                    return c;
                }
                else
                {
                    return NextCharNewLine();
                }
            }
            else
            {
                return NextCharEndOfEnumerator();
            }
        }

        private char NextCharNewLine()
        {
            int sliceLength = _currentSlice.Length;
            NewLine newLine = _currentSlice.NewLine;

            if (_offset == sliceLength)
            {
                if (newLine == NewLine.LineFeed)
                {
                    CurrentChar = '\n';
                    goto MoveToNewLine;
                }
                else if (newLine == NewLine.CarriageReturn)
                {
                    CurrentChar = '\r';
                    goto MoveToNewLine;
                }
                else if (newLine == NewLine.CarriageReturnLineFeed)
                {
                    CurrentChar = '\r';
                }
            }
            else if (_offset - 1 == sliceLength)
            {
                if (newLine == NewLine.CarriageReturnLineFeed)
                {
                    CurrentChar = '\n';
                    goto MoveToNewLine;
                }
            }

            goto Return;

        MoveToNewLine:
            if (SliceIndex < _lines.Lines.Length - 1)
            {
                SliceIndex++;
                _offset = -1;
                _currentSlice = _lines.Lines[SliceIndex];
            }

        Return:
            return CurrentChar;
        }

        private char NextCharEndOfEnumerator()
        {
            CurrentChar = '\0';
            Start = End + 1;
            SliceIndex = _lines.Count;
            return '\0';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipChar() => NextChar();

        public readonly char PeekChar() => PeekChar(1);

        public readonly char PeekChar(int offset)
        {
            if (offset < 0) ThrowHelper.ArgumentOutOfRangeException("Negative offset are not supported for StringLineGroup", nameof(offset));

            if (Start + offset > End)
            {
                return '\0';
            }

            offset += _offset;

            int sliceIndex = SliceIndex;
            ref StringSlice slice = ref _lines.Lines[sliceIndex].Slice;
            NewLine newLine = slice.NewLine;

            if (!(newLine == NewLine.CarriageReturnLineFeed && offset == slice.Length + 1))
            {
                while (offset > slice.Length)
                {
                    // We are not peeking at the same line
                    offset -= slice.Length + 1; // + 1 for new line

                    Debug.Assert(sliceIndex + 1 < _lines.Count, "'Start + offset > End' check above should prevent us from indexing out of range");
                    slice = ref _lines.Lines[++sliceIndex].Slice;
                }
            }
            else
            {
                if (slice.NewLine == NewLine.CarriageReturnLineFeed)
                {
                    return '\n'; // /n of /r/n (second character)
                }
            }

            if (offset == slice.Length)
            {
                if (newLine == NewLine.LineFeed)
                {
                    return '\n';
                }
                if (newLine == NewLine.CarriageReturn)
                {
                    return '\r';
                }
                if (newLine == NewLine.CarriageReturnLineFeed)
                {
                    return '\r'; // /r of /r/n (first character)
                }
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

    public readonly struct LineOffset(
        int linePosition,
        int column,
        int offset,
        int start,
        int end)
    {
        public readonly int LinePosition = linePosition;

        public readonly int Column = column;

        public readonly int Offset = offset;

        public readonly int Start = start;

        public readonly int End = end;
    }
}
