// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.IO;

namespace Markdig.Helpers
{
    /// <summary>
    /// A line reader from a <see cref="TextReader"/> that can provide precise source position
    /// </summary>
    public struct LineReader
    {
        private readonly string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineReader"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">bufferSize cannot be &lt;= 0</exception>
        public LineReader(string text)
        {
            if (text is null)
                ThrowHelper.ArgumentNullException_text();

            _text = text;
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
        public StringSlice ReadLine()
        {
            string text = _text;
            int sourcePosition = SourcePosition;

            for (int i = sourcePosition; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\r' || c == '\n')
                {
                    var slice = new StringSlice(text, sourcePosition, i - 1);

                    if (c == '\r' && (uint)(i + 1) < (uint)text.Length && text[i + 1] == '\n')
                        i++;

                    SourcePosition = i + 1;
                    return slice;
                }
            }

            if (sourcePosition >= text.Length)
                return default;

            SourcePosition = int.MaxValue;
            return new StringSlice(text, sourcePosition, text.Length - 1);
        }
    }
}