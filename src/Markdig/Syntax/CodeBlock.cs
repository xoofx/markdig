// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax;

/// <summary>
/// Represents an indented code block.
/// </summary>
/// <remarks>
/// Related to CommonMark spec: 4.4 Indented code blocks
/// </remarks>
public class CodeBlock : LeafBlock
{
    public class CodeBlockLine
    {
        public StringSlice TriviaBefore { get; set; }
    }

    private List<CodeBlockLine>? _codeBlockLines;
    public List<CodeBlockLine> CodeBlockLines => _codeBlockLines ??= [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public CodeBlock(BlockParser parser) : base(parser)
    {
    }
}