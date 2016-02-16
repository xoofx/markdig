using System;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public sealed class StringLine
    {
        public StringLine(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Text = text;
            Start = 0;
            End = Text.Length - 1;
            Column = 0;
            SpaceHeaderCount = 0;
            Current = End >= 0 ? Text[0] : (char)0;
        }
        
        public string Text { get; private set; }

        public int Start { get; set; }

        public int End { get; set; }

        public int SpaceHeaderCount { get; set; }

        public int Column { get; private set; }

        public char Current { get; private set; }
        
        public bool IsEol => Start > End;

        private bool isBlankLine;

        public char this[int index] => Text[index];


        public State Save()
        {
            return new State(Start, End, Column, SpaceHeaderCount, Current);
        }

        public void Restore(State state)
        {
            Restore(ref state);
        }

        public void Restore(ref State state)
        {
            Start = state.Start;
            End = state.End;
            Column = state.Column;
            SpaceHeaderCount = state.SpaceHeaderCount;
            Current = state.Current;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char NextChar()
        {
            Start++;
            if (Start <= End)
            {
                // If previous character was a tab make the Column += 4
                Column++;
                if (Utility.IsTab(Current))
                {
                    // Align the tab on a column
                    Column = ((Column + 3) / 4) * 4;
                }
                Current = Text[Start];
            }
            else
            {
                Start = End + 1;
                Current = (char) 0;
            }
            return Current;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekChar(int offset)
        {
            var index = Start + offset;
            return index <= End ? Text[index] : (char) 0;
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

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public bool IsBlankLine()
        {
            if (isBlankLine)
            {
                return true;
            }

            for (int i = Start; i <= End; i++)
            {
                if (!Utility.IsSpace(Text[i]))
                {
                    return false;
                }
            }
            isBlankLine = true;
            return true;
        }
        
        public void SkipLeadingSpaces3()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!Utility.IsSpace(Current))
                {
                    break;
                }
                NextChar();
            }
        }

        public StringSlice ToSlice()
        {
            return new StringSlice(Text, Start, End);
        }

        public override string ToString()
        {
            return Start <= End ? Text.Substring(Start, End - Start + 1) : string.Empty;
        }

        public struct State
        {
            public State(int start, int end, int column, int spaceHeaderCount, char current)
            {
                Start = start;
                End = end;
                Column = column;
                SpaceHeaderCount = spaceHeaderCount;
                Current = current;
            }

            public readonly int Start;

            public readonly int End;

            public readonly int Column;

            public readonly int SpaceHeaderCount;

            public readonly char Current;
        }
    }
}