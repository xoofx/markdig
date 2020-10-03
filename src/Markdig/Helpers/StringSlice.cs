// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers
{
    /// <summary>
    /// A lightweight struct that represents a slice of a string.
    /// </summary>
    /// <seealso cref="ICharIterator" />
    public struct StringSlice : ICharIterator
    {
        /// <summary>
        /// An empty string slice.
        /// </summary>
        public static readonly StringSlice Empty = new StringSlice(string.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSlice"/> struct.
        /// </summary>
        /// <param name="text">The text.</param>
        public StringSlice(string text)
        {
            Text = text;
            Start = 0;
            End = (Text?.Length ?? 0) - 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSlice"/> struct.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public StringSlice(string text, int start, int end)
        {
            if (text is null)
                ThrowHelper.ArgumentNullException_text();

            Text = text;
            Start = start;
            End = end;
        }

        /// <summary>
        /// The text of this slice.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Gets or sets the start position within <see cref="Text"/>.
        /// </summary>
        public int Start { readonly get; set; }

        /// <summary>
        /// Gets or sets the end position (inclusive) within <see cref="Text"/>.
        /// </summary>
        public int End { readonly get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public readonly int Length => End - Start + 1;

        /// <summary>
        /// Gets the current character.
        /// </summary>
        public readonly char CurrentChar
        {
            get
            {
                int start = Start;
                return start <= End ? Text[start] : '\0';
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public readonly bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Start > End;
        }

        /// <summary>
        /// Gets the <see cref="char"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A character in the slice at the specified index (not from <see cref="Start"/> but from the begining of the slice)</returns>
        public readonly char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Text[index];
        }


        /// <summary>
        /// Goes to the next character, incrementing the <see cref="Start" /> position.
        /// </summary>
        /// <returns>
        /// The next character. `\0` is end of the iteration.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char NextChar()
        {
            int start = Start;
            if (start >= End)
            {
                Start = End + 1;
                return '\0';
            }
            start++;
            Start = start;
            return Text[start];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int CountAndSkipChar(char matchChar)
        {
            string text = Text;
            int end = End;
            int current = Start;

            while (current <= end && (uint)current < (uint)text.Length && text[current] == matchChar)
            {
                current++;
            }

            int count = current - Start;
            Start = current;
            return count;
        }

        /// <summary>
        /// Peeks a character at the offset of 1 from the current <see cref="Start"/> position
        /// inside the range <see cref="Start"/> and <see cref="End"/>, returns `\0` if outside this range.
        /// </summary>
        /// <returns>The character at offset, returns `\0` if none.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly char PeekChar()
        {
            int index = Start + 1;
            return index <= End ? Text[index] : '\0';
        }

        /// <summary>
        /// Peeks a character at the specified offset from the current <see cref="Start"/> position
        /// inside the range <see cref="Start"/> and <see cref="End"/>, returns `\0` if outside this range.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The character at offset, returns `\0` if none.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly char PeekChar(int offset)
        {
            var index = Start + offset;
            return index >= Start && index <= End ? Text[index] : '\0';
        }

        /// <summary>
        /// Peeks a character at the specified offset from the current beginning of the string, without taking into account <see cref="Start"/> and <see cref="End"/>
        /// </summary>
        /// <returns>The character at offset, returns `\0` if none.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly char PeekCharAbsolute(int index)
        {
            string text = Text;
            return (uint)index < (uint)text.Length ? text[index] : '\0';
        }

        /// <summary>
        /// Peeks a character at the specified offset from the current begining of the slice
        /// without using the range <see cref="Start"/> or <see cref="End"/>, returns `\0` if outside the <see cref="Text"/>.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The character at offset, returns `\0` if none.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly char PeekCharExtra(int offset)
        {
            var index = Start + offset;
            var text = Text;
            return (uint)index < (uint)text.Length ? text[index] : '\0';
        }

        /// <summary>
        /// Matches the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text matches; <c>false</c> otherwise</returns>
        public readonly bool Match(string text, int offset = 0)
        {
            return Match(text, End, offset);
        }

        /// <summary>
        /// Matches the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="end">The end.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text matches; <c>false</c> otherwise</returns>
        public readonly bool Match(string text, int end, int offset)
        {
            var index = Start + offset;

            if (end - index + 1 < text.Length)
                return false;

            string sliceText = Text;
            for (int i = 0; i < text.Length; i++, index++)
            {
                if (text[i] != sliceText[index])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Expect spaces until a end of line. Return <c>false</c> otherwise.
        /// </summary>
        /// <returns><c>true</c> if whitespaces where matched until a end of line</returns>
        public bool SkipSpacesToEndOfLineOrEndOfDocument()
        {
            for (int i = Start; i <= End; i++)
            {
                var c = Text[i];
                if (c.IsWhitespace())
                {
                    if (c == '\0' || c == '\n' || (c == '\r' && i + 1 <= End && Text[i + 1] != '\n'))
                    {
                        return true;
                    }
                    continue;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Matches the specified text using lowercase comparison.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text matches; <c>false</c> otherwise</returns>
        public readonly bool MatchLowercase(string text, int offset = 0)
        {
            return MatchLowercase(text, End, offset);
        }

        /// <summary>
        /// Matches the specified text using lowercase comparison.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="end">The end.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text matches; <c>false</c> otherwise</returns>
        public readonly bool MatchLowercase(string text, int end, int offset)
        {
            var index = Start + offset;

            if (end - index + 1 < text.Length)
                return false;

            string sliceText = Text;
            for (int i = 0; i < text.Length; i++, index++)
            {
                if (text[i] != char.ToLowerInvariant(sliceText[index]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Searches the specified text within this slice.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="ignoreCase">true if ignore case</param>
        /// <returns><c>true</c> if the text was found; <c>false</c> otherwise</returns>
        public readonly int IndexOf(string text, int offset = 0, bool ignoreCase = false)
        {
            offset += Start;
            int length = End - offset + 1;

            if (length <= 0)
                return -1;

            return Text.IndexOf(text, offset, length, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        /// <summary>
        /// Searches for the specified character within this slice.
        /// </summary>
        /// <returns>A value >= 0 if the character was found, otherwise &lt; 0</returns>
        public readonly int IndexOf(char c)
        {
            int start = Start;
            int length = End - start + 1;

            if (length <= 0)
                return -1;

            return Text.IndexOf(c, start, length);
        }

        /// <summary>
        /// Trims whitespaces at the beginning of this slice starting from <see cref="Start"/> position.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if it has reaches the end of the iterator
        /// </returns>
        public bool TrimStart()
        {
            string text = Text;
            int end = End;
            int i = Start;

            while (i <= end && (uint)i < (uint)text.Length && text[i].IsWhitespace())
                i++;

            Start = i;
            return i > end;
        }

        /// <summary>
        /// Trims whitespaces at the beginning of this slice starting from <see cref="Start"/> position.
        /// </summary>
        /// <param name="spaceCount">The number of spaces trimmed.</param>
        public void TrimStart(out int spaceCount)
        {
            string text = Text;
            int end = End;
            int i = Start;

            while (i <= end && (uint)i < (uint)text.Length && text[i].IsWhitespace())
                i++;

            spaceCount = i - Start;
            Start = i;
        }

        /// <summary>
        /// Trims whitespaces at the end of this slice, starting from <see cref="End"/> position.
        /// </summary>
        /// <returns></returns>
        public bool TrimEnd()
        {
            string text = Text;
            int start = Start;
            int i = End;

            while (start <= i && (uint)i < (uint)text.Length && text[i].IsWhitespace())
                i--;

            End = i;
            return start > i;
        }

        /// <summary>
        /// Trims whitespaces from both the start and end of this slice.
        /// </summary>
        public void Trim()
        {
            string text = Text;
            int start = Start;
            int end = End;

            while (start <= end && (uint)start < (uint)text.Length && text[start].IsWhitespace())
                start++;

            while (start <= end && (uint)end < (uint)text.Length && text[end].IsWhitespace())
                end--;

            Start = start;
            End = end;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public readonly override string ToString()
        {
            string text = Text;
            int start = Start;
            int length = End - start + 1;

            if (text is null || length <= 0)
            {
                return string.Empty;
            }
            return text.Substring(start, length);
        }

        /// <summary>
        /// Determines whether this slice is empty or made only of whitespaces.
        /// </summary>
        /// <returns><c>true</c> if this slice is empty or made only of whitespaces; <c>false</c> otherwise</returns>
        public readonly bool IsEmptyOrWhitespace()
        {
            string text = Text;
            int end = End;

            for (int i = Start; i <= end && (uint)i < (uint)text.Length; i++)
            {
                if (!text[i].IsWhitespace())
                {
                    return false;
                }
            }
            return true;
        }
    }
}