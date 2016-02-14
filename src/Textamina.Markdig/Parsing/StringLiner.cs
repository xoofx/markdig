using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Textamina.Markdig.Parsing
{
    public struct StringLiner
    {
        public StringBuilder Text;

        public int Column { get; private set; }

        public int VirtualColumn { get; private set; }

        public char Current { get; private set; }
        
        public bool IsEol => Column == Text.Length;

        private bool isBlankLine;

        public void ToEol()
        {
            Column = Text.Length;
            Current = '\0';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char NextChar()
        {
            Column++;
            if (Column < Text.Length)
            {
                // If previous character was a tab make the VirtualColumn += 4
                VirtualColumn++;
                if (Utility.IsTab(Current))
                {
                    VirtualColumn += 3;
                }
                Current = Text[Column];
            }
            else
            {
                Column = Text.Length;
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

            for (int i = Column; i < Text.Length; i++)
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
            Column = 0;
            VirtualColumn = 0;
            Current = Text.Length > 0 ? Text[0] : (char) 0;
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
            return Column < Text.Length ? Text.ToString().Substring(Column) : string.Empty;
        }
    }
}