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
    public class LineReader
    {
        private readonly TextReader reader;
        private readonly char[] buffer;
        private readonly bool preciseSourceLocation;

        private int positionInBuffer;
        private int length;

        public const int DefaultBufferSize = 4096;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="preciseSourceLocation">if set to <c>true</c> provides precise source location through the <see cref="SourcePosition"/>.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">bufferSize cannot be &lt;= 0</exception>
        public LineReader(TextReader reader, bool preciseSourceLocation, int bufferSize = DefaultBufferSize)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize), "bufferSize cannot be <= 0");
            this.reader = reader;
            this.preciseSourceLocation = preciseSourceLocation;
            buffer = new char[bufferSize];
            positionInBuffer = -1;
        }

        /// <summary>
        /// Gets the char position of the line. Valid for the next line before calling <see cref="ReadLine"/>.
        /// </summary>
        public int SourcePosition { get; private set; }

        /// <summary>
        /// Reads a new line from the underlying <see cref="TextReader"/> and update the <see cref="SourcePosition"/> for the next line.
        /// </summary>
        /// <returns>A new line or null if the end of <see cref="TextReader"/> has been reached</returns>
        public string ReadLine()
        {
            if (!preciseSourceLocation)
            {
                return reader.ReadLine();
            }

            StringBuilder lineBuffer = null;

            var eol = false;
            while (!eol)
            {
                if (!ReadBuffer())
                {
                    break;
                }
                int positionEol = length;
                int start = positionInBuffer;
                char c = '\0';
                for (;positionInBuffer < length; positionInBuffer++)
                {
                    c = buffer[positionInBuffer];
                    if (c == '\r' || c == '\n')
                    {
                        positionEol = positionInBuffer;
                        positionInBuffer++;
                        SourcePosition++;
                        eol = true;
                        break;
                    }
                }

                var nextLength = positionEol - start;
                if (lineBuffer == null)
                {
                    lineBuffer = StringBuilderCache.Local();
                }
                lineBuffer.Append(buffer, start, nextLength);
                SourcePosition += nextLength;

                if (eol && c == '\r')
                {
                    // If we have reached the end of the buffer, try to load
                    // the next part to read if `\r` is actually followed by a `\n`
                    if (positionInBuffer == length)
                    {
                        ReadBuffer();
                    }

                    if (positionInBuffer < length)
                    {
                        if (buffer[positionInBuffer] == '\n')
                        {
                            positionInBuffer++;
                            SourcePosition++;
                        }
                    }
                }
            }

            return lineBuffer?.ToString();
        }

        private bool ReadBuffer()
        {
            if (positionInBuffer < 0 || positionInBuffer == buffer.Length)
            {
                positionInBuffer = 0;
                length = reader.Read(buffer, 0, buffer.Length);
                if (length == 0)
                {
                    return false;
                }
            }
            return positionInBuffer < length;
        }
    }
}