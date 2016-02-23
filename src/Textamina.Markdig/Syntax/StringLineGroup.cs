using System;
using System.Collections.ObjectModel;
using System.Text;
using Textamina.Markdig.Helpers;
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

        public StringLine Current => currentLine;

        public char CurrentChar { get; private set; }

        public char PreviousChar1 { get; private set; }

        public char PreviousChar2 { get; private set; }

        public void Reset()
        {
            ColumnPosition = -1;
            LinePosition = 0;
            currentLine = Count > 0 ? this[0] : null;
            if (currentLine != null)
            {
                ColumnPosition = currentLine.Start - 1;
            }
            NextChar();
        }

        public void SetColumn(int index)
        {
            if (Current != null && index <= Current.End)
            {
                CurrentChar = Current[index];
                ColumnPosition = index;
                // Don't set previous char
                PreviousChar1 = '\0';
                PreviousChar2 = '\0';
            }
        }

        public char PeekCharOnSameLine()
        {
            if (Current != null && (ColumnPosition+1) <= Current.End)
            {
                return Current[ColumnPosition + 1];
            }
            return (char)0;
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

        public bool IsEndOfLines => CurrentChar == '\0';

        public State Save()
        {
            return new State(currentLine, LinePosition, ColumnPosition, CurrentChar, PreviousChar1, PreviousChar2);
        }

        public void Save(ref State state)
        {
            state.CurrentLine = currentLine;
            state.LinePosition = LinePosition;
            state.ColumnPosition = ColumnPosition;
            state.Current = CurrentChar;
            state.PreviousChar1 = PreviousChar1;
            state.PreviousChar2 = PreviousChar2;
        }

        public void Restore(ref State state)
        {
            currentLine = state.CurrentLine;
            LinePosition = state.LinePosition;
            ColumnPosition = state.ColumnPosition;
            CurrentChar = state.Current;
            PreviousChar1 = state.PreviousChar1;
            PreviousChar2 = state.PreviousChar2;
        }

        public char NextChar()
        {
            PreviousChar2 = PreviousChar1;
            PreviousChar1 = CurrentChar;
            if (currentLine != null)
            {
                ColumnPosition++;
                if (ColumnPosition <= currentLine.End)
                {
                    CurrentChar = currentLine[ColumnPosition];
                }
                else
                {
                    LinePosition++;
                    if (LinePosition < Count)
                    {
                        currentLine = this[LinePosition];
                    }
                    else
                    {
                        currentLine = null;
                        LinePosition = Count;
                    }

                    ColumnPosition = -1;
                    if (currentLine != null)
                    {
                        ColumnPosition = currentLine.Start - 1;
                    }
                    CurrentChar = currentLine != null ? '\n' : '\0';
                }
            }
            else
            {
                CurrentChar = '\0';
            }

            return CurrentChar;
        }

        public bool SkipWhiteSpaces()
        {
            bool hasWhitespaces = false;
            while (CurrentChar.IsWhitespace())
            {
                NextChar();
                hasWhitespaces = true;
            }
            return hasWhitespaces;
        }

        public bool SkipWhiteSpaces(out int newLineCount)
        {
            bool hasWhitespaces = false;
            newLineCount = 0;
            while (CurrentChar.IsWhitespace())
            {
                if (CurrentChar == '\n')
                {
                    newLineCount++;
                }
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

        public struct State
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

            public StringLine CurrentLine;

            public int LinePosition;

            public int ColumnPosition;

            public char Current;

            public char PreviousChar1;

            public char PreviousChar2;
        }
    }
}