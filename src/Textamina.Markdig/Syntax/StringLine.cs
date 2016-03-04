// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Syntax
{
    public struct StringLine
    {
        public StringLine(ref StringSlice slice) : this()
        {
            Slice = slice;
        }

        public StringLine(ref StringSlice slice, int line, int column, int position)
        {
            Slice = slice;
            Line = line;
            Column = column;
            Position = position;
        }

        public StringSlice Slice;

        public int Line;

        public int Column;

        public int Position;

        public static implicit operator StringSlice(StringLine line)
        {
            return line.Slice;
        }

        public override string ToString()
        {
            return Slice.ToString();
        }
    }
}