// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers;

/// <summary>
/// A text based <see cref="IMarkdownRenderer"/>.
/// </summary>
/// <seealso cref="RendererBase" />
public abstract class TextRendererBase : RendererBase
{
    private TextWriter _writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRendererBase"/> class.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected TextRendererBase(TextWriter writer)
    {
        Writer = writer;
    }

    /// <summary>
    /// Gets or sets the writer.
    /// </summary>
    /// <exception cref="ArgumentNullException">if the value is null</exception>
    public TextWriter Writer
    {
        get => _writer;
        [MemberNotNull(nameof(_writer))]
        set
        {
            if (value is null)
            {
                ThrowHelper.ArgumentNullException(nameof(value));
            }

            // By default we output a newline with '\n' only even on Windows platforms
            value.NewLine = "\n";
            _writer = value;
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
        private readonly string[]? _lineSpecific;
        private int position;

        internal Indent(string constant)
        {
            _constant = constant;
        }

        internal Indent(string[] lineSpecific)
        {
            _lineSpecific = lineSpecific;
        }

        internal string Next()
        {
            if (_constant != null)
            {
                return _constant;
            }

            //if (_lineSpecific.Count == 0) throw new Exception("Indents empty");
            if (position == _lineSpecific!.Length) return string.Empty;

            return _lineSpecific![position++];
        }
    }

    protected bool previousWasLine;
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_1_OR_GREATER
    private char[] buffer;
#endif
    private readonly List<Indent> indents;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRendererBase{T}"/> class.
    /// </summary>
    /// <param name="writer">The writer.</param>
    protected TextRendererBase(TextWriter writer) : base(writer)
    {
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_1_OR_GREATER
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
        _childrenDepth = 0;
        previousWasLine = true;
        indents.Clear();
    }

    /// <summary>
    /// Ensures a newline.
    /// </summary>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T EnsureLine()
    {
        if (!previousWasLine)
        {
            previousWasLine = true;
            Writer.WriteLine();
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected void WriteIndent()
    {
        if (previousWasLine)
        {
            WriteIndentCore();
        }
    }

    private void WriteIndentCore()
    {
        previousWasLine = false;
        for (int i = 0; i < indents.Count; i++)
        {
            var indent = indents[i];
            var indentText = indent.Next();
            Writer.Write(indentText);
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
        Writer.Write(content);
        return (T)this;
    }

    /// <summary>
    /// Writes the specified char repeated a specified number of times.
    /// </summary>
    /// <param name="c">The char to write.</param>
    /// <param name="count">The number of times to write the char.</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T Write(char c, int count)
    {
        WriteIndent();        
        
        for (int i = 0; i < count; i++)
        {
            Writer.Write(c);
        }        

        return (T)this;
    }

    /// <summary>
    /// Writes the specified slice.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Write(ref StringSlice slice)
    {
        Write(slice.AsSpan());
        return (T)this;
    }

    /// <summary>
    /// Writes the specified slice.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Write(StringSlice slice)
    {
        Write(slice.AsSpan());
        return (T)this;
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
        if (content == '\n')
        {
            previousWasLine = true;
        }
        Writer.Write(content);
        return (T)this;
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
        if (content is not null)
        {
            Write(content.AsSpan(offset, length));
        }
        return (T)this;
    }

    /// <summary>
    /// Writes the specified content.
    /// </summary>
    /// <param name="content">The content.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<char> content)
    {
        if (!content.IsEmpty)
        {
            WriteIndent();
            WriteRaw(content);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteRaw(char content) => Writer.Write(content);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteRaw(string? content) => Writer.Write(content);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteRaw(ReadOnlySpan<char> content)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        Writer.Write(content);
#else
        if (content.Length > buffer.Length)
        {
            buffer = content.ToArray();
        }
        else
        {
            content.CopyTo(buffer);
        }
        Writer.Write(buffer, 0, content.Length);
#endif
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
        return (T)this;
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
        return (T)this;
    }

    /// <summary>
    /// Writes a content followed by a newline.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T WriteLine(char content)
    {
        WriteIndent();
        previousWasLine = true;
        Writer.WriteLine(content);
        return (T)this;
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
        Inline? inline = leafBlock.Inline;

        while (inline != null)
        {
            Write(inline);
            inline = inline.NextSibling;
        }

        return (T)this;
    }
}