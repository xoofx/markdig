using System;
using System.Collections.ObjectModel;
using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class StringLineGroup : Collection<StringLine>
    {
        private StringLine currentLine;

        public StringLineGroup()
        {
        }

        public StringLineGroup(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Add(new StringLine(text));
        }

        public int LinePosition { get; private set; }

        public int ColumnPosition { get; private set; }

        public char Current { get; private set; }

        public char PreviousChar1 { get; private set; }

        public char PreviousChar2 { get; private set; }

        public void Reset()
        {
            ColumnPosition = -1;
            currentLine = Count > 0 ? this[0] : null;
            if (currentLine != null)
            {
                ColumnPosition = currentLine.Start - 1;
            }
            NextChar();
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            Reset();
        }

        protected override void InsertItem(int index, StringLine item)
        {
            base.InsertItem(index, item);
            if (index == 0)
            {
                Reset();
            }
        }

        protected override void SetItem(int index, StringLine item)
        {
            base.SetItem(index, item);
            if (index == 0)
            {
                Reset();
            }
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            if (index == 0)
            {
                Reset();
            }
        }

        public bool IsEndOfLines => Current == '\0';

        public State Save()
        {
            return new State(currentLine, LinePosition, ColumnPosition, Current, PreviousChar1, PreviousChar2);
        }

        public void Restore(ref State state)
        {
            currentLine = state.CurrentLine;
            LinePosition = state.LinePosition;
            ColumnPosition = state.ColumnPosition;
            Current = state.Current;
            PreviousChar1 = state.PreviousChar1;
            PreviousChar2 = state.PreviousChar2;
        }

        public char NextChar()
        {
            PreviousChar2 = PreviousChar1;
            PreviousChar1 = Current;
            if (currentLine != null)
            {
                ColumnPosition++;
                if (ColumnPosition <= currentLine.End)
                {
                    Current = currentLine[ColumnPosition];
                }
                else
                {
                    LinePosition++;
                    currentLine = LinePosition < Count ? this[LinePosition] : null;
                    ColumnPosition = -1;
                    if (currentLine != null)
                    {
                        ColumnPosition = currentLine.Start - 1;
                    }
                    Current = currentLine != null ? '\n' : '\0';
                }
            }
            else
            {
                Current = '\0';
            }

            return Current;
        }

        public bool SkipWhiteSpaces()
        {
            bool hasWhitespaces = false;
            while (Utility.IsWhitespace(Current))
            {
                NextChar();
                hasWhitespaces = true;
            }
            return hasWhitespaces;
        }

        public void TrimStart()
        {
            if (Count == 0)
            {
                return;
            }

            // Strip leading and 
            this[0].TrimStart();
            Reset();
        }

        public void TrimEnd()
        {
            if (Count == 0)
            {
                return;
            }

            this[Count - 1].TrimEnd();
            Reset();
        }

        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            bool firstLine = true;
            foreach (var line in this)
            {
                if (!firstLine)
                {
                    stringBuilder.Append('\n');
                }
                stringBuilder.Append(line);
                firstLine = false;
            }
            return stringBuilder.ToString();
        }

        public class State
        {
            public State(StringLine currentLine, int linePosition, int columnPosition, char current, char previousChar1, char previousChar2)
            {
                CurrentLine = currentLine;
                LinePosition = linePosition;
                ColumnPosition = columnPosition;
                Current = current;
                PreviousChar1 = previousChar1;
                PreviousChar2 = previousChar2;
            }

            public readonly StringLine CurrentLine;

            public readonly int LinePosition;

            public readonly int ColumnPosition;

            public readonly char Current;

            public readonly char PreviousChar1;

            public readonly char PreviousChar2;
        }
    }
}