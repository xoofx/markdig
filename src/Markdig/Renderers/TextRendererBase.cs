// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers
{
    /// <summary>
    /// A text based <see cref="IMarkdownRenderer"/>.
    /// </summary>
    /// <seealso cref="RendererBase" />
    public abstract class TextRendererBase : RendererBase
    {
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected TextRendererBase(TextWriter writer)
        {
            if (writer == null) ThrowHelper.ArgumentNullException_writer();
            this.Writer = writer;
            // By default we output a newline with '\n' only even on Windows platforms
            Writer.NewLine = "\n";
        }

        /// <summary>
        /// Gets or sets the writer.
        /// </summary>
        /// <exception cref="ArgumentNullException">if the value is null</exception>
        public TextWriter Writer
        {
            get { return writer; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ArgumentNullException(nameof(value));
                }

                writer = value;
            }
        }

        /// <summary>
        /// Renders the specified markdown object (returns the <see cref="Writer"/> as a render object).
        /// </summary>
        /// <param name="markdownObject">The markdown object.</param>
        /// <returns></returns>
        public override object Render(MarkdownObject markdownObject)
        {
            Write(markdownObject);
            return Writer;
        }
    }

    /// <summary>
    /// Typed <see cref="TextRendererBase"/>.
    /// </summary>
    /// <typeparam name="T">Type of the renderer</typeparam>
    /// <seealso cref="RendererBase" />
    public abstract class TextRendererBase<T> : TextRendererBase where T : TextRendererBase<T>
    {
        private bool previousWasLine;
#if !NETCORE
        private char[] buffer;
#endif
        private readonly List<string> indents;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase{T}"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected TextRendererBase(TextWriter writer) : base(writer)
        {
#if !NETCORE
            buffer = new char[1024];
#endif
            // We assume that we are starting as if we had previously a newline
            previousWasLine = true;
            indents = new List<string>();
        }

        internal void Reset()
        {
            if (Writer is StringWriter stringWriter)
            {
                stringWriter.GetStringBuilder().Length = 0;
            }
            else
            {
                ThrowHelper.InvalidOperationException("Cannot reset this TextWriter instance");
            }

            previousWasLine = true;
            indents.Clear();
        }

        /// <summary>
        /// Ensures a newline.
        /// </summary>
        /// <returns>This instance</returns>
        public T EnsureLine()
        {
            if (!previousWasLine)
            {
                WriteLine();
            }
            return (T)this;
        }

        public void PushIndent(string indent)
        {
            if (indent == null) ThrowHelper.ArgumentNullException(nameof(indent));
            indents.Add(indent);
        }

        public void PopIndent()
        {
            // TODO: Check
            indents.RemoveAt(indents.Count - 1);
        }

        private void WriteIndent()
        {
            if (previousWasLine)
            {
                previousWasLine = false;
                for (int i = 0; i < indents.Count; i++)
                {
                    Writer.Write(indents[i]);
                }
            }
        }


        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Write(string content)
        {
            WriteIndent();
            previousWasLine = false;
            Writer.Write(content);
            return (T) this;
        }

        /// <summary>
        /// Writes the specified slice.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Write(ref StringSlice slice)
        {
            if (slice.Start > slice.End)
            {
                return (T) this;
            }
            return Write(slice.Text, slice.Start, slice.Length);
        }

        /// <summary>
        /// Writes the specified slice.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Write(StringSlice slice)
        {
            return Write(ref slice);
        }

        /// <summary>
        /// Writes the specified character.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Write(char content)
        {
            WriteIndent();
            previousWasLine = content == '\n';
            Writer.Write(content);
            return (T) this;
        }

        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>This instance</returns>
        public T Write(string content, int offset, int length)
        {
            if (content == null)
            {
                return (T) this;
            }

            WriteIndent();
            previousWasLine = false;

#if NETCORE
            Writer.Write(content.AsSpan(offset, length));
#else
            if (offset == 0 && content.Length == length)
            {
                Writer.Write(content);
            }
            else
            {
                if (length > buffer.Length)
                {
                    buffer = content.ToCharArray();
                    Writer.Write(buffer, offset, length);
                }
                else
                {
                    content.CopyTo(offset, buffer, 0, length);
                    Writer.Write(buffer, 0, length);
                }
            }
#endif
            return (T) this;
        }

        /// <summary>
        /// Writes a newline.
        /// </summary>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T WriteLine()
        {
            WriteIndent();
            Writer.WriteLine();
            previousWasLine = true;
            return (T) this;
        }

        /// <summary>
        /// Writes a content followed by a newline.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T WriteLine(string content)
        {
            WriteIndent();
            previousWasLine = true;
            Writer.WriteLine(content);
            return (T) this;
        }

        /// <summary>
        /// Writes the inlines of a leaf inline.
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T WriteLeafInline(LeafBlock leafBlock)
        {
            if (leafBlock == null) ThrowHelper.ArgumentNullException_leafBlock();
            var inline = (Inline) leafBlock.Inline;
            if (inline != null)
            {
                while (inline != null)
                {
                    Write(inline);
                    inline = inline.NextSibling;
                }
            }
            return (T) this;
        }
    }
}