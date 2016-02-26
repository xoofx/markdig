using System;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Syntax
{
    public interface ICharIterator
    {
        int Start { get; }

        char CurrentChar { get; }

        int End { get; }

        char NextChar();

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <returns><c>true</c> if it has reaches the end of the iterator</returns>
        bool TrimStart();

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <param name="spaceCount">The space count.</param>
        /// <returns><c>true</c> if it has reaches the end of the iterator</returns>
        bool TrimStart(out int spaceCount);
    }

    public struct StringSlice : ICharIterator
    {
        public StringSlice(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Text = text;
            Start = 0;
            End = Text.Length - 1;
        }

        public readonly string Text;

        public int Start { get; set; }

        public int End { get; set; }

        public int Length => End - Start + 1;

        public int Column => Start;

        public char CurrentChar => Start <= End ? this[Start] : '\0';

        public bool IsEndOfSlice => Start > End;

        public char this[int index] => Text[index];

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char NextChar()
        {
            Start++;
            if (Start > End)
            {
                Start = End + 1;
                return '\0';
            }
            return Text[Start];
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekChar(int offset)
        {
            var index = Start + offset;
            return index >= Start && index <= End ? Text[index] : (char) 0;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekCharExtra(int offset)
        {
            var index = Start + offset;
            return index >= 0 && index < Text.Length ? Text[index] : (char)0;
        }

        public bool Match(string text, int offset = 0)
        {
            return Match(text, End, offset);
        }

        public bool Match(string text, int end, int offset)
        {
            var index = Start + offset;
            int i = 0;
            for (; index <= end && i < text.Length; i++, index++)
            {
                if (text[i] != Text[index])
                {
                    return false;
                }
            }

            return i == text.Length;
        }

        public bool MatchLowercase(string text, int end, int offset)
        {
            var index = Start + offset;
            int i = 0;
            for (; index <= end && i < text.Length; i++, index++)
            {
                if (text[i] != char.ToLowerInvariant(Text[index]))
                {
                    return false;
                }
            }

            return i == text.Length;
        }

        public bool Search(string text, int offset = 0)
        {
            var end = End - text.Length + 1;
            for (int i = Start; i <= end; i++)
            {
                if (Match(text, End, i))
                {
                    return true;
                }
            }
            return false;
        }

        public bool SearchLowercase(string text, int offset = 0)
        {
            var end = End - text.Length + 1;
            for (int i = Start; i <= end; i++)
            {
                if (MatchLowercase(text, End, i))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TrimStart()
        {
            // Strip leading spaces
            for (; Start <= End; Start++)
            {
                if (!Text[Start].IsWhitespace())
                {
                    break;
                }
            }
            return Start > End;
        }

        public bool TrimStart(out int spaceCount)
        {
            spaceCount = 0;
            // Strip leading spaces
            for (; Start <= End; Start++)
            {
                if (!Text[Start].IsWhitespace())
                {
                    break;
                }
                spaceCount++;
            }
            return IsEndOfSlice;
        }

        public bool TrimEnd()
        {
            for (; Start <= End; End--)
            {
                if (!Text[End].IsWhitespace())
                {
                    break;
                }
            }
            return IsEndOfSlice;
        }

        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        public override string ToString()
        {
            return Text != null && Start <= End ? Text.Substring(Start, End - Start + 1) : string.Empty;
        }
    }
}