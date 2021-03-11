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
            if (writer is null) ThrowHelper.ArgumentNullException_writer();
            this.writer = writer;
            this.writer.NewLine = "\n";
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
                if (value is null)
                {
                    ThrowHelper.ArgumentNullException(nameof(value));
                }

                // By default we output a newline with '\n' only even on Windows platforms
                value.NewLine = "\n";
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
        private sealed class Indent
        {
            private readonly string? _constant;
            private readonly string[]? _lines;

            private int position = 0;

            internal Indent(string constant)
            {
                _constant = constant;
            }

            internal Indent(string[] lineSpecific)
            {
                _lines = lineSpecific;
            }

            internal string Next()
            {
                if (_constant != null)
                {
                    return _constant;
                }

                //if (_lines.Count == 0) throw new Exception("Indents empty");
                if (position == _lines!.Length) return string.Empty;

                return _lines![position++];             
            }
        }

        protected bool previousWasLine;
#if !NETCORE
        private char[] buffer;
#endif
        private readonly List<Indent> indents;

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
            indents = new List<Indent>();
        }

        protected internal void Reset()
        {
            if (Writer is StringWriter stringWriter)
            {
                stringWriter.GetStringBuilder().Length = 0;
            }
            else
            {
                ThrowHelper.InvalidOperationException("Cannot reset this TextWriter instance");
            }

            ResetInternal();
        }

        internal void ResetInternal()
        {
            childrenDepth = 0;
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
            if (indent is null) ThrowHelper.ArgumentNullException(nameof(indent));
            indents.Add(new Indent(indent));
        }

        public void PushIndent(string[] lineSpecific)
        {
            if (indents is null) ThrowHelper.ArgumentNullException(nameof(indents));
            indents.Add(new Indent(lineSpecific));

            // ensure that indents are written to the output stream
            // this assumes that calls after PushIndent wil write children content
            previousWasLine = true;
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
                    var indent = indents[i];
                    var indentText = indent.Next();
                    Writer.Write(indentText);
                }
            }
        }


        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Write(string? content)
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
            if (content is null)
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
        /// Writes a newline.
        /// </summary>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T WriteLine(NewLine newLine)
        {
            WriteIndent();
            Writer.Write(newLine.AsString());
            previousWasLine = true;
            return (T)this;
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
            if (leafBlock is null) ThrowHelper.ArgumentNullException_leafBlock();
            var inline = (Inline) leafBlock.Inline!;
          
            while (inline != null)
            {
                Write(inline);
                inline = inline.NextSibling;
            }
            
            return (T) this;
        }
    }
}