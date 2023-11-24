// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;

namespace Markdig.Syntax.Inlines;

/// <summary>
/// A delimiter used for parsing emphasis.
/// </summary>
/// <seealso cref="DelimiterInline" />
public class EmphasisDelimiterInline : DelimiterInline
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmphasisDelimiterInline" /> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="descriptor">The descriptor.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EmphasisDelimiterInline(InlineParser parser, EmphasisDescriptor descriptor) : base(parser)
    {
        if (descriptor is null)
            ThrowHelper.ArgumentNullException(nameof(descriptor));

        Descriptor = descriptor;
        DelimiterChar = descriptor.Character;
        Content = new StringSlice(ToLiteral());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmphasisDelimiterInline" /> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="content">The content.</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal EmphasisDelimiterInline(InlineParser parser, EmphasisDescriptor descriptor, StringSlice content) : base(parser)
    {
        if (descriptor is null)
            ThrowHelper.ArgumentNullException(nameof(descriptor));

        Descriptor = descriptor;
        DelimiterChar = descriptor.Character;
        Content = content;
    }

    /// <summary>
    /// Gets the descriptor for this emphasis.
    /// </summary>
    public EmphasisDescriptor Descriptor { get; }

    /// <summary>
    /// The delimiter character found.
    /// </summary>
    public char DelimiterChar { get; }

    /// <summary>
    /// The number of delimiter characters found for this delimiter.
    /// </summary>
    public int DelimiterCount { get; set; }

    /// <summary>
    /// The content as a <see cref="StringSlice"/>.
    /// </summary>
    public StringSlice Content;

    public override string ToLiteral()
    {
        if (DelimiterCount == 1)
        {
            return DelimiterChar switch
            {
                '*' => "*",
                '_' => "_",
                '~' => "~",
                '^' => "^",
                '+' => "+",
                '=' => "=",
                _ => DelimiterChar.ToString()
            };
        }

        return new string(DelimiterChar, DelimiterCount);
    }

    public LiteralInline AsLiteralInline()
    {
        return new LiteralInline()
        {
            Content = Content,
            IsClosed = true,
            Span = Span,
            Line = Line,
            Column = Column
        };
    }
}