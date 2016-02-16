using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Textamina.Markdig.Parsing
{
    public sealed class StringLiner
    {
        public StringBuilder Text;

        public int Start { get; private set; }

        public int End { get; private set; }

        public int SpaceHeaderCount { get; set; }

        public int Column { get; private set; }

        public char Current { get; private set; }
        
        public bool IsEol => Start == Text.Length;

        private bool isBlankLine;

        public void ToEol()
        {
            Start = Text.Length;
            Current = '\0';
        }

        public char this[int index] => Text[index];


        public State Save()
        {
            return new State(Start, End, Column, SpaceHeaderCount, Current);
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
        public bool IsBlankLine()
        {
            if (isBlankLine)
            {
                return true;
            }

            for (int i = Start; i < Text.Length; i++)
            {
                if (!Utility.IsSpace(Text[i]))
                {
                    return false;
                }
            }
            isBlankLine = true;
            return true;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        internal void Initialize()
        {
            Start = 0;
            Column = 0;
            SpaceHeaderCount = 0;
            Current = Text.Length > 0 ? Text[0] : (char) 0;
            End = Text.Length - 1;
        }

        public bool SkipLeadingSpaces3()
        {
            var maxLength = Math.Min(3, Text.Length);
            for (int i = 0; i < maxLength; i++)
            {
                if (!Utility.IsSpace(Current))
                {
                    return false;
                }
                NextChar();
            }
            return true;
        }

        public override string ToString()
        {
            return Start <= End ? Text.ToString().Substring(Start, End - Start + 1) : string.Empty;
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