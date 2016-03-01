using System.IO;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers
{
    public abstract class TextRendererBase : RendererBase
    {
        protected TextRendererBase(TextWriter writer = null)
        {
            this.Writer = writer;
        }

        public TextWriter Writer { get; set; }

        public override object Render(MarkdownObject markdownObject)
        {
            Write(markdownObject);
            return Writer;
        }
    }

    public abstract class TextRendererBase<T> : TextRendererBase where T : TextRendererBase<T>
    {
        private bool previousWasLine;
        private char[] buffer;

        protected TextRendererBase(TextWriter writer = null) : base(writer)
        {
            buffer = new char[1024];
            // We assume that we are starting as if we had previously a newline
            previousWasLine = true;
        }

        public T EnsureLine()
        {
            if (!previousWasLine)
            {
                WriteLine();
            }
            return (T)this;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(string content)
        {
            previousWasLine = false;
            Writer.Write(content);
            return (T) this;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(ref StringSlice slice)
        {
            if (slice.Start > slice.End)
            {
                return (T) this;
            }
            return Write(slice.Text, slice.Start, slice.Length);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(char content)
        {
            previousWasLine = content == '\n';
            Writer.Write(content);
            return (T) this;
        }

        public T Write(string content, int offset, int length)
        {
            previousWasLine = false;
            if (offset == 0 && content.Length == length)
            {
                Writer.Write(content);
            }
            else
            {
                if (length > buffer.Length)
                {
                    buffer = content.ToCharArray(offset, length);
                }
                else
                {
                    content.CopyTo(offset, buffer, 0, length);
                }

                Writer.Write(buffer, 0, length);
            }
            return (T) this;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T WriteLine()
        {
            Writer.WriteLine();
            previousWasLine = true;
            return (T) this;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T WriteLine(string content)
        {
            previousWasLine = true;
            Writer.WriteLine(content);
            return (T) this;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T WriteLeafInline(LeafBlock leafBlock)
        {
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