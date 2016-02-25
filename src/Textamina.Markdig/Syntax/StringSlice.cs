using System;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Syntax
{
    public struct StringSlice
    {
        public StringSlice(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Text = text;
            Start = 0;
            End = Text.Length - 1;
        }

        public readonly string Text;

        public int Start;

        public int End;

        public int Column => Start;

        public char CurrentChar => Start <= End ? this[Start] : '\0';

        public bool IsEndOfSlice => Start > End;

        public char this[int index] => Text[index].EscapeInsecure();

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char NextChar()
        {
            Start++;
            if (Start > End)
            {
                Start = End + 1;
            }
            return CurrentChar;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekChar(int offset)
        {
            var index = Start + offset;
            return index >= Start && index <= End ? Text[index].EscapeInsecure() : (char) 0;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekCharExtra(int offset)
        {
            var index = Start + offset;
            return index >= 0 && index < Text.Length ? Text[index].EscapeInsecure() : (char)0;
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
            var c = CurrentChar;
            var hasWhitespaces = false;
            while (c.IsWhitespace())
            {
                c = NextChar();
                hasWhitespaces = true;
            }
            return hasWhitespaces;
        }

        public bool TrimStart(out int newLineCount)
        {
            bool hasWhitespaces = false;
            newLineCount = 0;
            var c = CurrentChar;
            while (c.IsWhitespace())
            {
                if (c == '\n')
                {
                    newLineCount++;
                }
                c = NextChar();
                hasWhitespaces = true;
            }
            return hasWhitespaces;
        }

        public void TrimEnd(bool includeTabs = false)
        {
            for (int i = End; i >= Start; i--)
            {
                End = i;
                var c = this[i];
                if (!(includeTabs ? c.IsSpaceOrTab() : c.IsSpace()))
                {
                    break;
                }
            }
        }

        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        public override string ToString()
        {
            return Start <= End ? Text.Substring(Start, End - Start + 1) : string.Empty;
        }
    }
}