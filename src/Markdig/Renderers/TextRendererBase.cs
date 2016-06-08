// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
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
    /// <seealso cref="Markdig.Renderers.RendererBase" />
    public abstract class TextRendererBase : RendererBase
    {
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected TextRendererBase(TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            this.Writer = writer;
            // By default we output a newline with '\n' only even on Windows platforms
            Writer.NewLine = "\n";
        }

        /// <summary>
        /// Gets or sets the writer.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">if the value is null</exception>
        public TextWriter Writer
        {
            get { return writer; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
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
    /// <seealso cref="Markdig.Renderers.RendererBase" />
    public abstract class TextRendererBase<T> : TextRendererBase where T : TextRendererBase<T>
    {
        private bool previousWasLine;
        private char[] buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase{T}"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected TextRendererBase(TextWriter writer) : base(writer)
        {
            buffer = new char[1024];
            // We assume that we are starting as if we had previously a newline
            previousWasLine = true;
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

        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(string content)
        {
            previousWasLine = false;
            Writer.Write(content);
            return (T) this;
        }

        /// <summary>
        /// Writes the specified slice.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(StringSlice slice)
        {
            return Write(ref slice);
        }

        /// <summary>
        /// Writes the specified character.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(char content)
        {
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

            previousWasLine = false;
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
            return (T) this;
        }

        /// <summary>
        /// Writes a newline.
        /// </summary>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T WriteLine()
        {
            Writer.WriteLine();
            previousWasLine = true;
            return (T) this;
        }

        /// <summary>
        /// Writes a content followed by a newline.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T WriteLine(string content)
        {
            previousWasLine = true;
            Writer.WriteLine(content);
            return (T) this;
        }

        /// <summary>
        /// Writes the inlines of a leaf inline.
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T WriteLeafInline(LeafBlock leafBlock)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
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