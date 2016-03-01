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

        bool IsEmpty { get; }

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <returns><c>true</c> if it has reaches the end of the iterator</returns>
        bool TrimStart();
    }

    public static class CharIteratorHelper
    {
        public static bool TrimStartAndCountNewLines<T>(ref T iterator, out int countNewLines) where T : ICharIterator
        {
            countNewLines = 0;
            var c = iterator.CurrentChar;
            bool hasWhitespaces = false;
            while (c.IsWhitespace())
            {
                if (c == '\n')
                {
                    countNewLines++;
                }
                hasWhitespaces = true;
                c = iterator.NextChar();
            }
            return hasWhitespaces;
        }
    }

    public struct StringSlice : ICharIterator
    {
        public StringSlice(string text)
        {
            Text = text;
            Start = 0;
            End = (Text?.Length ?? 0) - 1;
        }

        public StringSlice(string text, int start, int end)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Text = text;
            Start = start;
            End = end;
        }

        public readonly string Text;

        public int Start { get; set; }

        public int End { get; set; }

        public int Length => End - Start + 1;

        public int Column => Start;

        public char CurrentChar => Start <= End ? this[Start] : '\0';

        public bool IsEmpty => Start > End;

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
            var start = Start;
            for (; Start <= End; Start++)
            {
                if (!Text[Start].IsWhitespace())
                {
                    break;
                }
            }
            return start != Start;
        }

        public void TrimStart(out int spaceCount)
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
            return IsEmpty;
        }

        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        public override string ToString()
        {
            if (Text != null && Start <= End)
            {
                var length = Length;
                if (Start == 0 && Text.Length == length)
                {
                    return Text;
                }

                return Text.Substring(Start, length);
            }
            return string.Empty;
        }

        public bool IsEmptyOrWhitespace()
        {
            for (int i = Start; i <= End; i++)
            {
                if (!Text[i].IsWhitespace())
                {
                    return false;
                }
            }
            return true;
        }
    }
}