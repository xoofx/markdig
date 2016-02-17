using System.Collections.Generic;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class StringLineList : List<StringLine>
    {
        private StringLine currentLine;

        public int LinePosition { get; private set; }

        public int ColumnPosition { get; private set; }

        public char Current { get; private set; }

        internal void Initialize()
        {
            ColumnPosition = -1;
            currentLine = Count > 0 ? this[0] : null;
            NextChar();
        }

        public bool IsEndOfLines => Current == '\0';

        public State Save()
        {
            return new State(currentLine, LinePosition, ColumnPosition, Current);
        }

        public void Restore(ref State state)
        {
            currentLine = state.CurrentLine;
            LinePosition = state.LinePosition;
            ColumnPosition = state.ColumnPosition;
            Current = state.Current;
        }

        public char NextChar()
        {
            if (currentLine != null)
            {
                ColumnPosition++;
                if (ColumnPosition <= currentLine.End)
                {
                    Current = currentLine[ColumnPosition];
                }
                else
                {
                    ColumnPosition = -1;
                    LinePosition++;
                    currentLine = LinePosition < Count ? this[LinePosition] : null;
                    Current = '\n';
                }
            }
            else
            {
                Current = '\0';
            }

            return Current;
        }

        public void TrimStart()
        {
            if (Count == 0)
            {
                return;
            }

            // Strip leading and 
            var liner = this[0];
            while (Utility.IsSpace(liner.Current))
            {
                liner.NextChar();
            }
        }

        public void TrimEnd()
        {
            if (Count == 0)
            {
                return;
            }

            var liner = this[Count - 1];
            for (int i = liner.End; i >= liner.Start; i--)
            {
                liner.End = i;
                if (!Utility.IsSpace(liner[i]))
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


        public class State
        {
            public State(StringLine currentLine, int linePosition, int columnPosition, char current)
            {
                CurrentLine = currentLine;
                LinePosition = linePosition;
                ColumnPosition = columnPosition;
                Current = current;
            }

            public readonly StringLine CurrentLine;

            public readonly int LinePosition;

            public readonly int ColumnPosition;

            public readonly char Current;
        }
    }
}