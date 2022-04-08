// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Diagnostics;

namespace Markdig.Helpers
{
    internal struct LazySubstring
    {
        public string Text;
        public int Offset;
        public int Length;

        public LazySubstring(string text)
        {
            Text = text;
            Offset = 0;
            Length = text.Length;
        }

        public LazySubstring(string text, int offset, int length)
        {
            Debug.Assert((ulong)offset + (ulong)length < (ulong)text.Length, $"{offset}-{length} in {text}");
            Text = text;
            Offset = offset;
            Length = length;
        }

        public ReadOnlySpan<char> AsSpan() => Text.AsSpan(Offset, Length);

        public override string ToString()
        {
            if (Offset != 0 || Length != Text.Length)
            {
                Text = Text.Substring(Offset, Length);
                Offset = 0;
            }

            return Text;

        }
    }
}
