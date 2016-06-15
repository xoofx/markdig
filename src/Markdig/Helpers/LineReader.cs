// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.IO;
using System.Text;

namespace Markdig.Helpers
{
    /// <summary>
    /// A line reader from a <see cref="TextReader"/> that can provide precise source position
    /// </summary>
    public struct LineReader
    {
        private readonly string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineReader"/> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">bufferSize cannot be &lt;= 0</exception>
        public LineReader(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            this.text = text;
            SourcePosition = 0;
        }

        /// <summary>
        /// Gets the char position of the line. Valid for the next line before calling <see cref="ReadLine"/>.
        /// </summary>
        public int SourcePosition { get; private set; }

        /// <summary>
        /// Reads a new line from the underlying <see cref="TextReader"/> and update the <see cref="SourcePosition"/> for the next line.
        /// </summary>
        /// <returns>A new line or null if the end of <see cref="TextReader"/> has been reached</returns>
        public StringSlice? ReadLine()
        {
            if (SourcePosition >= text.Length)
            {
                return null;
            }

            var startPosition = SourcePosition;
            var position = SourcePosition;
            var slice = new StringSlice(text, startPosition, startPosition);
            for (;position < text.Length; position++)
            {
                var c = text[position];
                if (c == '\r' || c == '\n')
                {
                    slice.End = position - 1;
                    if (c == '\r' && position + 1 < text.Length && text[position + 1] == '\n')
                    {
                        position++;
                    }
                    position++;
                    SourcePosition = position;
                    return slice;
                }
            }

            slice.End = position - 1;
            SourcePosition = position;

            return slice;
        }
   }
}